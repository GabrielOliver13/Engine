using System.Threading.Tasks;
using Engine;
using nkast.Aether.Physics2D.Collision;

namespace Scenes.SpaceShipsWar{
    public class Game : SceneBehaviour
    {
        public static int AreaLimit = 10_000;

        public override void Start()
        {
            BackgroundColor = new(40, 50, 60);

            physics.world.Gravity = Vector2.Zero;

            for(int i = 0; i < 100; i++)
            {
                new SpaceShip();
            }

            Game1.RenderVertices = true;
        }
        
        public override void _Update()
        {
            CameraChangeState(Keys.LeftControl);

            if (Input.Button(Keys.F))
            {
                new SpaceShip();
            }

            SpaceShipsManager._Update();
            BulletManager.Update();
            LineRender.Polygon(Vector2.Zero, 32, AreaLimit, Color.Red, Time.gameTime /2f);
        }

        public static void CameraChangeState(Keys debugKey)
        {
            if (Input.ButtonDown(debugKey))
            {
                if (Input.MouseMiddlePressed)
                    CameraManager.Position += Input.Moviment;
                if (Input.MouseScroll != 0)
                {
                    if (Input.MouseScroll > 0) CameraManager.Zoom *= 1.1f;
                    else CameraManager.Zoom *= 0.9f;
                }
            }
        }
    }

    public static class SpaceShipsManager
    {
        public static List<SpaceShip> SpaceShips = new();
        public static void _Update()
        {
            foreach(var ship in SpaceShips)
            {
                ship.Update();
            }
        }
    }


    public class SpaceShip
    {
        CustomBody body;
        CustomFixture mainFixture;
        CircleFixture shieldFixture;
        public float shieldLife = 100f;
        public float shieldSize = 150;
        public Vector2 Position => body.Position;
        public float Rotation {get; private set;}
        public float rotationSpeed = Rand.Randint(1f, 5f);
        public float speed = Rand.Randint(6, 10);
        public float speedScale = 1f;
        public float shotsPerSeconds = Rand.Randint(0.01f, 0.2f);
        public float damage = Rand.Randint(5f, 20f);
        //public float shotSpeed = 50;
        public float Life {get=>_life; set
            {
                _life = value;
                if (_life < 0){
                    Destroy();
                    DeferredManager.NextFrame(() =>
                    {
                        new SpaceShip();
                    });
                }
            }
        }

        float elapsedShootsTime = 0;
        private float _life = 100f;
        private int shipSize = 30;
        bool shouldShoot = false;
        float visionRange = 500;

        CustomFixture longView;
        public SpaceShip()
        {
            body = new(){Position = new(Rand.Randint(-Game.AreaLimit/2, Game.AreaLimit/2), Rand.Randint(-Game.AreaLimit/2, Game.AreaLimit/2))};
            mainFixture = body.CreateCircle(shipSize, Vector2.Zero);
            SpaceShipsManager.SpaceShips.Add(this);

            mainFixture.CollidesWith = CollisionCat.Ship | CollisionCat.Bullet | CollisionCat.Sensor;
            mainFixture.CollisionCategories = CollisionCat.Ship;
            mainFixture.CustomFixtureTag = this;


            longView = body.CreateCone(visionRange, visionRange / 1.5f, Vector2.Zero, 0);
            longView.IsSensor = true;
            longView.CollisionCategories = CollisionCat.Sensor;
            longView.CollidesWith = CollisionCat.Ship;

            shieldFixture = body.CreateCircle(shieldSize, Vector2.Zero, 0);
            shieldFixture.IsSensor = true;
            shieldFixture.CollisionCategories = CollisionCat.Shield;
            shieldFixture.CollidesWith = CollisionCat.Bullet;
            shieldFixture.CustomFixtureTag = this;

            BehaviorAsync();
        }

        public void Destroy()
        {
            DeferredManager.NextFrame(()=>{
                SpaceShipsManager.SpaceShips.Remove(this);
                body.Destroy();
            });

        }

        public void Update()
        {
            LineRender.Polygon(Position, 3, shipSize, Color.White, body.Rotation);
            if (shieldLife > 0)
                LineRender.Polygon(Position, 5, shieldSize, Color.DodgerBlue, body.Rotation + Time.gameTime * 2);




            if (shouldShoot && Time.Trigger(ref elapsedShootsTime, shotsPerSeconds))
            {
                Bullet.New(Position + Vector2.Rotate(Vector2.UnitX * (shipSize/2 + Bullet.Size/2), body.Rotation), body.Rotation, this);
                speedScale -= Time.deltaTime * 5f;
            }
            else
            {
                speedScale += Time.deltaTime;
                speedScale = Math.Clamp(speedScale, 0f, 1f);
            }

            body.Rotation = Utils.Slerp(body.Rotation, Rotation, Time.deltaTime * rotationSpeed);

            if (Input.ButtonDown(Keys.Space))
                body.LinearVelocity = Vector2.Zero;
            else
                body.LinearVelocity = Vector2.Rotate(Vector2.UnitX * speed * speedScale, body.Rotation);

            longView.debugLinesColor = longView.foundCollisionsCount == 0 ? Color.White : Color.Coral;
        }

        private async void BehaviorAsync()
        {
            while (true)
            {
                if (Vector2.Distance(Position, Vector2.Zero) > Game.AreaLimit)
                    SetRot(Vector2.Zero);
                else
                    SetRot(Position + Vector2.Rotate(Vector2.UnitX, Rotation + Rand.Randint(-3f, 3f)));
                await ForwardNavegationAsync(Rand.Randint(1f, 5f));
                await AttackingAsync();
                
                await TaskRunner.Yield();
            }
        }

        private async Task AttackingAsync()
        {
            //bool hasFoundTarget = true;
            CustomFixture foundTarget = null;
            shouldShoot = false;

            var wrapper = new TimeWrapper(2);
            var giveUpWrapper = new TimeWrapper(3);

            while (wrapper.Up)
            {
                if (foundTarget == null){
                    if (longView.foundCollisionsCount > 0 && longView.TryGetClosestFixture(Position, out foundTarget)){
                        shouldShoot = true;
                    }
                }
                
                if (foundTarget != null){
                    if (longView.ContainsFixture(foundTarget)){
                        SetRot(foundTarget.CustomBody.Position);
                        shouldShoot = true;
                        wrapper.Reset();
                        giveUpWrapper.Reset();
                    }
                    else
                    {
                        if (foundTarget.CustomBody.hasBeenDestroyed == false && giveUpWrapper.Up)
                        {
                            SetRot(foundTarget.CustomBody.Position);
                            shouldShoot = false;
                        }
                        else
                        {
                            foundTarget = null;
                            shouldShoot = false;
                        }
                    }
                }


                

                await TaskRunner.Yield();
            }
        }

        private async Task ForwardNavegationAsync(float seconds)
        {
            var wrapper = new TimeWrapper(seconds);
            //float elapsed = Time.gameTime + seconds;
            while (wrapper.Up)
            {
                if (longView.foundCollisionsCount != 0)
                    break;
                await TaskRunner.Yield();
            }
        }

        private bool TryFindEnemy()
        {
            return false;
        }
        


        private void SetRot(Vector2 at)
        {
            Rotation = MathF.Atan2(at.Y - Position.Y, at.X - Position.X);
        }
    }

    public static class BulletManager
    {
        public static HashSet<Bullet> Bullets = new();
        public static void Update()
        {
            foreach(var bullet in Bullets)
            {
                bullet.Update();
            }
        }

        //public static void Add(Bullet bullet) => Bullets.Add(bullet);
        //public static void Remove(Bullet bullet) => Bullets.Remove(bullet);
    }

    public class Bullet
    {
        public static int Size {get;} = 10;
        CustomBody body;
        CustomFixture mainFixture;
        float elpasedLifeTime;
        SpaceShip ownerOrigin;
        float rot = Rand.Randint(0, 100f);
        private Bullet(float radius, Vector2 from, float direction, SpaceShip ownerOrigin)
        {
            this.ownerOrigin = ownerOrigin;
            body = new(){Position = from};
            mainFixture = body.CreateCircle(Size, Vector2.Zero);
            //body = new CircleBody(radius, from);
            body.Rotation = direction;
            body.LinearVelocity = Vector2.Rotate(Vector2.UnitX * 65, direction);
            mainFixture.CollidesWith = CollisionCat.Ship | CollisionCat.Shield;
            mainFixture.CollisionCategories = CollisionCat.Bullet;
            mainFixture.IsSensor = true;
            mainFixture.CustomFixtureTag = this;
            elpasedLifeTime = Time.gameTime;

            mainFixture.AddCollisionAction((customFixture) =>
            {
                if(customFixture.CustomFixtureTag is SpaceShip ship && ship != ownerOrigin)
                {
                    if (ship.shieldLife > 0)
                    {
                        ship.shieldLife -= ownerOrigin.damage;
                        Destroy();

                    }
                    else
                    {
                        ship.Life -= ownerOrigin.damage;
                        regenerateLife();
                        Destroy();

                    }
                }
            });
        }

        void regenerateLife()
        {
            if (ownerOrigin.Life < 100)
                ownerOrigin.Life += ownerOrigin.damage;
        }

        public static void New(Vector2 from, float direction, SpaceShip ownerOrigin)
        {
            BulletManager.Bullets.Add(new(Size, from, direction, ownerOrigin));
        }

        public void Destroy()
        {
            DeferredManager.NextFrame(()=>{
                body.Destroy();
                BulletManager.Bullets.Remove(this);
            });
        }

        public void Update()
        {
            if(Time.Trigger(ref elpasedLifeTime, 5)){
                Destroy();
            }

            LineRender.Polygon(body.Position, 5, Size, Color.Coral, rot + Time.gameTime * 3);
        }
    }
}

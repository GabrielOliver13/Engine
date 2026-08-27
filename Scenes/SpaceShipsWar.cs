using System.Threading.Tasks;
using Engine;
using nkast.Aether.Physics2D.Collision;

namespace Scenes.SpaceShipsWar{
    public class Game : SceneBehaviour
    {
        public static int AreaLimit = 2000;

        public override void Start()
        {
            BackgroundColor = new(40, 50, 60);

            physics.world.Gravity = Vector2.Zero;
        }
        
        public override void _Update()
        {
            CameraChangeState(Keys.LeftAlt);

            if (Input.Button(Keys.F))
            {
                new SpaceShip(Input.MousePosition);
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
        public Vector2 Position => body.Position;
        public float Rotation {get; private set;}
        public float speed = 3f;
        public float speedScale = 1f;
        public float shotsPerSeconds = 0.1f;
        public float shotSpeed = 50;
        public float Life {get=>_life; set
            {
                _life = value;
                if (_life < 0)
                    Destroy();
            }
        }

        float elapsedShootsTime = 0;
        private float _life = 100f;
        private int shipSize = 100;
        bool shouldShoot = false;

        CustomFixture longView;
        public SpaceShip(Vector2 position)
        {
            body = new(){Position = position};
            mainFixture = body.CreateCircle(shipSize);
            SpaceShipsManager.SpaceShips.Add(this);

            mainFixture.CollidesWith = CollisionCat.Ship | CollisionCat.Bullet | CollisionCat.Sensor;
            mainFixture.CollisionCategories = CollisionCat.Ship;
            mainFixture.CustomFixtureTag = this;


            longView = body.CreateCone(600, 300);
            longView.IsSensor = true;
            longView.CollisionCategories = CollisionCat.Sensor;
            longView.CollidesWith = CollisionCat.Ship;
            //longView = new(300, 100, body.Position);
            //longView.IsSensor = true;

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
            //DirectionalGoTo(Input.Position, speed * speedScale, 3 * Time.deltaTime);
            LineRender.Polygon(Position, 3, shipSize/2, Color.White, body.Rotation);
            if (shouldShoot && Time.Trigger(ref elapsedShootsTime, shotsPerSeconds))
            {
                Bullet.New(Position + Vector2.Rotate(Vector2.UnitX * (shipSize/2 + Bullet.Size/2), body.Rotation), shotSpeed, body.Rotation, this);
            }

            body.LinearVelocity = Vector2.Rotate(Vector2.UnitX * speed, body.Rotation);
            body.Rotation = Utils.Slerp(body.Rotation, Rotation, Time.deltaTime * 2);

            longView.debugLinesColor = longView.foundCollisionsCount == 0 ? Color.White : Color.Coral;
        }

        private async void BehaviorAsync()
        {
            while (true)
            {
                SetRot(Position + Vector2.Rotate(Vector2.UnitX, body.Rotation + Rand.Randint(-3f, 3f)));
                var hasFound = await TryFindingEnemyAsync(Rand.Randint(1f, 5f));
                if (hasFound)
                {
                    //await -> Going in attack
                } else
                {
                    //nothing
                }
                
                await TaskRunner.Yield();
            }
        }

        private async void AttackingAsync()
        {
            bool isStillFinding = true;
            shouldShoot = true;
            while (true)
            {
                
                await TaskRunner.Yield();
            }
        }

        private async Task<bool> TryFindingEnemyAsync(float seconds)
        {
            float elapsed = Time.gameTime;
            while (true)
            {
                if (Time.Trigger(ref elapsed, seconds))
                    return false;
                
                if (TryFindEnemy())
                    return true;

                await TaskRunner.WaitForSeconds(0.1f);
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
        public static int Size {get;} = 25;
        CustomBody body;
        CustomFixture mainFixture;
        float elpasedLifeTime;
        SpaceShip ownerOrigin;
        private Bullet(float radius, Vector2 from, float speed, float direction, SpaceShip ownerOrigin)
        {
            this.ownerOrigin = ownerOrigin;
            body = new(){Position = from};
            mainFixture = body.CreateCircle(Size);
            //body = new CircleBody(radius, from);
            body.Rotation = direction;
            body.LinearVelocity = Vector2.Rotate(Vector2.UnitX * speed, direction);
            mainFixture.CollidesWith = CollisionCat.Ship;
            mainFixture.CollisionCategories = CollisionCat.Bullet;
            mainFixture.IsSensor = true;
            mainFixture.CustomFixtureTag = this;
            elpasedLifeTime = Time.gameTime;

            mainFixture.AddCollisionAction((customFixture) =>
            {
                if(customFixture.CustomFixtureTag is SpaceShip ship && ship != ownerOrigin)
                {
                    ship.Life -= 10f;
                    Destroy();
                }
            });
        }

        public static void New(Vector2 from, float speed, float direction, SpaceShip ownerOrigin)
        {
            BulletManager.Bullets.Add(new(Size, from, speed, direction, ownerOrigin));
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
        }
    }
}

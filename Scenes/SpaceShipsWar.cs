using System.Security.Cryptography;
using System.Threading.Tasks;
using Engine;
using nkast.Aether.Physics2D.Collision;

namespace Scenes.SpaceShipsWar{
    public class Game : SceneBehaviour
    {
        public static int AreaLimit = 10_000;

        SpaceShip target;

        public override void Start()
        {
            BulletManager.Start();
            BasicParticleSystem._Start("warning");
            BackgroundColor = new(40, 50, 60);

            physics.world.Gravity = Vector2.Zero;

            for(int i = 0; i < 200; i++)
            {
                new SpaceShip();
            }

            Game1.RenderVertices = false;
            Next();

        }
        
        public override void _Update()
        {
            BasicParticleSystem._Update();
            Utils.CameraChangeState(Keys.LeftControl);

            if (Input.Button(Keys.F))
            {
                new SpaceShip();
            }

            SpaceShipsManager._Update();
            BulletManager.Update();
            LineRender.Polygon(Vector2.Zero, 32, AreaLimit, Color.Red, Time.gameTime /2f);







            if (target.body.hasBeenDestroyed || Input.Button(Keys.Space)) Next();
            

            CameraManager.Position = Vector2.Lerp(CameraManager.Position, target.Position, Time.deltaTime * 5);
            CameraManager.Rotation = Utils.Slerp(CameraManager.Rotation, -(target.body.Rotation + MathF.PI/2f), Time.deltaTime * 2);
        }

        private void Next()
        {
            target = SpaceShipsManager.SpaceShips[Rand.Randint(0, SpaceShipsManager.SpaceShips.Count)];
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


    public class Particle
    {
        public float lifeTime;
        public TransfRenderer transf;
        private float totalLifeTime;
        public float Percent {get; private set;} = 1f;
        private Color color;
        public Particle(float lifeTime, TransfRenderer transf)
        {
            totalLifeTime = lifeTime;
            this.transf = transf;
            this.lifeTime = lifeTime;
            color = transf.color;
        }

        public void Update()
        {
            Percent = Math.Clamp(lifeTime/totalLifeTime, 0f, 1f);
            lifeTime -= Time.deltaTime;
            transf.color = color * (((int)(Percent * 4))/4f);
            transf.DrawCall();
        }
    }
    public static class BasicParticleSystem
    {
        private static SpriteRenderer renderer;
        private static HashSet<Particle> transfs = new();
        private static List<Particle> toRemove = new();

        static float elapsed = 0;
        public static void _Start(string particle)
        {
            renderer = new(particle);
        }

        public static void Add(Vector2 position, float rotation, string imageName)
        {
            if (Input.ButtonDown(Keys.Space)) return;
            var transf = new TransfRenderer(){position = position, rotation = rotation, renderer = renderer, texture = LoadContent.GetTexture(imageName), scale = 1f};
            transfs.Add(new(2f, transf));
        }

        public static void _Update()
        {
            foreach(var transf in transfs){
                transf.Update();
                if (transf.lifeTime < 0f) toRemove.Add(transf);
            }

            if (Time.Trigger(ref elapsed, 1f))
            {
                Console.WriteLine($"Particles: {transfs.Count} - Bullets: {BulletManager.Bullets.Count} - Ships: {SpaceShipsManager.SpaceShips.Count}");
            }
            foreach(var f in toRemove) transfs.Remove(f);
            toRemove.Clear();

            
        }


    }


    public class DamageCash
    {
        Dictionary<SpaceShip, (float time, int shotsFrom)> cash = new();
        List<SpaceShip> toRemove = new();
        public bool GotDamage(SpaceShip from)
        {
            if (cash.TryAdd(from, (Time.gameTime + 1.5f, 0)))
            {

            }
            else
            {
                var value = cash[from];
                if (Time.gameTime > value.time)
                {
                    
                } 
            }
        }

        
    }

    public class SpaceShip
    {
        public CustomBody body;
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
        public RectangleRenderer lifeRenderer;
        //public float shotSpeed = 50;
        public float Life {get=>_life; set
            {
                _life = value;
                if (_life < 0){
                    Destroy();
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
            lifeRenderer = new(100, 10);
            lifeRenderer.transf.scale = 0.5f;
            body = new(){Position = new(Rand.Randint(-Game.AreaLimit/2, Game.AreaLimit/2), Rand.Randint(-Game.AreaLimit/2, Game.AreaLimit/2))};
            mainFixture = body.CreateCircle(shipSize, Vector2.Zero);
            SpaceShipsManager.SpaceShips.Add(this);
            body.CustomBodyTag = this;

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
            shieldFixture.CustomFixtureTag = "Shield";

            BehaviorAsync();
        }

        public void Destroy()
        {
            DeferredManager.NextFrame(()=>{
                if (SpaceShipsManager.SpaceShips.Remove(this))
                {
                    body.Destroy();
                    new SpaceShip();
                    
                }
            });

        }

        public void Update()
        {
            LineRender.Polygon(Position, 3, shipSize, Color.White, body.Rotation);
            if (shieldLife > 0)
                LineRender.Polygon(Position, 5, shieldSize, Color.DodgerBlue * (_life / 100f), body.Rotation + Time.gameTime * 2);




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
        
            lifeRenderer.transf.position = body.Position + Vector2.Rotate(Vector2.UnitX * -25, body.Rotation);
            lifeRenderer.transf.rotation = body.Rotation + MathF.PI/2f;
            lifeRenderer.transf.destinationRectangle.Width = (int)Life; 

            lifeRenderer.DrawCall();

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
        public static SpriteRenderer bulletSprite;
        public static void Start()
        {
            bulletSprite = new("point");
            bulletSprite.transf.color = Color.Coral;
            
        }
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
        Vector2 shotedFrom;
        private Bullet(float radius, Vector2 from, float direction, SpaceShip ownerOrigin)
        {
            this.ownerOrigin = ownerOrigin;
            body = new(){Position = from};
            shotedFrom = from;
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
                if(customFixture.CustomBody.CustomBodyTag is SpaceShip ship && ship != ownerOrigin)
                {
                    float damage = (1f - Vector2.Distance(shotedFrom, body.Position) / (Game.AreaLimit/2f)) * ownerOrigin.damage;
                    if (damage < 0) damage = 0;
                    if (customFixture.CustomFixtureTag is string)
                    {
                        if (ship.shieldLife > 0){
                            ship.shieldLife -= damage;
                            regenerateShield(damage);
                            Destroy();
                            BasicParticleSystem.Add(body.Position, direction + MathF.PI/2f, "base_warning");

                        }
                    }
                    else
                    {
                        ship.Life -= damage;
                        regenerateLife(damage);
                        Destroy();
                        BasicParticleSystem.Add(body.Position, direction + MathF.PI/2f, "warning");


                    }

                }
            });
        }

        void regenerateLife(float damage)
        {
            if (ownerOrigin.Life < 100)
                ownerOrigin.Life += damage;
        }
        void regenerateShield(float damage)
        {
            if (ownerOrigin.shieldLife < 100)
                ownerOrigin.shieldLife += damage/2f;
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

            BulletManager.bulletSprite.transf.position = body.Position;
            BulletManager.bulletSprite.DrawCall();
            LineRender.Polygon(body.Position, 8, Size * 2, Color.Coral, rot + Time.gameTime * 3);
        }
    }
}

using Engine;

namespace Scenes.PandaRunner{
    public class Game : SceneBehaviour
    {
        ParalaxRender paralax;
        public BaseCreature targetCreature;
        public override void Start()
        {
            Game1.RenderVertices = true;

            TerrainManager.Start();
            LoadContent.Folder("Paralax");
            physics.Gravity = 65;
            paralax = new("Paralax\\Forest\\1", "Paralax\\Forest\\2","Paralax\\Forest\\3","Paralax\\Forest\\4","Paralax\\Forest\\5","Paralax\\Forest\\6","Paralax\\Forest\\7","Paralax\\Forest\\8","Paralax\\Forest\\9","Paralax\\Forest\\10");
            CreatureManager.Creatures.Add(targetCreature = new Panda(Vector2.Zero));


            if (targetCreature is {Life: <= 0 })
            {
                
            }
        }
        
        public override void Update()
        {
            paralax.DrawCall();

            Utils.CameraChangeState(Keys.LeftControl);
            TerrainManager.Update();

            if (Input.MouseRightClicked)
            {
                CreatureManager.Creatures.Add(new BaseCreature(Input.MousePosition, 50, 100));
            }

            CreatureManager.Update();
            BaseEntityManager.Update();

            if (targetCreature != null)
                Camera2D.SmoothFollow(targetCreature.Position - Vector2.UnitY * 150, targetCreature.speed/18f, 50);

        }
    }

    public static class CreatureManager
    {
        public static List<BaseCreature> Creatures = new();

        public static void Update()
        {
            foreach(var creature in Creatures)
            {
                creature.Update();
            }

            if (Input.ButtonDown(Keys.Q))
            {
                foreach(var creature in Creatures)
                    creature.ApplyDamage(10);
            }
            if (Input.ButtonDown(Keys.E))
            {
                foreach(var creature in Creatures)
                    creature.ApplyLife(10);
            }

        }
    }

    public static class BaseEntityManager
    {
        public static List<BaseEntity> Entities = new();
        public static void Update()
        {
            foreach(var entity in Entities)
            {
                entity.Update();
            }
        }
    }


    public class BaseEntity
    {

        
        public float Width {get => fixture.Width;}
        public float Height {get => fixture.Height;}
        public bool IsEntityDestroyed {get; private set;} = false;
        public Vector2 Position {get=> body.Position;}
        private RectFixture fixture;
        private CustomBody body;

        public CollisionCat CollidesWith {get=>fixture.CollidesWith; set=>fixture.CollidesWith = value;}
        public CollisionCat CollisionCategories {get=>fixture.CollisionCategories; set=>fixture.CollisionCategories = value;}
        public object CustomFixtureTag {get=>fixture.CustomFixtureTag; set=>fixture.CustomFixtureTag = value;}
        public Vector2 LinearVelocity {get=>body.LinearVelocity; set=> body.LinearVelocity = value;}
        
        public BaseEntity(Vector2 position, int width, int height)
        {
            body = new(){Position = position};
            fixture = body.CreateRect(width, height, Vector2.Zero, 1f);
            BaseEntityManager.Entities.Add(this);
        }

        public virtual void Update()
        {
            
        }

        public void Destroy()
        {
            if (IsEntityDestroyed) return;
            IsEntityDestroyed = true;
            body.Destroy();
            BaseEntityManager.Entities.Remove(this);

        }
    }


    public class CreatureController
    {
        public BaseCreature creature;

        public virtual void Start()
        {
            
        }

        public virtual void Update()
        {
            
        }
    }


    public class HitBall
    {
        CustomBody body;
        CustomFixture fixture;
        bool hasBeenDestroyed = false;
        public HitBall(Vector2 position, Vector2 velocity)
        {
            body = new();
            fixture = body.CreateCircle(20, Vector2.Zero, 1f);
            body.Position = position;
            body.LinearVelocity = velocity;
            //fixture.IsSensor = true;
            //body.IgnoreGravity = true;
            fixture.Restitution = 1f;
            fixture.CollidesWith = CollisionCat.Ground | CollisionCat.BaseCreature;
            fixture.CollisionCategories = CollisionCat.Bullet;

            fixture.AddCollisionAction((collision) =>
            {
                if (collision.CustomFixtureTag is BaseCreature baseCreature)
                {
                    baseCreature.ApplyDamage(10);
                    Destroy();
                }
            });
        }

        public void Destroy()
        {
            if (hasBeenDestroyed) return;
            hasBeenDestroyed = true;
            body.Destroy();
        }

        public void Update()
        {
            
        }
    }


    public class TargetBullet
    {
        BaseCreature target;
        public TargetBullet(BaseCreature target)
        {
            this.target = target;            
        }

        public void Update()
        {
            
        }
    }
    

    public class BaseCreature : BaseEntity
    {
        public float TotalLifeLimite {get; protected set;} = 100;
        public float Life {get; private set;} = 100;
        public bool IsAlive {get; private set;} = true;

        public float speed = 10;

        RectangleRenderer lifeRenderer;

        private int barHeightThick = 10;
        CreatureController controller = new();
        public bool isMoving = false;
        private int facingDirection = 1;
        public int jumpHeight = 18;
        public int FacingDirection{get=>facingDirection; set
            {
                if (value > facingDirection) facingDirection = 1;
                else if (value < facingDirection) facingDirection = -1;
            }
        }
        
        public BaseCreature(Vector2 position, int width, int height) : base(position, width, height)
        {
            lifeRenderer = new(width, barHeightThick);
            lifeRenderer.transf.color = Color.Coral;

            CollidesWith = CollisionCat.Ground | CollisionCat.Bullet;
            CollisionCategories = CollisionCat.BaseCreature;
            CustomFixtureTag = this;
        }

        public void SetController(CreatureController controller)
        {
            this.controller = controller;
            this.controller.creature = this;
            this.controller.Start();
        }

        public void ApplyDamage(float damage)
        {
            Life -= Math.Abs(damage);
            if (Life < 0)
            {
                Life = 0;
                IsAlive = false;
                Destroy();
            }
            UpdateLifeRendererState();
        }

        public void ApplyLife(float life)
        {
            Life += Math.Abs(life);
            if (Life > TotalLifeLimite){
                Life = TotalLifeLimite;
            }
            UpdateLifeRendererState();
        }

        public override void Update()
        {
            lifeRenderer.transf.position = new(Position.X, Position.Y - Height/2f - barHeightThick/2f);
            lifeRenderer.DrawCall();

            controller.Update();
            LinearVelocity = new Vector2(isMoving ? speed*facingDirection : 0, LinearVelocity.Y);
        }

        public void Jump()
        {
            LinearVelocity = new(LinearVelocity.X, -jumpHeight);
        }

        public virtual void Attack()
        {
            
        }

        private void UpdateLifeRendererState()
        {
            lifeRenderer.transf.destinationRectangle.Width = (int)(Width * (Life / TotalLifeLimite));
        }
    }


    public class PlayerController : CreatureController
    {
        public override void Start()
        {
            
        }
        public override void Update()
        {
            int x = 0;
            if (Input.Button(Keys.A)) --x;
            if (Input.Button(Keys.D)) ++x;

            if (x != 0)
                creature.FacingDirection = x;
            creature.isMoving = x != 0;
            
            if (Input.ButtonDown(Keys.Space))
            {
                creature.Jump();
            }

            if (Input.ButtonDown(Keys.F))
            {
                creature.Attack();
            }
        }
    }

    public class Panda : BaseCreature
    {
        public Panda(Vector2 position) : base(position, 50, 100)
        {
            SetController(new PlayerController());
        }

        public override void Update()
        {
            base.Update();
        }

        public override void Attack()
        {
            float bulletSize = 20;
            new HitBall(Position + FacingDirection * (Width/2f + bulletSize*1.5f) *  Vector2.UnitX, new(18 * FacingDirection, -12));
        }
    }


    public static class TerrainManager
    {
        public static List<Terrain> Terrains = new();
        public static void Start()
        {
            Terrains.Add(new());
        }
        public static void Update()
        {
            foreach(var terrain in Terrains)
            {
                terrain.Update();
            }
        }
    }

    public class Terrain
    {
        private CustomBody body;
        private CustomFixture fixture;
        public Terrain()
        {
            body = new();
            body.BodyType = BodyType.Static;
            fixture = body.CreateRect(10_000, 300, Vector2.Zero, 1f);
            body.Position += Vector2.UnitY * 500;
            
            fixture.CollidesWith = CollisionCat.BaseCreature | CollisionCat.Bullet;
            fixture.CollisionCategories = CollisionCat.Ground;
        }

        public void Update()
        {
            
        }
    }
}

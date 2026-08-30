namespace Engine;

using nkast.Aether.Physics2D.Collision;
using nkast.Aether.Physics2D.Collision.Shapes;
using nkast.Aether.Physics2D.Common;
using nkast.Aether.Physics2D.Dynamics;

public class PhysicsManager
{
    public World world;
    public int Units = 45;
    public HashSet<CustomBody> bodies = new();
    public PhysicsManager(float gravity = 9.8f)
    {
        world = new(Vector2.UnitY * gravity);
    }

    public void _Update()
    {
        world.Step(Time.deltaTime);
        foreach(var body in bodies)
        {
            body.LineRender();
        }
    }
}


public enum CollisionCat
{
    None = Category.None,
    Ship = Category.Cat1,
    Bullet = Category.Cat2,
    Sensor = Category.Cat3,
    Shield = Category.Cat4,
}

public class CustomBody
{
    public Body body;
    public PhysicsManager _physicsManager {get; protected set;}
    public Vector2 Position {get => body.Position * _physicsManager.Units; set => body.Position = value / _physicsManager.Units;}
    public BodyType BodyType {get=>body.BodyType; set=> body.BodyType = value;}
    public float Rotation {get=>body.Rotation; set=>body.Rotation = value;}
    public Vector2 LinearVelocity {get => body.LinearVelocity; set=> body.LinearVelocity = value;}
    public object CustomBodyTag {get; set;}
    // public CollisionCat CollidesWith {get=>(CollisionCat)MainFixture.CollidesWith; set=>MainFixture.CollidesWith = (Category)value;}
    // public CollisionCat CollisionCategories {get=>(CollisionCat)MainFixture.CollisionCategories; set=>MainFixture.CollisionCategories = (Category)value;}
    // public bool IsSensor {get=>MainFixture.IsSensor; set=>MainFixture.IsSensor = value;}
    // private HashSet<CustomBody> foundCollisions = new();

    //protected Fixture MainFixture {get; set;}
    public bool hasBeenDestroyed {get; private set;} = false;
    private List<CustomFixture> customFixtures = new();
    public CustomBody()
    {
        _physicsManager = SceneManager.currentScene.physics;
        body = new();
        body.BodyType = BodyType.Dynamic;
        _physicsManager.world.Add(body);
        _physicsManager = SceneManager.currentScene.physics;
        _physicsManager.bodies.Add(this);
    }

    public CircleFixture CreateCircle(float radius, Vector2 offSet, float density = 0)
    {
        CircleFixture fixture = new(radius, body, this, offSet, density);
        customFixtures.Add(fixture);
        return fixture;
    }

    public RectFixture CreateRect(float width, float height, Vector2 offSet, float density)
    {
        RectFixture fixture = new(width, height, body, this, offSet, density);
        customFixtures.Add(fixture);
        return fixture;
    }

    public ConeFixture CreateCone(float length, float height, Vector2 offSet, float density = 1f)
    {
        ConeFixture fixture = new(length, height, body, this, offSet, density);
        customFixtures.Add(fixture);
        return fixture;
    }

    protected void SetMainFixture()
    {
        // MainFixture = body.FixtureList[0];
        // MainFixture.Tag = this;

        // AddCollisionAction((body)=>{
        //     foundCollisions.Add(body);
        // });
        // AddOnSeparation((body)=>{
        //     foundCollisions.Remove(body);
        // });
    }

    // public void AddCollisionAction(Action<CustomBody> action)
    // {
    //     MainFixture.OnCollision += (a, b, c) =>
    //     {
    //         if (b.Tag is CustomBody body) action(body);
    //         return true;
    //     };
    // }

    // public void AddOnSeparation(Action<CustomBody> action)
    // {
    //     MainFixture.OnSeparation += (a, b, c) =>
    //     {
    //         if (b.Tag is CustomBody body)
    //             action(body);  
    //     };
    // }

    public void Destroy()
    {
        if (hasBeenDestroyed) return;
        _physicsManager.bodies.Remove(this);

        DeferredManager.NextFrame(() =>
        {
            foreach(var fixtures in customFixtures)
                fixtures._OnDispose();
            customFixtures.Clear();
            _physicsManager.world.Remove(body);
        });
        hasBeenDestroyed = true;
    }

    public virtual void LineRender()
    {
        if (Game1.RenderVertices == false) return;
        foreach(var fixture in customFixtures)
        {
            fixture.Rendering();
        }
    }

}

/* 
public class CircleBody : CustomBody
{
    
    public float Radius {get; private set;} 
    public CircleBody(float radius, Vector2 position)
    {
        Radius = radius/2f;
        body = _physicsManager.world.CreateCircle(Radius / _physicsManager.Units, 1f, Vector2.Zero, BodyType.Dynamic);
        SetMainFixture();
        Position = position;
        MainFixture = body.FixtureList[0];
        MainFixture.Tag = this;
    }

    public override void LineRender()
    {
        Engine.LineRender.Polygon(Position, 16, Radius, Color.White);
    }
}


public class RectBody : CustomBody
{
    public float Width {get; private set;}
    public float Height {get; private set;}

    public RectBody(float width, float height, Vector2 position)
    {
        Width = width;
        Height = height;
        body = _physicsManager.world.CreateRectangle(Width / _physicsManager.Units, Height / _physicsManager.Units, 1f, Vector2.Zero, 0, BodyType.Dynamic);
        SetMainFixture();
        Position = position;
    }

    public override void LineRender()
    {
        Engine.LineRender.NormalizedRectangle(Position, Width, Height, Rotation, Color.White);
    }
}


public class ConeBody : CustomBody
{
    public float Length {get; private set;}
    public float Height {get; private set;}

    public ConeBody(float length, float height, Vector2 position)
    {
        Length = length;
        Height = height;

        Vertices vertices = new(){
            Vector2.Zero,
            new(Length / _physicsManager.Units, -height/2f / _physicsManager.Units),
            new(Length / _physicsManager.Units, height/2f / _physicsManager.Units)
        };
        body = _physicsManager.world.CreatePolygon(vertices, 1f, Vector2.Zero, 0, BodyType.Dynamic);
        SetMainFixture();
        Position = position;

    }

    public override void LineRender()
    {
        Engine.LineRender.Cone(Position, Length, Height, Rotation, Color.White);
        //Engine.LineRender.NormalizedRectangle(Position, Width, Height, Rotation, Color.White);
        
    }
}

 */

public abstract class CustomFixture
{
    public Fixture fixture;
    public CustomBody CustomBody {get; protected set;}
    public CollisionCat CollidesWith {get=>(CollisionCat)fixture.CollidesWith; set=>fixture.CollidesWith = (Category)value;}
    public CollisionCat CollisionCategories {get=>(CollisionCat)fixture.CollisionCategories; set=>fixture.CollisionCategories = (Category)value;}
    public bool IsSensor {get=>fixture.IsSensor; set=>fixture.IsSensor = value;}
    public Color debugLinesColor = Color.White;
    private HashSet<CustomFixture> foundCollisions = new();
    public int foundCollisionsCount => foundCollisions.Count;
    public Vector2 OffSet {get; protected set;}
    public object CustomFixtureTag;
    protected void SetFixture(Fixture fixture, CustomBody customBody)
    {
        CustomBody = customBody;
        this.fixture = fixture;

        AddCollisionAction((body)=>{
            foundCollisions.Add(body);
        });
        AddOnSeparation((body)=>{
            foundCollisions.Remove(body);
        });
    }

    public void AddCollisionAction(Action<CustomFixture> action)
    {
        fixture.OnCollision += (a, b, c) =>
        {
            if (b.Tag is CustomFixture fixt){
                action(fixt);
            }
            return true;
        };
    }

    public void _OnDispose()
    {
        foreach(var col in foundCollisions)
                col.foundCollisions.Remove(this);
        foundCollisions.Clear();
    }

    public void AddOnSeparation(Action<CustomFixture> action)
    {
        fixture.OnSeparation += (a, b, c) =>
        {
            if (b.Tag is CustomFixture fixt){
                action(fixt);  
            }
        };
    }

    public bool TryGetClosestFixture(Vector2 closestTo, out CustomFixture closest)
    {
        closest = null;
        //if (foundCollisionsCount == 0) return false;
        float compareDistance = 0;
        foreach(var col in foundCollisions)
        {
            float distance = Vector2.Distance(col.CustomBody.Position, closestTo);
            if (closest == null || distance < compareDistance) {
                closest = col;
                compareDistance = distance;
            }
        }

        return closest != null;
    }

    public bool ContainsFixture(CustomFixture fixture){
        return foundCollisions.Contains(fixture);
    }

    public abstract void Rendering();
}

public class CircleFixture : CustomFixture
{
    public float Radius {get; private set;} 
    public CircleFixture(float radius, Body body, CustomBody from, Vector2 offSet, float density)
    {
        OffSet = offSet;
        Radius = radius;
        SetFixture(body.CreateCircle(radius / from._physicsManager.Units, density, offSet / from._physicsManager.Units), from);
        fixture.Tag = this;
    }

    public override void Rendering()
    {
        LineRender.Polygon(CustomBody.Position + Vector2.Rotate(OffSet, CustomBody.Rotation), 16, Radius, debugLinesColor, CustomBody.Rotation);
    }
}

public class RectFixture : CustomFixture
{
    public float Width {get; private set;}
    public float Height {get; private set;}
    public RectFixture(float width, float height, Body body, CustomBody from, Vector2 offSet, float density)
    {
        OffSet = offSet;
        Width = width;
        Height = height;
        SetFixture(body.CreateRectangle(Width / from._physicsManager.Units, Height / from._physicsManager.Units, density, offSet / from._physicsManager.Units), from);
        fixture.Tag = this;
    }

    public override void Rendering()
    {
        LineRender.NormalizedRectangle(CustomBody.Position + Vector2.Rotate(OffSet, CustomBody.Rotation), Width, Height, CustomBody.Rotation, debugLinesColor);
    }
}


public class ConeFixture : CustomFixture
{
    public float Length {get; private set;}
    public float Height {get; private set;} 
    public ConeFixture(float length, float height, Body body, CustomBody from, Vector2 offSet, float density)
    {
        Length = length;
        Height = height;
        OffSet = offSet;


        Vertices vertices = new(){
            offSet / from._physicsManager.Units,
            offSet / from._physicsManager.Units + new Vector2(Length / from._physicsManager.Units, -height/2f / from._physicsManager.Units),
            offSet / from._physicsManager.Units + new Vector2(Length / from._physicsManager.Units, height/2f / from._physicsManager.Units)
        };

        SetFixture(body.CreatePolygon(vertices, density), from);
        fixture.Tag = this;
    }

    public override void Rendering()
    {
        LineRender.Cone(CustomBody.Position + Vector2.Rotate(OffSet, CustomBody.Rotation), Length, Height, CustomBody.Rotation, debugLinesColor);
    }
}















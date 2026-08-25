namespace Engine;
using nkast.Aether.Physics2D;
using nkast.Aether.Physics2D.Dynamics;

public class PhysicsManager
{
    public World world;
    public int Units = 45;
    public List<CustomBody> bodies = new();
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


public abstract class CustomBody
{
    protected Body body;
    protected PhysicsManager physicsManager;
    public Vector2 Position {get => body.Position * physicsManager.Units; set => body.Position = value / physicsManager.Units;}
    public BodyType BodyType {get=>body.BodyType; set=> body.BodyType = value;}
    public float Rotation {get=>body.Rotation; set=>body.Rotation = value;}
    public CustomBody()
    {
        physicsManager = SceneManager.currentScene.physics;
        physicsManager.bodies.Add(this);
    }

    public abstract void LineRender();

}

public class CircleBody : CustomBody
{
    
    public float Radius {get; private set;} 
    public CircleBody(float radius, Vector2 position)
    {
        Radius = radius/2f;
        body = physicsManager.world.CreateCircle(Radius / physicsManager.Units, 1f, Vector2.Zero, BodyType.Dynamic);
        Position = position;
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
        body = physicsManager.world.CreateRectangle(Width / physicsManager.Units, Height / physicsManager.Units, 1f, Vector2.Zero, 0, BodyType.Dynamic);
        Position = position;
    }

    public override void LineRender()
    {
        Engine.LineRender.NormalizedRectangle(Position, Width, Height, Rotation, Color.White);
    }

}





















using nkast.Aether.Physics2D.Dynamics;

namespace Engine;

public class MainScene : SceneBehaviour
{
    List<CustomBody> bodies = new();
    SpriteRenderer render;
    public MainScene()
    {
        ViewWidth = 1600;
        ViewHeight = 700;
    }

    public override void Start()
    {
        render = new("x");
    }
    
    public override void Update()
    {
        render.DrawCall();
        
        if (Input.MouseRightPressed) CameraManager.Position += Input.Moviment;
        if (Input.MouseLeftPressed) CameraManager.Rotation += Time.deltaTime * 2;
        if (Input.MouseScroll != 0)
        {
            if (Input.MouseScroll > 0) CameraManager.Zoom *= 1.1f;
            else CameraManager.Zoom *= 0.9f;
        }

        LineRender.Polygon(Input.Position, 16, 500, Color.Red);
    }
}

namespace Engine;

public class MainScene : SceneBehaviour
{
    public MainScene()
    {
        ViewWidth = 1600;
        ViewHeight = 700;
    }

    public override void Start()
    {

    }
    
    public override void Update()
    {
        LineRender.Polygon(Input.Position, 16, 200);
    }
}

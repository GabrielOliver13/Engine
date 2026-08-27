using Engine;

namespace Scenes.DebugProject{
    public class Game : SceneBehaviour
    {
        List<CustomBody> bodies = new();
        SpriteRenderer render;
        public Game()
        {
            ViewWidth = 1600;
            ViewHeight = 700;
        }

        public override void Start()
        {
            render = new("x");
        }
        
        public override void _Update()
        {
            render.DrawCall();
            
            if (Input.MouseRightPressed) CameraManager.Position += Input.Moviment;
            if (Input.MouseLeftPressed) CameraManager.Rotation += Time.deltaTime * 2;
            if (Input.MouseScroll != 0)
            {
                if (Input.MouseScroll > 0) CameraManager.Zoom *= 1.1f;
                else CameraManager.Zoom *= 0.9f;
            }

            LineRender.Polygon(Input.MousePosition, 16, 500, Color.Red);
        }
    }
}

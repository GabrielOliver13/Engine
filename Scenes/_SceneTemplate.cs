using Engine;

namespace Scenes._Template{
    public class Game : SceneBehaviour
    {
        RectangleRenderer bg;
        RectangleRenderer renderer;

        public override void Start()
        {
            bg = new(500, 100);
            renderer = new(500, 100);
            renderer.transf.color = Color.Coral;

            renderer.transf.destinationRectangle.Width = 250;
            renderer.transf.scale = 0.5f;
            //renderer.transf.destinationRectangle.X = 250/2;

        }
        
        public override void _Update()
        {
            Utils.CameraChangeState(Keys.LeftControl);

            bg.DrawCall();
            renderer.DrawCall();
            renderer.transf.position = Input.MousePosition;
            renderer.transf.rotation = Time.gameTime;

        }
    }
}

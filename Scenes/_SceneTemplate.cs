using Engine;

namespace Scenes._Template{
    public class Game : SceneBehaviour
    {
        RectangleRenderer bg;
        RectangleRenderer renderer;
        BasicParticleManager manager;
        SpriteRenderer rend;
        public override void Start()
        {
            bg = new(700, 200);
            renderer = new(500, 100);
            renderer.transf.color = Color.Coral;

            renderer.transf.destinationRectangle.Width = 250;
            renderer.transf.scale = 0.5f;
            //renderer.transf.destinationRectangle.X = 250/2;

            manager = new(async(transf) =>
            {
                float timer = Time.gameTime + 1f;
                while (timer > Time.gameTime)
                {
                    transf.DrawCall();
                    await TaskRunner.Yield();
                } 
            });

            rend = new("point");
            bg.transf.color = Color.DodgerBlue;
            bg.transf.scale = 2;
        }
        
        public override void _Update()
        {
            Utils.CameraChangeState(Keys.LeftControl);

            bg.DrawCall();
            renderer.DrawCall();
            //renderer.transf.position = Input.MousePosition;
            //renderer.transf.rotation = Time.gameTime;

            //manager.Add(new(){renderer = rend, position = Input.MousePosition});
        }
    }
}

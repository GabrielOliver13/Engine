using Engine;
using GridPathFinder;

namespace Scenes.PathFinding{
    public class Game : SceneBehaviour
    {
        Grid grid;
        Mapper mapper;
        int size = 50;

        RectangleRenderer renderer;
        List<(int x, int y)> tracking = new();
        bool blockstate = false;
        public override void Start()
        {
            grid = new(25, 16);


            mapper = grid.CreateMapping(0, 0);

            CameraManager.Position += new Vector2(grid.Width * size/2, grid.Height * size/2f);
            renderer = new(size, size);
        }
        
        public override void Update()
        {
            Vector2 pos;
            for(int y = 0; y < grid.Height; y++)
            {
                for(int x = 0; x < grid.Width; x++)
                {
                    pos = new(x * size, y * size);
                    
                    renderer.transf.position = pos + size/2f * Vector2.One;
                    SetColor(x, y);

                    renderer.DrawCall();
                    
                    LineRender.Rectangle(pos.X, pos.Y, size, size, Color.White);
                }
            }
            Utils.CameraChangeState(Keys.LeftAlt);

            Point point = new((int)(Input.MousePosition.X / size), (int)(Input.MousePosition.Y / size));
            if (point.X < grid.Width && point.Y < grid.Height && point.X >= 0 && point.Y >= 0)
            {
                if (Input.MouseLeftClicked)
                {
                    mapper = grid.CreateMapping(point.X, point.Y);
                    tracking.Clear();

                }

                if (Input.MouseRightClicked)
                {
                    tracking = Tracker.CreateTrack(point.X, point.Y, mapper);
                }

                if (Input.MouseMiddleClicked)
                {
                    bool state = grid.GetBlockAt(point.X, point.Y);
                    blockstate = !state;
                }

                if (Input.MouseMiddlePressed && grid.GetBlockAt(point.X, point.Y) != blockstate)
                {
                    grid.SetBlockState(point.X, point.Y, blockstate);
                    grid.UpdateMapper(mapper);
                    tracking.Clear();
                }
                
            }

            for(int i = 1; i < tracking.Count; i++)
            {
                LineRender.Line(Vec(tracking[i-1]), Vec(tracking[i]), Color.DodgerBlue);
            }
        }

        Vector2 Vec((int x, int y) coord)
        {
            return new(coord.x * size + size/2f, coord.y * size + size/2f);
        }

        void SetColor(int x, int y)
        {
            int weight = mapper.weights[y, x];
            if (weight >= 0)
            {
                renderer.transf.color = Color.Lerp(Color.SeaGreen, Color.Crimson, weight / (float)mapper.WeightestValue);
            } else if (weight == -1)
            {
                renderer.transf.color = Color.Gray;
            }else if (weight == -2)
            {
                renderer.transf.color = Color.Black;
            }
        }
    }
}

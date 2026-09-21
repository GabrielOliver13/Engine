using System.Data;
using Engine;

namespace Scenes.Fake3D{
    public class Game : SceneBehaviour
    {
        public static Virtual3D.Camera3D camera;
        Vector3[,] coords;
        int length = 128;
        public Mapper<int> mapper;

        SpriteRenderer dot;
        SpriteRenderer sprite;
        
        public override void Start()
        {
            sprite = new SpriteRenderer("toothless");
        }

        public override void Update()
        {
            Utils.Point(Vector2.Zero, Color.White);
            Utils.CameraChangeState(Keys.LeftControl);
            //sprite.DrawCall();

            Utils.Point(Vector2.Zero, Color.White);
            if (Input.ButtonDown(Keys.F))
            {
                Console.WriteLine(Utils.camera.Position);
                Console.WriteLine(Vector3.Distance(Utils.camera.Position, Vector3.Zero));
            }

        }





        // public override void Start()
        // {
        //     BackgroundColor = Color.Black;
        //     camera = new();

        //     coords = new Vector3[length, length];
        //     for(int z = 0; z < length; z++)
        //     {
        //         for(int x = 0; x < length; x++)
        //         {
        //             coords[z, x] = new(x * 1f, 0, z * 1f);
        //         }
        //     }

        //     camera.speed = 50;
            

        //     mapper = new(Mapper<int>.MapNoiseTexture(LoadContent.GetTexture("large_noise_map")));
        // }

        
        // public override void Update()
        // {
            
        //     float len = 0.5f;
        //     Utils.CameraChangeState(Keys.LeftControl);
        //     camera.Update();
        //     // for(int y = 0; y < mapper.Height; y++)
        //     // {
        //     //     for(int x = 0; x < mapper.Width; x++)
        //     //     {
        //     //         mapper.Set(x, y, new(x * len, (float)Math.Cos(x+y + Time.gameTime * 3) * len, y * len));
        //     //     }
        //     // }
            
        //     float _z = -Time.gameTime * 12;
        //     float _x = -Time.gameTime * 12;

        //     for(int z = (int)_z; z < (200 + (int)_z); z++)
        //     {
        //         for(int x = (int)_x; x < (200 + (int)_x); x++)
        //         {
        //             // bool startState = camera.WorldToScreen(new(x - _x, mapper.Get(x, z) * len, z - _z), out var startVec);
        //             // bool endState1 = camera.WorldToScreen(new(x - _x, mapper.Get(x, z+1) * len,z+1  - _z), out var endVec1);
        //             // bool endState2 = camera.WorldToScreen(new(x+1 - _x, mapper.Get(x+1, z) * len,z  - _z), out var endVec2);
                    

        //             // if (startState && endState1){
        //             //     LineRender.Line(startVec, endVec1, Color.White);
        //             // }
        //             // if (startState && endState2){
        //             //     LineRender.Line(startVec, endVec2, Color.White);
        //             // }

        //             float d = 200;
        //             Vector3 vec = new(x - _x, mapper.Get(x, z) * len, z - _z);
        //             if (camera.WorldToScreen(vec, out var output))
        //             {
        //                 Utils.Point(output, Color.White, Math.Min(Vector3.Distance(camera.Position, vec) / d -1, 0f));
                        
        //             }
        //         }
        //     }
        // }


        void CreateCube(Vector3 position)
        {
            var model = new Virtual3D.Model()
            {
                marks = [
                    new(-1, -1, -1), new(1, -1, -1), new(1, 1, -1), new(-1, 1, -1),
                    new(-1, -1, 1), new(1, -1, 1), new(1, 1, 1), new(-1, 1, 1),
                ],
                vertices = [
                    new(0, 1), new(1, 2), new(2, 3), new(3, 0),
                    new(4, 5), new(5, 6), new(6, 7), new(7, 4),
                    new(0, 4), new(1, 5), new(2, 6), new(3, 7)
                ],
                Position = position
            };
        }
        
    }

    public class Mapper<T>
    {
        public T[,] map;
        public int Height => map.GetLength(0);
        public int Width => map.GetLength(1);
        
        public Mapper(T[,] map)
        {
            this.map = map;
        }
        public T Get(int x, int y)
        {
            return map[Utils.GetCircularValue(y, Height), Utils.GetCircularValue(x, Width)];
        }

        public void Set(int x, int y, T value)
        {
            map[Utils.GetCircularValue(y, Height), Utils.GetCircularValue(x, Width)] = value;
        }

        public static int[,] MapNoiseTexture(Texture2D texture)
        {
            Color[] data = new Color[texture.Width * texture.Height];
            var map = new int[texture.Height, texture.Width];
            texture.GetData(data);
            
            int pointer = 0;
            for(int y = 0; y < texture.Height; y++)
            {
                for(int x = 0; x < texture.Width; x++)
                {
                    map[y, x] = data[pointer].R;
                    pointer++;
                }
            }
            return map;
        }
    }
    
}


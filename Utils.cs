namespace Engine;

public static class Utils
{
    public static Texture2D pixel;
    private static SpriteRenderer point;
    public static void Start()
    {
        pixel = LoadContent.GetTexture("pixel");
        point = new("point");
    }

    public static void Point(Vector2 position, Color color, float scale = 1)
    {
        point.transf.position = position;
        point.transf.color = color;
        point.transf.scale = scale;
        point.DrawCall();
    }

    public static float Slerp(float from, float to, float weight)
    {
        return from + MathHelper.WrapAngle(to - from) * weight;
    }

    public static void Debug()
    {
        Console.WriteLine($"Debug[{Time.gameTime}]");
    }

    public static void CameraChangeState(Keys debugKey)
    {
        if (Input.ButtonDown(debugKey))
        {
            if (Input.MouseMiddlePressed)
                CameraManager.Position += Input.Moviment;
            if (Input.MouseScroll != 0)
            {
                if (Input.MouseScroll > 0) CameraManager.Zoom *= 1.1f;
                else CameraManager.Zoom *= 0.9f;
            }
        }
    }
}





// --- MY OLD STUFF THAT WILL LATER ON BE ADDED --- /

// public static class Utils
// {
//     public static Texture2D pixel;
//     public static SpriteFont font;
//     private static float elapsedWriteLine = 0;
//     private static bool hasOpenDialogFileOpened = false;

//     public static void __Init()
//     {
//         pixel = new(Game1.GraphicDevice, 1, 1);
//         Color[] data = { Color.White };
//         pixel.SetData(data);
//         font = Game1.ContentManager.Load<SpriteFont>($"Fonts\\RobotoMono");
//     }

//     public static void Print(object obj)
//     {
//         if (TickTimer.LocalCheck(ref elapsedWriteLine, 0.1f)) Console.WriteLine(obj);
//     }

//     public static float MoveTowards(float value, float to, float weight)
//     {
//         value += to.CompareTo(value) * weight;
//         if (Math.Abs(to - value) < weight) return to;
//         return value;
//     }

//     public static float Lerp(float a, float b, float t){
//         return a + (b - a) * t;
//     }

//     public static float Slerp(float a, float b, float t)
//     {
//         float delta = (b - a) % (MathF.PI * 2);

//         if (delta > MathF.PI)
//             delta -= MathF.PI * 2;
//         else if (delta < -MathF.PI)
//             delta += MathF.PI * 2;

//         return a + delta * t;
//     }
    
//     public static Vector2 Clamp(Vector2 vec, float min, float max)
//     {
//         return new(Math.Clamp(vec.X, min, max), Math.Clamp(vec.Y, min, max));
//     }

//     public static void OpenFileDialog(string filter, Action<string> result)
//     {
//         // "PNG (*.png)|*.png" // "All files (*.*)|*.*" // "JSON (*.json)|*.json"
//         if (hasOpenDialogFileOpened) return;
//         hasOpenDialogFileOpened = true;
//         using (var dialog = new System.Windows.Forms.OpenFileDialog())
//         {
//             dialog.Title = "Select a file";
//             dialog.Filter = filter;

//             if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
//             {
//                 result(dialog.FileName);

//             }
//             hasOpenDialogFileOpened = false;
//         }
//     }

//     public static void Exit()
//     {
//         Game1.Game.Exit();
//     }

//     public static void Text(object value, Vector2 position, float scale = 0.15f)
//     {
//         new TextRender(value){Position = position, Scale = scale}.FrameRender();
//     }


//     public static Vector2 CreateLetterBox(float width, float height, float fromWidth, float fromHeight)
//     {
//         return new((fromWidth - width) / 2f, (fromHeight - height) / 2f);
//     }

//     public static float CreateScaleFromSizes(int width, int height, int fromWidth, int fromHeight)
//     {
//         return Math.Min((float)fromWidth / width, (float)fromHeight / height);
//     }

//     public static Vector2 GetRectangleDistance(Vector2 point, Rectangle rect){
//         float left   = rect.Left;
//         float right  = rect.Right;
//         float top    = rect.Top;
//         float bottom = rect.Bottom;

//         if (rect.Contains(point))
//         {
//             float distLeft   = point.X - left;
//             float distRight  = right - point.X;
//             float distTop    = point.Y - top;
//             float distBottom = bottom - point.Y;

//             float minDist = Math.Min(Math.Min(distLeft, distRight), Math.Min(distTop, distBottom));

//             if (minDist == distLeft)   return new Vector2(left, point.Y);
//             if (minDist == distRight)  return new Vector2(right, point.Y);
//             if (minDist == distTop)    return new Vector2(point.X, top);
//             return new Vector2(point.X, bottom);
//         }
//         else
//         {
//             float clampedX = MathHelper.Clamp(point.X, left, right);
//             float clampedY = MathHelper.Clamp(point.Y, top, bottom);

//             return new Vector2(clampedX, clampedY);
//         }
//     }

//     public static Vector2 GridVector(Vector2 position, int length) => Vector2.Floor(position / length) * length;

//     public static float PercentSin(float timer) => MathF.Sin(timer)/2 + 0.5f;
//     public static float PercentSin(float timer, float min, float max) => min + (max - min) * (MathF.Sin(timer)/2 + 0.5f);

//     public static float Sin(float timer) => MathF.Sin(timer);
    

//     public static void Dot(Vector2 pos, float scale = 1){Dot(pos, Color.White, scale);}
//     public static void Dot(Vector2 pos, Color color, float scale = 1){new SpriteRender("dot"){Position = pos, Scale = scale, Color = color, Layer = RenderLayer.UI}.FrameRender();}

//     public static int GetCircularValue(int value, int upTo) { return upTo == 0 ? 0 : (value % upTo + upTo) % upTo; }
//     public static float GetCircularValue(float value, float upTo) { return (value % upTo + upTo) % upTo; }

//     public static T GetCircularValue<T>(T[] values, int index) { return values[(index % values.Length + values.Length) % values.Length]; }
//     public static T GetCircularValue<T>(List<T> values, int index) { return values[(index % values.Count + values.Count) % values.Count]; }
    
//     public static int GetMirrorCircular(int value, int count)
//     {
//         if (count <= 1) return 0;
//         int period = count * 2 - 2;
//         value = ((value % period) + period) % period;
//         if (value >= count) value = period - value;
//         return value;
//     }

//     public static float AngleLookAt(Vector2 origin, Vector2 target){
//         return MathF.Atan2(target.Y - origin.Y, target.X - origin.X);
//     }

//     public static void ElapsedAngleLookAt(Vector2 origin, Vector2 target, ref float originAngle, float rotationSpeed){
//         originAngle += MathHelper.Clamp(MathHelper.WrapAngle(AngleLookAt(origin, target) - originAngle), -rotationSpeed, rotationSpeed);
//     }


//     public static void GetClosest<T>(Vector2 referencePosition, ref T foundObject, Vector2? foundObjectPos, T OnIndex_Object, Vector2 OnIndex_ObjectPos){
//         if (foundObject == null || Vector2.Distance(foundObjectPos.Value, referencePosition) > Vector2.Distance(referencePosition, OnIndex_ObjectPos)) foundObject = OnIndex_Object;
//     }

//     public static void GetClosest<T>(T selfObject, Vector2 selfObjectPos, ref T foundObject, Vector2? foundObjectPos, T OnIndex_Object, Vector2 OnIndex_ObjectPos){
//         if (selfObject.Equals(OnIndex_Object) == false && (foundObject == null || Vector2.Distance(foundObjectPos.Value, selfObjectPos) > Vector2.Distance(selfObjectPos, OnIndex_ObjectPos))){
//             foundObject = OnIndex_Object;
//         }
//     }

//     public static float PercentValueBetween(float min, float max, float valueAmountMinMax){
//         /*
//             return a percentage (0f->1f) abount <value> amoung <min> and <max>
//             Ex:
//                 min:50
//                 max:100
//                 value:75
//                 return = 0.5f (50%) -> Because, 75 is halph way from 50 to 100
//         */
//         if (min == max) return 0f;
//         return (valueAmountMinMax - min) / (max - min);
//     }

//     public static bool ContainsInRect(Vector2 posIn, Vector2 posOf, int Width, int Height){
//         if (posOf.X <= posIn.X && posIn.X < posOf.X + Width && posOf.Y <= posIn.Y)
//         {
//             return posIn.Y < posOf.Y + Height;
//         }
//         return false;
//     }
//     public static bool RectCenterIntersection(Vector2 posA, float w_A, float h_A, Vector2 posB, float w_B, float h_B)
//     {
//         if ((posA.X - w_A/2f) < (posB.X + w_B/2f) && (posB.X - w_B/2f) < (posA.X + w_A/2f) && (posA.Y + h_A/2f) < (posB.Y + h_B/2f + h_A))
//         {
//             return (posB.Y - h_B/2f) < (posA.Y + h_A/2f);
//         }

//         return false;
//     }

//     public static bool ContainsInRectCenter(Vector2 point, Vector2 rectPosition, float Width, float Height){
//         point.X += Width/2f;
//         point.Y += Height/2f;
//         if (rectPosition.X <= point.X && point.X < rectPosition.X + Width && rectPosition.Y <= point.Y)
//         {
//             return point.Y < rectPosition.Y + Height;
//         }
//         return false;
//     }

//     public static void DisplayTextureRange(Vector2 Position, List<string> paths, int blocks = 3, int length = 100)
//     {
//         Position.Y -= length + length * blocks/2f;
//         Position.X += length * blocks/2f;
//         for(int i = 0; i < paths.Count; i++)
//         {
//             if (i % blocks == 0) {
//                 Position.X -= blocks * length;
//                 Position.Y += length;
//             }
//             new LengthSpriteRender(paths[i], length){Position = Position}.FrameRender();
//             Position.X += length;
//         }
//     }

//     public static Color ColorsLerp(float amount, params Color[] colors)
//     {
//         if (colors == null || colors.Length == 0)
//             throw new ArgumentException("At least one color is required.");

//         if (colors.Length == 1)
//             return colors[0];

//         amount = MathHelper.Clamp(amount, 0f, 1f);

//         float scaled = amount * (colors.Length - 1);

//         int index = (int)Math.Floor(scaled);

//         // Evita ultrapassar o último índice
//         if (index >= colors.Length - 1)
//             return colors[^1];

//         float localAmount = scaled - index;

//         return Color.Lerp(colors[index], colors[index + 1], localAmount);
//     }
// }









// public static class Mecanics
// {
//     public static string GetSerial() => Guid.NewGuid().ToString("N");
//     public static void SetValueInClassVariable<Value>(object obj, string variable, Value value) => obj.GetType().GetProperty(variable).SetValue(obj, value);
//     public static int fixValue(int value, int lenght)// ?????????????????????????????
//     {
//         return (int)(value / (float)lenght) * lenght;
//     }

//     public static Texture2D CopyTexture2D(Texture2D texture)
//     {
//         Color[] data = new Color[texture.Width * texture.Height];
//         texture.GetData(data);
//         var copy = new Texture2D(Game1.GraphicDevice, texture.Width, texture.Height);
//         copy.SetData(data);
//         return copy;
//     }

//     public static void BasicsForDragAndDrop(){
//         DragAndDrop.Add("RenderVertices", (Vector2 pos) => {Game1.RenderVertices = !Game1.RenderVertices;});
//     }

//     public static Color RandColor() => new(SysRandom.Randint(128, 256), SysRandom.Randint(128, 256), SysRandom.Randint(128, 256));
    
//     public static T[,] FromArrayToXYGrid<T>(int width, int height, T[] array)
//     {
//         int pointer = 0;
//         T[,] grid = new T[height, width];
//         for(int y = 0; y < height; y++)
//         {
//             for(int x = 0; x < width; x++)
//             {
//                 grid[y, x] = array[pointer];
//                 pointer++;
//             }
//         }
//         return grid;
//     }
    
//     public static Dictionary<string, Texture2D> SizedTextureCut(string path, int size)
//     {
//         Dictionary<string, Texture2D> cutTextures = new();
//         Texture2D texture = Image.Get(path);
//         int width = texture.Width / size;
//         int height = texture.Height / size;

//         int IDindex = 0;
//         var data = new Color[size * size];

//         for (int y = 0; y < height; y++)
//         {
//             for (int x = 0; x < width; x++)
//             {
//                 texture.GetData(0, new(size * x, size * y, size, size), data, 0, data.Length);
//                 var cutTexture = new Texture2D(texture.GraphicsDevice, size, size);
//                 cutTextures.Add($"{IDindex}", cutTexture);
//                 cutTexture.SetData(data);
//                 IDindex++;
//             }
//         }
//         return cutTextures;
//     }
// }


// public static class OutDated
// {
//     public static SpriteEffects Direction(SpriteEffects effects, float direction)
//     {
//         return direction > 0 ? SpriteEffects.None : direction < 0 ? SpriteEffects.FlipHorizontally : effects;
//     }
//     public static SpriteEffects Direction(float direction)
//     {
//         return Direction(SpriteEffects.None, direction);
//     }

//     public static float FaceDirection(float originalValue, float newValue)
//     {
//         if (newValue > 0) return 1;
//         if (newValue < 0) return -1;
//         return originalValue;
//     }

//     public static SpriteEffects GetEffectDir(float dir, SpriteEffects effects)
//     {
//         return dir > 0 ? SpriteEffects.None : dir < 0 ? SpriteEffects.FlipHorizontally : effects;
//     }

//     public static void DisplayTextures(List<string> textures, Vector2 position, int length = 100, int leftBlocks = 100)
//     {
//         Vector2 pos = - new Vector2(((leftBlocks > textures.Count ? textures.Count : leftBlocks)-1) * length/2, (MathF.Ceiling(textures.Count / (float)leftBlocks)-1) * length / 2f);
//         for (int i = 0; i < textures.Count; i++)
//         {
//             if (i % leftBlocks == 0 && i > 0)
//             {
//                 pos.Y += length;
//                 pos.X -= length * leftBlocks;
//             }
//             new LengthSpriteRender(textures[i], length){Position = position + pos}.FrameRender();
//             pos.X += length;
//         }
//     }

//     public static Dictionary<T, int> AssignRandomWeights<T>(List<T> values, int length = 100)
//     {
//         Dictionary<T, int> dict = new();
//         foreach(var value in values) dict.Add(value, 0);
//         AssignRandomWeights(dict, length);
//         return dict; 
//     }
//     public static void AssignRandomWeights<T>(Dictionary<T, int> dict, int length = 100)
//     {
//         var keys = dict.Keys.ToList();
//         int upTo = SysRandom.Randint(dict.Count);

//         float concentration = 0.5f;

//         for (int i = 0; i < keys.Count; i++)
//         {
//             if (length > 0)
//             {
//                 int rand = SysRandom.Randint((int)(length * concentration), length);
//                 dict[Utils.GetCircularValue(keys, upTo + i)] = rand;
//                 length-=rand;
//                 concentration = 0;
//             }
//             else
//             {
//                 dict[Utils.GetCircularValue(keys, upTo + i)] = 0;
//             }
//         }

//         if (length>0) dict[keys[SysRandom.Randint(keys.Count)]] += length; 
//     }

//     public static List<Rectangle> CreateGridOfRectangles(int Columns, int Rows, int Width, int Height, int Xpos = 0, int Ypos = 0, int Xspacing = 0, int Yspacing = 0)
//     {
//         List<Rectangle> rectangles = new();
//         for (int row = 0; row < Rows; row++)
//         {
//             for (int column = 0; column < Columns; column++)
//             {
//                 rectangles.Add(new(Xpos + column * Width + column * Xspacing, Ypos + row * Height + row * Yspacing, Width, Height));
//             }
//         }
//         return rectangles;
//     }
// }
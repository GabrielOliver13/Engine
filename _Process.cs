using System.Threading.Tasks;
using Microsoft.Xna.Framework.Audio;
using Newtonsoft.Json.Linq;

namespace Engine;





public abstract class SceneBehaviour
{
    public int ViewWidth {get; init;} = 1600;
    public int ViewHeight {get; init;} = 900;
    public RenderTarget2D RenderTarget;
    public Color BackgroundColor = Color.SlateGray;
    public RendererManager rendererManager = new();
    public PhysicsManager physics = new();
    public Camera2D camera2D = new();
    public DeferredBehaviour deferredBehaviour = new();

    public virtual void Start(){}
    public virtual void _Update(){}
}

public static class SceneManager
{
    public static SceneBehaviour currentScene;

    public static Scene CreateScene<Scene>() where Scene : SceneBehaviour
    {
        var scene = Activator.CreateInstance<Scene>();
        scene.RenderTarget = new(Game1._graphicsDevice, scene.ViewWidth, scene.ViewHeight);
        return scene;
    }
}

public static class SysWindow
{
    public static void SetSize(int width, int height)
    {
        Game1._graphics.PreferredBackBufferWidth = width;
        Game1._graphics.PreferredBackBufferHeight = height;
        Game1._graphics.ApplyChanges();
    }
}



public static class LoadContent
{
    private static Dictionary<string, Texture2D> Textures = new();
    private static Dictionary<string, JObject> Jsons = new();
    private static Dictionary<string, SoundEffect> SoundEffects = new();
 
    public static class NewFile
    {
        public static Texture2D Texture2D(string name, string systemPath)
        {
            var texture = Microsoft.Xna.Framework.Graphics.Texture2D.FromFile(Game1._graphicsDevice, systemPath);
            Textures.Add(name, texture);
            return texture;
        }

        public static JObject Json(string name, string systemPath)
        {
            var json = JObject.Parse(File.ReadAllText(systemPath));
            Jsons.Add(name, json);
            return json;
        }

        public static SoundEffect SoundEffect(string name, string systemPath)
        {
            var soundEffect = Microsoft.Xna.Framework.Audio.SoundEffect.FromFile(systemPath);
            SoundEffects.Add(name, soundEffect);
            return soundEffect;
        }
    }

    public static Texture2D GetTexture(string name) => Textures[name];
    public static JObject GetJson(string name) => Jsons[name];
    public static SoundEffect GetSoundEffect(string name) => SoundEffects[name];

    public static void Folder(params string[] folderNameInAssets)
    {
        List<string> files;
        foreach(var folder in folderNameInAssets)
        {
            files = Mecanics.GetFiles($"{Mecanics.AssetPath}\\{folder}");
            LoadPathsAsAsset(files);
        }
        
    }

    public static void LoadPathsAsAsset(List<string> filesPaths)
    {
        string extension;
        string name;
        foreach(var filePath in filesPaths)
        {
            extension = Path.GetExtension(filePath);
            name = Mecanics.GetFileName(filePath);
            switch (extension)
            {
                case ".png":
                case ".jpeg": NewFile.Texture2D(name, filePath); break;
                case ".json": NewFile.Json(name, filePath); break;
                case ".wav": NewFile.SoundEffect(name, filePath); break;
            }
        }
    }
}


public static class Mecanics
{
    public static string AssetPath {get;} = "..\\..\\..\\Assets";

    public static List<string> GetFiles(string systemPath)
    {
        var files = Directory.GetFiles(systemPath).ToList();
        foreach(var dir in Directory.GetDirectories(systemPath))
            files.AddRange(GetFiles(dir));

        return files;
    }

    public static string GetFileName(string path)
    {
        return Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path)).Substring(AssetPath.Length+1);
    }

    public static List<string> GetFilesInProject(string systemPath)
    {
        
        List<string> files = new();
        GetFilesInProject($"..\\..\\..\\Assets\\{systemPath}", files, "..\\..\\..\\Assets".Length, true);
        return files;
    }
    private static void GetFilesInProject(string systemPath, List<string> files, int indexLengh, bool init = false)
    {
        var foundFiles = Directory.GetFiles(systemPath).Select(x => x.Substring(indexLengh+1)).ToList();
        files.AddRange(foundFiles);
        var directories = Directory.GetDirectories(systemPath);
        foreach(var directory in directories)
            GetFilesInProject(directory, files, indexLengh);
    }
}

public static class Input
{
    public static Vector2 MousePosition {get; private set;}
    public static Vector2 Moviment {get; private set;}

    public static bool MouseLeftPressed {get; private set;} = false;
    public static bool MouseLeftClicked {get; private set;} = false;

    public static bool MouseRightPressed {get; private set;} = false;
    public static bool MouseRightClicked {get; private set;} = false;

    public static bool MouseMiddlePressed {get; private set;} = false;
    public static bool MouseMiddleClicked {get; private set;} = false;

    public static int MouseScroll = 0;

    public static KeyboardState _CurrentKeyboardState {get; private set;}
    public static KeyboardState _PreviestKeyboardState {get; private set;}

    public static MouseState _CurrentMouseState {get; private set;}
    public static MouseState _PreviestMouseState {get; private set;}

    private static Vector2 prevPosition;
    
    public static void _Update()
    {
        GetMouseButtonStates();
        GetKeyboardButtonStates();
        MousePositionUpdate();
    }

    private static void MousePositionUpdate()
    {
        MousePosition = Vector2.Rotate(_CurrentMouseState.Position.ToVector2() / SceneManager.currentScene.camera2D.Zoom, -CameraManager.Rotation);
        Moviment = Vector2.Rotate(_PreviestMouseState.Position.ToVector2() / SceneManager.currentScene.camera2D.Zoom, -CameraManager.Rotation) - MousePosition;
        Vector2 rot = Vector2.Rotate(new(CameraManager.ViewWidth/2 / CameraManager.Zoom, CameraManager.ViewHeight/2f / CameraManager.Zoom), -CameraManager.Rotation);
        MousePosition -= new Vector2(rot.X - CameraManager.Position.X, rot.Y - CameraManager.Position.Y);
    }

    private static void GetMouseButtonStates()
    {
        _PreviestMouseState = _CurrentMouseState;
        _CurrentMouseState = Mouse.GetState();

        MouseRightPressed = _CurrentMouseState.RightButton == ButtonState.Pressed;
        MouseLeftPressed = _CurrentMouseState.LeftButton == ButtonState.Pressed;
        MouseMiddlePressed = _CurrentMouseState.MiddleButton == ButtonState.Pressed;

        MouseRightClicked = MouseRightPressed && _PreviestMouseState.RightButton == ButtonState.Released;
        MouseLeftClicked = MouseLeftPressed && _PreviestMouseState.LeftButton == ButtonState.Released;
        MouseMiddleClicked = MouseMiddlePressed && _PreviestMouseState.MiddleButton == ButtonState.Released;

        MouseScroll = _CurrentMouseState.ScrollWheelValue.CompareTo(_PreviestMouseState.ScrollWheelValue);
    }

    private static void GetKeyboardButtonStates()
    {
        _PreviestKeyboardState = _CurrentKeyboardState;
        _CurrentKeyboardState = Keyboard.GetState();
    }

    public static bool Button(Keys key)
    {
        return ButtonDown(key) && _PreviestKeyboardState.IsKeyUp(key);
    }

    public static bool ButtonDown(Keys key)
    {
        return _CurrentKeyboardState.IsKeyDown(key);
    }
}

public static class Time
{
    public static float deltaTime {get; private set;}
    public static float gameTime {get; private set;}
    public static int FPS {get; private set;} = 0;
    private static float fpsElapsedTime = 0;
    private static int amountFrames = 0;
    public static void _Update(GameTime gametime)
    {
        deltaTime = (float)gametime.ElapsedGameTime.TotalSeconds;
        gameTime += deltaTime;

        GetFpsUpdate();
    }

    private static void GetFpsUpdate()
    {
        if (Trigger(ref fpsElapsedTime, 1f))
        {
            FPS = amountFrames;
            amountFrames = 0;
        }
        else
            amountFrames++;
    }

    public static bool Trigger(ref float elapsed, float seconds)
    {
        if ((elapsed+seconds) < Time.gameTime)
        {
            elapsed = Time.gameTime + seconds;
            return true;
        }
        return false;
    }
}


public static class CameraManager
{
    public static Vector2 Position {get => SceneManager.currentScene.camera2D.Position; set => SceneManager.currentScene.camera2D.Position = value;}
    public static float Rotation {get => SceneManager.currentScene.camera2D.Rotation; set => SceneManager.currentScene.camera2D.Rotation = value;}
    public static float Zoom {get => SceneManager.currentScene.camera2D.Zoom; set => SceneManager.currentScene.camera2D.Zoom = value;}
    public static int ViewWidth => SceneManager.currentScene.RenderTarget.Width;
    public static int ViewHeight => SceneManager.currentScene.RenderTarget.Height;

    public static void _Update()
    {
        LineRender.NormalizedRectangle(Vector2.Zero, ViewWidth, ViewHeight, 0f, Color.Red);
    }
}

public class Camera2D
{
    public Vector2 Position {get; set;}
    public float Zoom {get; set;} = 1f;
    public float Rotation {get; set;} = 0f;
    public Matrix ViewMatrix {get; private set;}
    public Matrix CameraTransform {get; private set;}

    public Camera2D()
    {
        ViewMatrix = Matrix.Identity;
    }

    public void _Update()
    {
        // ViewMatrix = 
        //     Matrix.CreateTranslation(new(Position.X + CameraManager.ViewWidth/2f / Zoom, Position.Y + CameraManager.ViewHeight/2f / Zoom, 0)) *
        //     Matrix.CreateRotationZ(Rotation) * Matrix.CreateScale(Zoom, Zoom, 0);
    
        ViewMatrix =
            Matrix.CreateTranslation(new Vector3(-Position.X, -Position.Y, 0)) *
            Matrix.CreateRotationZ(Rotation) *
            Matrix.CreateScale(Zoom, Zoom, 1f) *
            Matrix.CreateTranslation(new Vector3(
            CameraManager.ViewWidth / 2f,
            CameraManager.ViewHeight / 2f,
        0));
    }
}


public class DeferredBehaviour
{  
    public List<Action> current = new();
    public List<Action> toUpdate = new();
    public DeferredBehaviour(){}

    void Swap()
    {
        List<Action> _current = current;
        current = toUpdate;
        toUpdate = _current;
    }

    public void NextFrameUpdate()
    {
        Swap();
        foreach(var action in toUpdate) action();
        toUpdate.Clear();
    }
}


// public class DeferredBehaviour
// {
//     private List<Action>[] nextFrameBuffers = {new(), new()};
//     public List<Action> currentAdd;
//     public DeferredBehaviour()
//     {
//         currentAdd = nextFrameBuffers[0];
//     }
//     public void NextFrameUpdate()
//     {
//         while(currentAdd.Count > 0){
//             ChangeBuffer(1);
//             ChangeBuffer(0);
//         }
//     }

//     private void ChangeBuffer(int index)
//     {
//         List<Action> inLoop = currentAdd; 
//         currentAdd = nextFrameBuffers[index];
//         foreach(var action in inLoop) action();
//         inLoop.Clear();
//     }
// }

public static class DeferredManager
{
    public static void NextFrame(Action action)
    {
        SceneManager.currentScene.deferredBehaviour.current.Add(action);
    }
}


public class TimeWrapper
{
    float compareTime;
    float extraSeconds;
    public bool BeforeGameTime => Time.gameTime < compareTime;
    public bool AfterGameTime => Time.gameTime > compareTime;

    public TimeWrapper(float extraSeconds)
    {
        this.extraSeconds = extraSeconds;
        Reset();
    }

    public void Reset()
    {
        compareTime = extraSeconds + Time.gameTime;
    }
}

public class BasicParticleManager
{
    Func<TransfRenderer, Task> ActionAsync;
    public Renderer defaultRenderer;
    public BasicParticleManager(Func<TransfRenderer, Task> actionAsync) 
    {
        ActionAsync = actionAsync;
    }

    public async void Add(TransfRenderer transf)
    {
        if (defaultRenderer!=null && transf.renderer == null) transf.renderer = defaultRenderer; 
        await ActionAsync(transf);
    }
}



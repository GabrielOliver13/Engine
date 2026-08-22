using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json.Linq;

namespace Engine;





public abstract class SceneBehaviour
{
    public int ViewWidth {get; init;} = 1600;
    public int ViewHeight {get; init;} = 900;
    public RenderTarget2D RenderTarget;
    public Color BackgroundColor = Color.SlateGray;
    public RendererManager rendererManager = new();

    public virtual void Start(){}
    public virtual void Update(){}
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
    public static string AssetPath {get;} = "..\\..\\..\\Assets\\Sprites";

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
        GetFilesInProject($"..\\..\\..\\Assets\\Sprites\\{systemPath}", files, "..\\..\\..\\Assets\\Sprites".Length, true);
        return files;
    }
    private static void GetFilesInProject(string systemPath, List<string> files, int indexLengh, bool init = false)
    {
        //var foundFiles = Directory.GetFiles(systemPath).Select(x => Path.Combine(Path.GetDirectoryName(x).Substring(indexLengh), Path.GetFileNameWithoutExtension(x))).ToList();
        var foundFiles = Directory.GetFiles(systemPath).Select(x => x.Substring(indexLengh+1)).ToList();
        
        // if (init)
        //     files.AddRange(foundFiles);
        // else
        //     files.AddRange(foundFiles.Select(x => x.Substring(1)));
        
        files.AddRange(foundFiles);


        var directories = Directory.GetDirectories(systemPath);
        foreach(var directory in directories)
            GetFilesInProject(directory, files, indexLengh);
    }
}

public static class Input
{

    public static Vector2 Position {get; private set;}
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
    
    public static void _Update()
    {
        GetMouseButtonStates();
        GetKeyboardButtonStates();
        MousePositionUpdate();
    }

    private static void MousePositionUpdate()
    {
        Position = _CurrentMouseState.Position.ToVector2();
        Moviment = Position - _PreviestMouseState.Position.ToVector2();
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

    public static bool ClickButton(Keys key)
    {
        return PressedButton(key) && _PreviestKeyboardState.IsKeyDown(key);
    }

    public static bool PressedButton(Keys key)
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
        if (elapsed < Time.gameTime)
        {
            elapsed = Time.gameTime + seconds;
            return true;
        }
        return false;
    }
}





// public class TexturesRenderinManager
// {
    
// }

// public struct Data
// {
//     public Texture2D texture;
//     public Vector2 position;
//     public Rectangle? sourceRectangle;
//     public Color color;
//     public float rotation;
//     public Vector2 origin;
//     public float scale;
//     public SpriteEffects effects;
// }


// public class Sprite
// {
    
// }


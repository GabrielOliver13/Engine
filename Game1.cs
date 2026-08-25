global using System.IO;
global using System.Linq;
global using Microsoft.Xna.Framework;
global using Microsoft.Xna.Framework.Graphics;
global using Microsoft.Xna.Framework.Input;
global using System.Collections.Generic;
global using System;

namespace Engine;

public class Game1 : Game
{
    public static GraphicsDeviceManager _graphics {get; private set;}
    public static SpriteBatch _spriteBatch {get; private set;}
    public static GraphicsDevice _graphicsDevice {get; private set;}
    public static Game1 _game {get; private set;}

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        _game = this;

        _game.IsFixedTimeStep = false;
        
    }

    protected override void Initialize()
    {
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _graphicsDevice = GraphicsDevice;

        SceneManager.currentScene = SceneManager.CreateScene<MainScene>();

        SysWindow.SetSize(SceneManager.currentScene.ViewWidth, SceneManager.currentScene.ViewHeight);
        
        
        
        Engine.LoadContent.LoadPathsAsAsset(Directory.GetFiles(Mecanics.AssetPath).ToList());
        Utils.Start();
        SceneManager.currentScene.Start();

    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        Time._Update(gameTime);
        SceneManager.currentScene.Update();
        Input._Update();
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.SetRenderTarget(SceneManager.currentScene.RenderTarget);
        GraphicsDevice.Clear(SceneManager.currentScene.BackgroundColor);
        GraphicsDevice.SetRenderTarget(null);

        _spriteBatch.Begin();

        _spriteBatch.Draw(SceneManager.currentScene.RenderTarget, Vector2.Zero, Color.White);
       // _spriteBatch.Draw(((MainScene)SceneManager.currentScene).spriteRenderer.transf.texture, Vector2.Zero, Color.White);

        SceneManager.currentScene.rendererManager.Draw();
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}

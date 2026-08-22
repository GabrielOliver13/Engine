using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine;


public struct TransfRenderer
{
    public Texture2D texture;
    public Color color = Color.White;
    public Vector2 position;
    public float scale = 1f;
    public float rotation;
    public SpriteEffects effects;
    public Vector2 origin;
    public Rectangle? sourceRectangle;
    public Rectangle destinationRectangle;
    public Renderer renderer;

    public TransfRenderer()
    {
    }
    public void DrawCall()
    {
        SceneManager.currentScene.rendererManager.renders.Add(this);
    }
}


public abstract class Renderer
{
    public abstract int GetWidth();
    public abstract int GetHeight();

    public abstract void Render(TransfRenderer transfRender);
}

public class SpriteRenderer : Renderer
{
    public TransfRenderer transf;

    public SpriteRenderer(Texture2D texture)
    {
        transf = new(){texture = texture, renderer = this};
        transf.origin = new(texture.Width/2f, texture.Height/2f);
    }
    public SpriteRenderer(string name) : this(LoadContent.GetTexture(name)){}

    public override int GetWidth() => transf.texture.Width;
    public override int GetHeight() => transf.texture.Height;


    public override void Render(TransfRenderer transf)
    {
        Game1._spriteBatch.Draw(transf.texture, transf.position, null, transf.color, transf.rotation, transf.origin, transf.scale, transf.effects, 0);
    }
}

public class RendererManager
{
    public List<TransfRenderer> renders = new();

    public void Draw()
    {
        foreach(var render in renders)
        {
            render.renderer.Render(render);
        }
        renders.Clear();
    }
}


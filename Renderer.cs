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

    public TransfRenderer(){}
    public void DrawCall()
    {
        SceneManager.currentScene.rendererManager.renders.Add(this);
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


public class RectangleRenderer : Renderer
{
    public TransfRenderer transf;
    //public Vector2 size;
    public RectangleRenderer(int width, int height)
    {
        transf = new(){
            texture = LoadContent.GetTexture("pixel"), 
            destinationRectangle = new (0, 0, width, height), 
            origin = new(width/2f, height/2f),
            renderer = this
        };
        //size = new(width, height);
    }
    public override int GetWidth() => transf.destinationRectangle.Width;
    public override int GetHeight() => transf.destinationRectangle.Height;

    public override void Render(TransfRenderer transf)
    {
        transf.destinationRectangle.X += (int)transf.position.X;
        transf.destinationRectangle.Y += (int)transf.position.Y;
        transf.origin.X /= transf.destinationRectangle.Width;
        transf.origin.Y /= transf.destinationRectangle.Height;

        Game1._spriteBatch.Draw(transf.texture, transf.destinationRectangle, null, transf.color, transf.rotation, transf.origin, SpriteEffects.None, 0);
    }
}

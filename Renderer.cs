namespace Engine;


public struct TransfLineRenderer
{
    public Vector2 start;
    public Vector2 end;
    public Color color = Color.White;
    public int thick = 1;
    public TransfLineRenderer(){}

    public void _SystemSpriteBatchDraw()
    {
        int width = (int)Vector2.Distance(start, end);
        float rotation = MathF.Atan2(end.Y - start.Y, end.X - start.X);
        Game1._spriteBatch.Draw(Utils.pixel, new Rectangle((int)start.X, (int)start.Y, width, thick), null, color, rotation, Vector2.Zero, SpriteEffects.None, 0);
    }
}


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
        StaticDrawCall(this);
    }

    public static void StaticDrawCall(TransfRenderer renderer)
    {
        SceneManager.currentScene.rendererManager.renders.Add(renderer);
    }
}


public class RendererManager
{
    public List<TransfRenderer> renders = new();
    public List<TransfLineRenderer> lineRenders = new();


    public void Draw()
    {
        foreach(var render in renders)
            render.renderer.Render(render);
        renders.Clear();

        foreach(var render in lineRenders)
            render._SystemSpriteBatchDraw();
        lineRenders.Clear();
    }
}


public abstract class Renderer
{
    public TransfRenderer transf;

    public abstract int GetWidth();
    public abstract int GetHeight();

    public abstract void Render(TransfRenderer transfRender);
    public void DrawCall() => transf.DrawCall();
}


public class SpriteRenderer : Renderer
{
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
    public RectangleRenderer(int width, int height)
    {
        transf = new(){
            destinationRectangle = new (0, 0, width, height), 
            origin = new(width/2f, height/2f),
            renderer = this
        };
    }
    public override int GetWidth() => transf.destinationRectangle.Width;
    public override int GetHeight() => transf.destinationRectangle.Height;

    public override void Render(TransfRenderer transf)
    {
        transf.destinationRectangle.X += (int)transf.position.X;
        transf.destinationRectangle.Y += (int)transf.position.Y;
        transf.origin.X /= transf.destinationRectangle.Width;
        transf.origin.Y /= transf.destinationRectangle.Height;

        Game1._spriteBatch.Draw(Utils.pixel, transf.destinationRectangle, null, transf.color, transf.rotation, transf.origin, SpriteEffects.None, 0);
    }
}


public static class LineRender
{
    public static void Line(Vector2 start, Vector2 end)
    {
        SceneManager.currentScene.rendererManager.lineRenders.Add(new(){start = start, end = end});
    }

    public static void Rectangle(float X, float Y, float Width, float Height)
    {
        Line(new(X, Y), new(X + Width, Y));
        Line(new(X, Y + Height), new(X + Width, Y + Height));
        Line(new(X, X + Height), new(X, Y + Height));
        Line(new(X + Width, X + Height), new(X + Width, Y + Height));
    }

    public static void Rectangle(Rectangle rect)
    {
        Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
    }


    public static void Polygon(Vector2 position, int lines, int diameter)
    {
        float piece = MathF.PI * 2 / lines;
        for(int i = 0; i < lines; i++)
        {
            Line(
                position + Vector2.Rotate(Vector2.UnitX * diameter, piece * (i+1)), 
                position + Vector2.Rotate(Vector2.UnitX * diameter, piece * i)
            );
        }
    }
    
}



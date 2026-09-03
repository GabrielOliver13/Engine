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
    private Texture2D texture;
    public Texture2D Texture {get=> texture ?? renderer.transf.texture; set=>texture = value;}
    public Color color = Color.White;
    public Vector2 position;
    public float scale;
    public float rotation;
    public SpriteEffects effects;
    public Vector2 origin;
    public Rectangle? sourceRectangle;
    public Rectangle destinationRectangle;
    public Renderer renderer;

    public TransfRenderer()
    {
        scale = 1f;
    }

    public TransfRenderer(string imageName) : this()
    {
        texture = LoadContent.GetTexture(imageName);
    }

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
        transf = new(){Texture = texture, renderer = this};
        transf.origin = new(texture.Width/2f, texture.Height/2f);
    }
    public SpriteRenderer(string name) : this(LoadContent.GetTexture(name)){}

    public override int GetWidth() => transf.Texture.Width;
    public override int GetHeight() => transf.Texture.Height;


    public override void Render(TransfRenderer transf)
    {
        Game1._spriteBatch.Draw(transf.Texture, transf.position, null, transf.color, transf.rotation, transf.origin, transf.scale, transf.effects, 0);
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
        
        //transf.origin.X = (transf.destinationRectangle.X + transf.destinationRectangle.Width) /2;
        //transf.origin.Y = (transf.destinationRectangle.Y + transf.destinationRectangle.Height)/ 2;
        transf.origin = new(0.5f, 0.5f);
        transf.destinationRectangle.X += (int)transf.position.X;
        transf.destinationRectangle.Y += (int)transf.position.Y;
        transf.destinationRectangle.Width = (int)(transf.destinationRectangle.Width * transf.scale);
        transf.destinationRectangle.Height = (int)(transf.destinationRectangle.Height * transf.scale);



        Game1._spriteBatch.Draw(Utils.pixel, transf.destinationRectangle, null, transf.color, transf.rotation, transf.origin, SpriteEffects.None, 0);
    }
}


public static class LineRender
{
    public static void Line(Vector2 start, Vector2 end, Color color){
        SceneManager.currentScene.rendererManager.lineRenders.Add(new(){start = start, end = end, color = color, thick = (int)Math.Ceiling(1f / CameraManager.Zoom)});
    }

    public static void Rectangle(float X, float Y, float Width, float Height, Color color)
    {
        Line(new(X, Y), new(X + Width, Y), color);
        Line(new(X, Y + Height), new(X + Width, Y + Height), color);
        Line(new(X, X + Height), new(X, Y + Height), color);
        Line(new(X + Width, X + Height), new(X + Width, Y + Height), color);
    }

    public static void Rectangle(Rectangle rect, Color color)
    {
        Rectangle(rect.X, rect.Y, rect.Width, rect.Height, color);
    }

    public static void NormalizedRectangle(Vector2 position, float Width, float Height, float rotation, Color color)
    {
        Vector2 topLeft = Vector2.Rotate(new(-Width/2f, -Height/2), rotation);
        Vector2 topRight = Vector2.Rotate(new(Width/2f, -Height/2), rotation);
        Vector2 bottomLeft = Vector2.Rotate(new(-Width/2, Height/2f), rotation);
        Vector2 bottomRight = Vector2.Rotate(new(Width/2, Height/2f), rotation);

        Line(position + topLeft, position + topRight, color);
        Line(position + topRight, position + bottomRight, color);
        Line(position + bottomRight, position + bottomLeft, color);
        Line(position + bottomLeft, position + topLeft, color);
    }


    public static void Polygon(Vector2 position, int lines, float diameter, Color color, float rotation = 0)
    {
        float piece = MathF.PI * 2 / lines;
        for(int i = 0; i < lines; i++)
        {
            Line(
                position + Vector2.Rotate(Vector2.UnitX * diameter, piece * (i+1) + rotation), 
                position + Vector2.Rotate(Vector2.UnitX * diameter, piece * i + rotation),
            color);
        }
    }

    public static void Cone(Vector2 position, float length, float height, float rotation, Color color)
    {
        Vector2 topPart = Vector2.Rotate(new Vector2(length, height/2f), rotation);
        Vector2 bottomPart = Vector2.Rotate(new Vector2(length, -height/2f), rotation);
        
        var heighLine = position + topPart;
        var lowLine = position + bottomPart;
        
        Line(position, heighLine, color);
        Line(position, lowLine, color);
        Line(lowLine, heighLine, color);
    }

    
    
}



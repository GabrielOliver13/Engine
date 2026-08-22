using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;

namespace Engine;

public class MainScene : SceneBehaviour
{
    public SpriteRenderer spriteRendererX;


    public MainScene()
    {
        ViewWidth = 800;
        ViewHeight = 300;
        
    }

    public override void Start()
    {
        LoadContent.Folder("TesteScene");

        spriteRendererX = new SpriteRenderer("TesteScene\\otherFolder\\WeBareBears");

    } 
    

    public override void Update()
    {
        spriteRendererX.transf.DrawCall();
        spriteRendererX.transf.position = Input.Position;
    }
}

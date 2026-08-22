using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;

namespace Engine;

public class MainScene : SceneBehaviour
{
    public MainScene()
    {
        ViewWidth = 1600;
        ViewHeight = 700;
    }

    public override void Start()
    {
        LoadContent.Folder("TesteScene");

    } 
    
    public override void Update()
    {
        
    }
}

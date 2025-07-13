// D:\PROJETOS\GraphFacil\Library\Utilities\ParXY.cs
﻿using System.Collections.Generic;

namespace LotoLibrary.Utilities;

public class ParXY
{
    public int X;
    public int Y;
    public ParXY(int x, int y)
    {
        X = x;
        Y = y;
    }
}


public class ParesXY : List<ParXY> { }

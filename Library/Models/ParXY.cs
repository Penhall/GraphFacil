using System.Collections.Generic;

namespace LotoLibrary.Models;

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

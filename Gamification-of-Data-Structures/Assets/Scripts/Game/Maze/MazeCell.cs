using UnityEngine;

public class MazeCell
{
    public int X { get; private set; }
    public int Y { get; private set; }
    public bool IsWall { get; set; }
    public bool IsVisited { get; set; }
    public bool IsLit { get; set; }
    public bool IsEnd { get; set; }
    public GameObject CellObject { get; set; }

    public MazeCell(int x, int y)
    {
        X = x;
        Y = y;
        IsWall = true;
        IsVisited = false;
        IsLit = false;
        IsEnd = false;
        CellObject = null;
    }
} 
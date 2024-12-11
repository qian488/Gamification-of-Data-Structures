using UnityEngine;

/// <summary>
/// 迷宫单元格类
/// 表示迷宫中的一个单元格，可以是墙壁或通道
/// </summary>
/// <remarks>
/// 主要属性：
/// 1. 位置坐标 (X, Y)
/// 2. 单元格状态（是否为墙、是否被访问等）
/// 3. 单元格的游戏对象引用
/// </remarks>
public class MazeCell
{
    /// <summary>单元格在迷宫中的X坐标</summary>
    public int X { get; private set; }
    /// <summary>单元格在迷宫中的Y坐标</summary>
    public int Y { get; private set; }
    /// <summary>是否是墙壁</summary>
    public bool IsWall { get; set; }
    /// <summary>是否已被访问（用于迷宫生成和寻路算法）</summary>
    public bool IsVisited { get; set; }
    /// <summary>是否处于发光状态</summary>
    public bool IsLit { get; set; }
    /// <summary>是否是终点</summary>
    public bool IsEnd { get; set; }
    /// <summary>对应的Unity游戏对象</summary>
    public GameObject CellObject { get; set; }

    /// <summary>
    /// 创建新的迷宫单元格
    /// </summary>
    /// <param name="x">X坐标</param>
    /// <param name="y">Y坐标</param>
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
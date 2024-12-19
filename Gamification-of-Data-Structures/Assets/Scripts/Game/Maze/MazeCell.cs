using UnityEngine;

/// <summary>
/// 迷宫单元格类
/// 表示迷宫中的一个基本单元格，包含位置和状态信息
/// </summary>
/// <remarks>
/// 主要功能：
/// 1. 状态管理：
///    - IsWall：是否为墙壁
///    - IsVisited：是否已访问
///    - IsPath：是否为路径
///    - IsEnd：是否为终点
/// 2. 视觉效果：
///    - IsLit：是否被照亮
///    - HighlightColor：高亮颜色
///    - FloorMaterialType：地板材质类型
/// 
/// 使用方式：
/// - 通过构造函数创建新单元格
/// - 使用属性访问和修改状态
/// - 调用Reset()重置单元格
/// </remarks>
public class MazeCell
{
    public int X { get; private set; }
    public int Y { get; private set; }
    public bool IsWall { get; set; }
    public bool IsVisited { get; set; }
    public bool IsLit { get; set; }
    public bool IsEnd { get; set; }
    public GameObject CellObject { get; set; }
    public bool IsPath { get; set; }
    public Color HighlightColor { get; set; }

    public MazeCell(int x, int y)
    {
        X = x;
        Y = y;
        IsWall = true;
        IsVisited = false;
        IsLit = false;
        IsEnd = false;
        CellObject = null;
        IsPath = false;
        HighlightColor = Color.white;
    }

    public void Reset()
    {
        IsWall = true;
        IsVisited = false;
        IsPath = false;
        HighlightColor = Color.white;
    }
}

public enum FloorMaterialType
{
    Default,       
    PathFloor,      
    HighlightFloor, 
    PlayerMaterial  
} 
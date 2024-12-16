using UnityEngine;

/// <summary>
/// 迷宫单元格类
/// 表示迷宫中的一个基本单元，包含位置和状态信息
/// </summary>
/// <remarks>
/// 主要功能：
/// 1. 存储单元格的基本属性（位置、类型、状态）
/// 2. 管理单元格的视觉状态（墙体、地板、高亮）
/// 3. 提供单元格状态的查询和修改接口
/// 4. 支持材质系统（默认、路径、高亮、玩家）
/// 5. 维护单元格的游戏对象引用
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
    /// <summary>是否是路径</summary>
    public bool IsPath { get; set; }
    /// <summary>高亮颜色</summary>
    public Color HighlightColor { get; set; }

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
        IsPath = false;
        HighlightColor = Color.white;
    }

    /// <summary>
    /// 重置单元格状态
    /// </summary>
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
    Default,        // 默认地板材质
    PathFloor,      // 寻路路径材质
    HighlightFloor, // 高亮材质
    PlayerMaterial  // 玩家行走材质
} 
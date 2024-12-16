using UnityEngine;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// 寻路算法基类
/// 为不同的寻路算法提供基础框架和共享功能
/// </summary>
/// <remarks>
/// 主要功能：
/// 1. 定义寻路算法的基本接口
/// 2. 提供共享的辅助方法
/// 3. 管理路径可视化
/// 4. 处理迷宫单元格的标记
/// </remarks>
public abstract class PathFinder
{
    /// <summary>迷宫数据引用</summary>
    protected MazeCell[,] maze;
    /// <summary>存储找到的路径</summary>
    protected List<Vector2Int> path = new List<Vector2Int>();
    /// <summary>起点坐标</summary>
    protected Vector2Int startPos;
    /// <summary>终点坐标</summary>
    protected Vector2Int endPos;
    /// <summary>记录已访问的单元格</summary>
    protected bool[,] visited;
    
    /// <summary>可移动的四个方向：上、右、下、左</summary>
    protected static readonly Vector2Int[] directions = new Vector2Int[]
    {
        new Vector2Int(-1, 0),
        new Vector2Int(0, 1),
        new Vector2Int(1, 0),
        new Vector2Int(0, -1)
    };

    public PathFinder(MazeCell[,] maze)
    {
        this.maze = maze;
        this.startPos = new Vector2Int(1, 1);  // 起点
        this.endPos = new Vector2Int(maze.GetLength(0) - 2, maze.GetLength(1) - 2);  // 终点
        this.visited = new bool[maze.GetLength(0), maze.GetLength(1)];
    }

    public abstract IEnumerator FindPath();

    public List<Vector2Int> GetPath()
    {
        return path;
    }

    protected bool IsValid(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < maze.GetLength(0) &&
               pos.y >= 0 && pos.y < maze.GetLength(1) &&
               !maze[pos.x, pos.y].IsWall &&
               !visited[pos.x, pos.y];
    }

    protected IEnumerator StartCoroutine(IEnumerator routine)
    {
        while (routine.MoveNext())
        {
            yield return routine.Current;
        }
    }

    protected void MarkCell(Vector2Int pos, Color color)
    {
        if (maze[pos.x, pos.y].CellObject != null)
        {
            var renderer = maze[pos.x, pos.y].CellObject.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                Material material = null;  // 初始化为 null
                if (color == Color.yellow)
                {
                    material = Resources.Load<Material>("Materials/HighlightFloor");
                }
                else if (color == Color.green)
                {
                    material = Resources.Load<Material>("Materials/PathFloor");
                }
                else if (color == Color.gray)
                {
                    material = Resources.Load<Material>("Materials/DefaultFloor");
                }
                else
                {
                    material = Resources.Load<Material>("Materials/FloorMaterial");  // 默认情况
                }

                if (material != null)
                {
                    renderer.material = new Material(material);
                }
            }
        }
    }

    // 添加新方法
    public abstract IEnumerator<YieldInstruction> FindPathStepByStep();
    public abstract Vector2Int GetCurrentExploringPosition();
    public abstract int GetExploredCount();
} 
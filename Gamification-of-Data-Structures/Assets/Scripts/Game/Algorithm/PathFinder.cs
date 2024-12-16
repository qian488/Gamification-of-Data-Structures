using UnityEngine;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// 寻路算法基类
/// 为各种寻路算法提供基础框架
/// </summary>
/// <remarks>
/// 主要功能：
/// 1. 定义寻路算法的通用接口
/// 2. 提供基础的路径查找工具
/// 3. 管理路径的可视化效果
/// 4. 处理算法执行状态
/// 5. 提供算法性能统计
/// 6. 支持分步执行和动画展示
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
        Debug.Log($"Attempting to mark cell at {pos} with color {color}");
        
        if (maze[pos.x, pos.y].CellObject != null)
        {
            Debug.Log($"Found cell object at {pos}");
            Transform bodyTransform = maze[pos.x, pos.y].CellObject.transform.Find("body");
            if (bodyTransform != null)
            {
                Debug.Log($"Found body transform at {pos}");
                var renderer = bodyTransform.GetComponent<MeshRenderer>();
                if (renderer != null)
                {
                    Material material = null;
                    string materialName = "";
                    
                    if (color == Color.yellow)
                    {
                        materialName = "Materials/HighlightFloor";
                    }
                    else if (color == Color.green)
                    {
                        materialName = "Materials/PathFloor";
                    }
                    else if (color == Color.gray)
                    {
                        materialName = "Materials/DefaultFloor";
                    }
                    else
                    {
                        materialName = "Materials/FloorMaterial";
                    }

                    Debug.Log($"Loading material: {materialName}");
                    material = Resources.Load<Material>(materialName);

                    if (material != null)
                    {
                        Debug.Log($"Successfully loaded material {material.name} for cell at {pos}");
                        renderer.material = material;
                    }
                    else
                    {
                        Debug.LogError($"Failed to load material: {materialName}");
                    }
                }
                else
                {
                    Debug.LogError($"No MeshRenderer found on body at {pos}");
                }
            }
            else
            {
                Debug.LogError($"No 'body' child found on floor at {pos}");
            }
        }
        else
        {
            Debug.LogError($"No cell object found at {pos}");
        }
    }

    // 添加新方法
    public abstract IEnumerator<YieldInstruction> FindPathStepByStep();
    public abstract Vector2Int GetCurrentExploringPosition();
    public abstract int GetExploredCount();
} 
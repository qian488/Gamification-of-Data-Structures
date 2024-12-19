using UnityEngine;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// 迷宫寻路算法基类
/// 定义寻路算法的通用接口和基本功能
/// </summary>
/// <remarks>
/// 主要功能：
/// 1. 寻路控制：
///    - StartPathFinding()：开始寻路
///    - StopPathFinding()：停止寻路
///    - ResetPath()：重置路径
/// 2. 路径管理：
///    - HighlightPath()：高亮显示路径
///    - MarkVisited()：标记已访问单元格
/// 3. 状态通知：
///    - OnPathFound：找到路径时的回调
///    - OnVisitCell：访问单元格时的回调
/// 
/// 使用方式：
/// - 继承此类实现具体的寻路算法
/// - 重写FindPath()方法实现算法逻辑
/// - 通过事件系统通知UI更新
/// </remarks>
public abstract class PathFinder
{
    protected MazeCell[,] maze;
    protected List<Vector2Int> path = new List<Vector2Int>();
    protected Vector2Int startPos;
    protected Vector2Int endPos;
    protected bool[,] visited;
    
    /// <summary>可移动的四个方向：上、右、下、左</summary>
    protected static readonly Vector2Int[] directions = new Vector2Int[]
    {
        new Vector2Int(-1, 0),
        new Vector2Int(0, 1),
        new Vector2Int(1, 0),
        new Vector2Int(0, -1)
    };

    protected bool pathFound = false;
    protected List<Vector2Int> finalPath = new List<Vector2Int>();

    public PathFinder(MazeCell[,] maze)
    {
        this.maze = maze;
        this.startPos = new Vector2Int(1, 1);  
        this.endPos = new Vector2Int(maze.GetLength(0) - 2, maze.GetLength(1) - 2);  
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

    public abstract IEnumerator<YieldInstruction> FindPathStepByStep();
    public abstract Vector2Int GetCurrentExploringPosition();
    public abstract int GetExploredCount();

    public bool HasFoundPath()
    {
        return pathFound;
    }

    public List<Vector2Int> GetFinalPath()
    {
        return finalPath;
    }

    protected void ReconstructPath(Dictionary<Vector2Int, Vector2Int> parent)
    {
        finalPath.Clear();
        Vector2Int current = endPos;
        
        while (current != startPos)
        {
            finalPath.Add(current);
            MarkCell(current, Color.green);
            current = parent[current];
        }
        
        finalPath.Add(startPos);
        finalPath.Reverse();
        pathFound = true;
    }
} 
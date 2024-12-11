using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public abstract class PathFinder
{
    protected MazeCell[,] maze;
    protected List<Vector2Int> path = new List<Vector2Int>();
    protected Vector2Int startPos;
    protected Vector2Int endPos;
    protected bool[,] visited;
    
    // 四个方向：上、右、下、左
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
            var renderer = maze[pos.x, pos.y].CellObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                Material newMaterial = new Material(renderer.material);
                newMaterial.color = color;
                renderer.material = newMaterial;
            }
        }
        // 更新步数
        GameUIManager.GetInstance().IncrementSteps();
    }

    // 添加新方法
    public abstract IEnumerator<YieldInstruction> FindPathStepByStep();
    public abstract Vector2Int GetCurrentExploringPosition();
    public abstract int GetExploredCount();
} 
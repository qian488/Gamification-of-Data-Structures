using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public abstract class PathFinder
{
    protected MazeCell[,] maze;
    protected Vector2Int startPos;
    protected Vector2Int endPos;
    protected bool[,] visited;
    protected Dictionary<Vector2Int, Vector2Int> parentMap;
    
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
        this.startPos = new Vector2Int(1, 1);
        this.endPos = new Vector2Int(maze.GetLength(0) - 2, maze.GetLength(1) - 2);
        this.visited = new bool[maze.GetLength(0), maze.GetLength(1)];
        this.parentMap = new Dictionary<Vector2Int, Vector2Int>();
    }

    public abstract IEnumerator FindPath();

    protected bool IsValid(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < maze.GetLength(0) &&
               pos.y >= 0 && pos.y < maze.GetLength(1) &&
               !maze[pos.x, pos.y].IsWall &&
               !visited[pos.x, pos.y];
    }

    protected List<Vector2Int> ReconstructPath()
    {
        List<Vector2Int> path = new List<Vector2Int>();
        Vector2Int current = endPos;

        while (parentMap.ContainsKey(current))
        {
            path.Add(current);
            current = parentMap[current];
        }
        path.Add(startPos);
        path.Reverse();
        return path;
    }
} 
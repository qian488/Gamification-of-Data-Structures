using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BFSPathFinder : PathFinder
{
    private Queue<Vector2Int> queue = new Queue<Vector2Int>();
    private Dictionary<Vector2Int, Vector2Int> parent = new Dictionary<Vector2Int, Vector2Int>();
    private Vector2Int currentExploring;
    private int exploredCount = 0;
    private Queue<Vector2Int> movementPath = new Queue<Vector2Int>();

    public BFSPathFinder(MazeCell[,] maze) : base(maze)
    {
        currentExploring = startPos;
    }

    public override Vector2Int GetCurrentExploringPosition()
    {
        if (movementPath.Count > 0)
        {
            return movementPath.Peek();
        }
        return currentExploring;
    }

    public override int GetExploredCount()
    {
        return exploredCount;
    }

    public override IEnumerator<YieldInstruction> FindPathStepByStep()
    {
        queue.Clear();
        parent.Clear();
        exploredCount = 0;
        movementPath.Clear();
        
        queue.Enqueue(startPos);
        visited[startPos.x, startPos.y] = true;
        MarkCell(startPos, Color.yellow);
        currentExploring = startPos;

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            
            if (current != currentExploring)
            {
                GenerateMovementPath(currentExploring, current);
                
                while (movementPath.Count > 0)
                {
                    yield return new WaitForSeconds(0.1f);
                    currentExploring = movementPath.Dequeue();
                }
            }

            exploredCount++;
            
            if (current == endPos)
            {
                ReconstructPath();
                yield break;
            }

            foreach (var dir in directions)
            {
                Vector2Int next = new Vector2Int(current.x + dir.x, current.y + dir.y);
                if (IsValid(next))
                {
                    queue.Enqueue(next);
                    visited[next.x, next.y] = true;
                    parent[next] = current;
                    MarkCell(next, Color.yellow);
                }
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    private void GenerateMovementPath(Vector2Int from, Vector2Int to)
    {
        movementPath.Clear();
        
        Queue<Vector2Int> pathQueue = new Queue<Vector2Int>();
        Dictionary<Vector2Int, Vector2Int> pathParent = new Dictionary<Vector2Int, Vector2Int>();
        HashSet<Vector2Int> pathVisited = new HashSet<Vector2Int>();
        
        pathQueue.Enqueue(from);
        pathVisited.Add(from);
        
        bool foundPath = false;
        while (pathQueue.Count > 0 && !foundPath)
        {
            Vector2Int current = pathQueue.Dequeue();
            
            if (current == to)
            {
                foundPath = true;
                while (current != from)
                {
                    movementPath.Enqueue(current);
                    current = pathParent[current];
                }
                Vector2Int[] path = movementPath.ToArray();
                movementPath.Clear();
                for (int i = path.Length - 1; i >= 0; i--)
                {
                    movementPath.Enqueue(path[i]);
                }
                break;
            }
            
            foreach (var dir in directions)
            {
                Vector2Int next = new Vector2Int(current.x + dir.x, current.y + dir.y);
                
                if (IsValidMove(next) && !pathVisited.Contains(next))
                {
                    pathQueue.Enqueue(next);
                    pathVisited.Add(next);
                    pathParent[next] = current;
                }
            }
        }
    }

    private bool IsValidMove(Vector2Int pos)
    {
        if (pos.x < 0 || pos.x >= maze.GetLength(0) || 
            pos.y < 0 || pos.y >= maze.GetLength(1))
            return false;
        
        if (maze[pos.x, pos.y].IsWall)
            return false;
        
        return true;
    }

    public override IEnumerator FindPath()
    {
        yield break;
    }

    private void ReconstructPath()
    {
        path.Clear();
        Vector2Int current = endPos;
        
        while (current != startPos)
        {
            path.Add(current);
            MarkCell(current, Color.green);
            current = parent[current];
        }
        
        path.Add(startPos);
        path.Reverse();
    }
}
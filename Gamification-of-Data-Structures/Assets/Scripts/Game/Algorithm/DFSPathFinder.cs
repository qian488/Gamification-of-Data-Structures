using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DFSPathFinder : PathFinder
{
    private Stack<Vector2Int> stack = new Stack<Vector2Int>();
    private Dictionary<Vector2Int, Vector2Int> parentMap = new Dictionary<Vector2Int, Vector2Int>();

    public DFSPathFinder(MazeCell[,] maze) : base(maze) { }

    public override IEnumerator FindPath()
    {
        Debug.Log("Starting DFS path finding...");
        stack.Clear();
        parentMap.Clear();
        path.Clear();

        // 重置访问标记
        for (int i = 0; i < visited.GetLength(0); i++)
            for (int j = 0; j < visited.GetLength(1); j++)
                visited[i, j] = false;

        stack.Push(startPos);
        visited[startPos.x, startPos.y] = true;
        MarkCell(startPos, GameConfig.PathFinding.VisitedColor);

        while (stack.Count > 0)
        {
            Vector2Int current = stack.Peek();
            
            if (current == endPos)
            {
                Debug.Log("Found end position!");
                ReconstructPath();
                yield return StartCoroutine(VisualizePath());
                yield break;
            }

            bool foundPath = false;
            foreach (Vector2Int dir in directions)
            {
                Vector2Int next = new Vector2Int(current.x + dir.x, current.y + dir.y);
                if (IsValid(next))
                {
                    stack.Push(next);
                    visited[next.x, next.y] = true;
                    parentMap[next] = current;
                    MarkCell(next, GameConfig.PathFinding.VisitedColor);
                    foundPath = true;
                    break;
                }
            }

            if (!foundPath)
            {
                stack.Pop();
                MarkCell(current, Color.gray);
            }

            yield return new WaitForSeconds(GameConfig.PathFinding.StepDelay);
        }
    }

    private void ReconstructPath()
    {
        Vector2Int current = endPos;
        while (parentMap.ContainsKey(current))
        {
            path.Add(current);
            current = parentMap[current];
        }
        path.Add(startPos);
        path.Reverse();
    }

    private IEnumerator VisualizePath()
    {
        foreach (var pos in path)
        {
            MarkCell(pos, GameConfig.PathFinding.PathColor);
            yield return new WaitForSeconds(GameConfig.PathFinding.StepDelay / 2);
        }
    }
}
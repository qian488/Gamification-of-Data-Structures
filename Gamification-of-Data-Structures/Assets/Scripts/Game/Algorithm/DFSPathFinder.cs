using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DFSPathFinder : PathFinder
{
    private Stack<Vector2Int> stack = new Stack<Vector2Int>();
    private Vector2Int currentExploring;
    private int exploredCount = 0;
    private Dictionary<Vector2Int, Vector2Int> parent = new Dictionary<Vector2Int, Vector2Int>();
    private HashSet<Vector2Int> inStack = new HashSet<Vector2Int>();

    public DFSPathFinder(MazeCell[,] maze) : base(maze)
    {
        currentExploring = startPos;
    }

    public override Vector2Int GetCurrentExploringPosition()
    {
        return currentExploring;
    }

    public override int GetExploredCount()
    {
        return exploredCount;
    }

    public override IEnumerator<YieldInstruction> FindPathStepByStep()
    {
        stack.Clear();
        parent.Clear();
        inStack.Clear();
        exploredCount = 0;
        
        stack.Push(startPos);
        inStack.Add(startPos);
        visited[startPos.x, startPos.y] = true;
        MarkCell(startPos, Color.yellow); // 标记起点

        while (stack.Count > 0)
        {
            currentExploring = stack.Peek();
            exploredCount++;
            
            if (currentExploring == endPos)
            {
                // 找到终点，重建路径
                ReconstructPath();
                yield break;
            }

            // 尝试找到一个未访问的相邻节点
            bool foundUnvisited = false;
            foreach (var dir in directions)
            {
                Vector2Int next = new Vector2Int(currentExploring.x + dir.x, currentExploring.y + dir.y);
                if (IsValid(next))
                {
                    stack.Push(next);
                    inStack.Add(next);
                    visited[next.x, next.y] = true;
                    parent[next] = currentExploring;
                    MarkCell(next, Color.yellow); // 标记访问过的节点
                    foundUnvisited = true;
                    break;
                }
            }

            // 如果没有找到未访问的相邻节点，进行回溯
            if (!foundUnvisited)
            {
                Vector2Int backtrack = stack.Pop();
                inStack.Remove(backtrack);
                if (backtrack != endPos) // 不要标记终点为灰色
                {
                    MarkCell(backtrack, Color.gray); // 标记回溯的节点
                }
            }

            // 每探索一个节点后等待一帧
            yield return new WaitForSeconds(0.05f);
        }
    }

    // 保留原有的FindPath方法实现
    public override IEnumerator FindPath()
    {
        // 原有的DFS实现...
        yield break;
    }

    private void ReconstructPath()
    {
        path.Clear();
        Vector2Int current = endPos;
        
        while (current != startPos)
        {
            path.Add(current);
            MarkCell(current, Color.green); // 标记最终路径
            current = parent[current];
        }
        
        path.Add(startPos);
        path.Reverse();
    }
}
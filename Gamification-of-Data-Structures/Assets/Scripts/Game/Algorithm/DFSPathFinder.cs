using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 深度优先搜索寻路器
/// 实现基于DFS的迷宫寻路算法
/// </summary>
/// <remarks>
/// 主要功能：
/// 1. 实现深度优先搜索算法
/// 2. 提供实时的路径可视化
/// 3. 支持回溯路径显示
/// 4. 记录搜索过程数据
/// 5. 优化搜索效率
/// 6. 处理边界情况
/// </remarks>
public class DFSPathFinder : PathFinder
{
    /// <summary>用于DFS的栈</summary>
    private Stack<Vector2Int> stack = new Stack<Vector2Int>();
    /// <summary>当前正在探索的位置</summary>
    private Vector2Int currentExploring;
    /// <summary>已探索的节点数量</summary>
    private int exploredCount = 0;
    /// <summary>记录每个位置的父节点</summary>
    private Dictionary<Vector2Int, Vector2Int> parent = new Dictionary<Vector2Int, Vector2Int>();
    /// <summary>记录在栈中的节点</summary>
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

    /// <summary>
    /// 执行DFS寻路的每一步
    /// 包含可视化和回溯效果
    /// </summary>
    public override IEnumerator<YieldInstruction> FindPathStepByStep()
    {
        Debug.Log("Starting DFS path finding");
        stack.Clear();
        parent.Clear();
        inStack.Clear();
        exploredCount = 0;
        
        stack.Push(startPos);
        inStack.Add(startPos);
        visited[startPos.x, startPos.y] = true;
        Debug.Log($"Marking start position: {startPos}");
        MarkCell(startPos, Color.yellow); // 标记起点

        while (stack.Count > 0)
        {
            currentExploring = stack.Peek();
            exploredCount++;
            Debug.Log($"Exploring position: {currentExploring}");
            
            if (currentExploring == endPos)
            {
                Debug.Log("Found end position!");
                ReconstructPath();
                yield break;
            }

            bool foundUnvisited = false;
            foreach (var dir in directions)
            {
                Vector2Int next = new Vector2Int(currentExploring.x + dir.x, currentExploring.y + dir.y);
                if (IsValid(next))
                {
                    Debug.Log($"Found valid next position: {next}");
                    stack.Push(next);
                    inStack.Add(next);
                    visited[next.x, next.y] = true;
                    parent[next] = currentExploring;
                    MarkCell(next, Color.yellow);
                    foundUnvisited = true;
                    break;
                }
            }

            if (!foundUnvisited)
            {
                Debug.Log($"No unvisited neighbors found for {currentExploring}, backtracking");
                Vector2Int backtrack = stack.Pop();
                inStack.Remove(backtrack);
                if (backtrack != endPos)
                {
                    MarkCell(backtrack, Color.gray);
                }
            }

            yield return new WaitForSeconds(0.05f);
        }
    }

    // 保留原有的FindPath方法实现
    public override IEnumerator FindPath()
    {
        yield break;
    }

    /// <summary>
    /// 重建从起点到终点的路径
    /// 并标记最终路径
    /// </summary>
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
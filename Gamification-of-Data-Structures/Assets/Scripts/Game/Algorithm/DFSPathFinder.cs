using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 深度优先搜索寻路算法类
/// 使用DFS策略在迷宫中寻找路径
/// </summary>
/// <remarks>
/// 算法流程：
/// 1. 从起点开始深度搜索：
///    - 标记当前单元格为已访问
///    - 递归探索未访问的相邻单元格
///    - 回溯处理死路
/// 2. 路径处理：
///    - 使用栈记录访问路径
///    - 回溯时移除无效路径
///    - 找到终点时保存路径
/// 
/// 特点：
/// - 倾向于生成较长的路径
/// - 不保证最短路径
/// - 适合探索迷宫的所有可能路径
/// </remarks>
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
        pathFound = false;
        
        stack.Push(startPos);
        inStack.Add(startPos);
        visited[startPos.x, startPos.y] = true;
        MarkCell(startPos, Color.yellow);

        while (stack.Count > 0)
        {
            currentExploring = stack.Peek();
            exploredCount++;
            
            if (currentExploring == endPos)
            {
                Debug.Log("Found end position!");
                ReconstructPath(parent);
                yield break;
            }

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
                    MarkCell(next, Color.yellow);
                    foundUnvisited = true;
                    break;
                }
            }

            if (!foundUnvisited)
            {
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
}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 广度优先搜索寻路算法类
/// 使用BFS策略在迷宫中寻找最短路径
/// </summary>
/// <remarks>
/// 算法流程：
/// 1. 层级遍历：
///    - 使用队列存储待访问单元格
///    - 按距离递增顺序访问
///    - 记录每个单元格的前驱节点
/// 2. 路径构建：
///    - 找到终点后回溯构建路径
///    - 使用前驱节点信息
///    - 反向生成最终路径
/// 
/// 特点：
/// - 保证找到最短路径
/// - 搜索范围呈圆形扩展
/// - 内存消耗较大但效率高
/// </remarks>
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

    /// <summary>
    /// 获前正在探索的位置
    /// </summary>
    /// <returns>当前探索位置的坐标</returns>
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

    /// <summary>
    /// 执行BFS寻路的每一步
    /// 包含可视化和移动效果
    /// </summary>
    public override IEnumerator<YieldInstruction> FindPathStepByStep()
    {
        queue.Clear();
        parent.Clear();
        exploredCount = 0;
        pathFound = false;
        
        // 重置访问数组
        for (int i = 0; i < maze.GetLength(0); i++)
            for (int j = 0; j < maze.GetLength(1); j++)
                visited[i, j] = false;
        
        queue.Enqueue(startPos);
        visited[startPos.x, startPos.y] = true;
        MarkCell(startPos, Color.yellow);
        currentExploring = startPos;

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            currentExploring = current;
            exploredCount++;
            
            if (current == endPos)
            {
                Debug.Log("Found end position!");
                ReconstructPath(parent);
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

            if (current != startPos && current != endPos)
            {
                MarkCell(current, Color.gray);
            }

            yield return new WaitForSeconds(0.05f);
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
}
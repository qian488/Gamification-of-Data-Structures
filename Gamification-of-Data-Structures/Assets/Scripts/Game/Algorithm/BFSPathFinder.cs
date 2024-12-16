using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 广度优先搜索寻路器
/// 实现迷宫中的广度优先搜索算法
/// </summary>
/// <remarks>
/// 主要特点：
/// 1. 按层级顺序探索迷宫
/// 2. 保证找到最短路径
/// 3. 提供实时的搜索过程可视化
/// 4. 支持平滑的路径移动
/// </remarks>
public class BFSPathFinder : PathFinder
{
    /// <summary>用于BFS的队列</summary>
    private Queue<Vector2Int> queue = new Queue<Vector2Int>();
    /// <summary>记录每个位置的父节点</summary>
    private Dictionary<Vector2Int, Vector2Int> parent = new Dictionary<Vector2Int, Vector2Int>();
    /// <summary>当前正在探索的位置</summary>
    private Vector2Int currentExploring;
    /// <summary>已探索的节点数量</summary>
    private int exploredCount = 0;
    /// <summary>用于平滑移动的路径队列</summary>
    private Queue<Vector2Int> movementPath = new Queue<Vector2Int>();

    public BFSPathFinder(MazeCell[,] maze) : base(maze)
    {
        currentExploring = startPos;
    }

    /// <summary>
    /// 获取当前正在探索的位置
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
        movementPath.Clear();
        
        // 获取玩家基础移动速度并翻倍
        float baseSpeed = PlayerManager.GetInstance().GetMoveSpeed();
        PlayerManager.GetInstance().SetMoveSpeed(baseSpeed * 2);
        
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
                    yield return new WaitForSeconds(0.05f);
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

            yield return new WaitForSeconds(0.05f);
        }

        // 搜索结束后恢复原速度
        PlayerManager.GetInstance().SetMoveSpeed(baseSpeed);
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
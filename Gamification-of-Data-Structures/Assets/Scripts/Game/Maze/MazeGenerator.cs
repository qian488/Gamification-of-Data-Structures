using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 迷宫生成器类
/// 使用深度优先搜索算法生成随机迷宫
/// </summary>
/// <remarks>
/// 主要功能：
/// 1. 随机生成迷宫布局
/// 2. 确保迷宫的连通性
/// 3. 设置起点和终点
/// 4. 处理迷宫边界
/// </remarks>
public class MazeGenerator
{
    /// <summary>迷宫数据数组</summary>
    private MazeCell[,] maze;
    /// <summary>迷宫宽度</summary>
    private int width;
    /// <summary>迷宫高度</summary>
    private int height;
    /// <summary>随机数生成器</summary>
    private System.Random random;

    /// <summary>可移动方向数组：上、右、下、左</summary>
    private readonly int[,] directions = new int[,] { { -1, 0 }, { 0, 1 }, { 1, 0 }, { 0, -1 } };

    // 定义迷宫生成算法类型
    private enum GenerationAlgorithm
    {
        DFS,
        Prim,
        Kruskal,
        RecursiveDivision
    }

    public MazeGenerator()
    {
        random = new System.Random();
    }

    /// <summary>
    /// 生成新的迷宫
    /// </summary>
    /// <param name="mazeData">迷宫数据数组</param>
    public void GenerateMaze(MazeCell[,] mazeData)
    {
        maze = mazeData;
        width = maze.GetLength(0);
        height = maze.GetLength(1);

        InitializeMaze();

        // 随机选择一种算法
        GenerationAlgorithm algorithm = (GenerationAlgorithm)random.Next(4);
        Debug.Log($"Using algorithm: {algorithm}");

        switch (algorithm)
        {
            case GenerationAlgorithm.DFS:
                CarvePassagesDFS(1, 1);
                break;
            case GenerationAlgorithm.Prim:
                PrimGeneration();
                break;
            case GenerationAlgorithm.Kruskal:
                KruskalGeneration();
                break;
            case GenerationAlgorithm.RecursiveDivision:
                // 先将所有格子设为通道
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        // 设置边界为墙
                        if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                        {
                            maze[x, y].IsWall = true;
                        }
                        else
                        {
                            maze[x, y].IsWall = false;
                        }
                    }
                }
                RecursiveDivision(1, 1, width - 2, height - 2);
                break;
        }

        SetBoundaries();
        SetStartAndEnd();
    }

    /// <summary>
    /// 初始化迷宫，将所有单元格设置为墙
    /// </summary>
    private void InitializeMaze()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                maze[x, y].IsWall = true;
                maze[x, y].IsVisited = false;
            }
        }
    }

    // DFS算法（原有的算法，改名以区分）
    private void CarvePassagesDFS(int x, int y)
    {
        maze[x, y].IsWall = false;
        maze[x, y].IsVisited = true;

        List<int> dirs = new List<int> { 0, 1, 2, 3 };
        for (int i = dirs.Count - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            int temp = dirs[i];
            dirs[i] = dirs[j];
            dirs[j] = temp;
        }

        foreach (int dir in dirs)
        {
            int nextX = x + directions[dir, 0] * 2;
            int nextY = y + directions[dir, 1] * 2;

            if (IsInBounds(nextX, nextY) && maze[nextX, nextY].IsWall)
            {
                maze[x + directions[dir, 0], y + directions[dir, 1]].IsWall = false;
                CarvePassagesDFS(nextX, nextY);
            }
        }
    }

    // Prim算法
    private void PrimGeneration()
    {
        List<Vector2Int> frontier = new List<Vector2Int>();
        Vector2Int start = new Vector2Int(1, 1);
        
        maze[start.x, start.y].IsWall = false;
        AddFrontier(start, frontier);

        while (frontier.Count > 0)
        {
            int index = random.Next(frontier.Count);
            Vector2Int current = frontier[index];
            frontier.RemoveAt(index);

            List<Vector2Int> neighbors = GetPassageNeighbors(current);
            if (neighbors.Count > 0)
            {
                Vector2Int neighbor = neighbors[random.Next(neighbors.Count)];
                ConnectCells(current, neighbor);
                maze[current.x, current.y].IsWall = false;
                AddFrontier(current, frontier);
            }
        }
    }

    // Kruskal算法
    private void KruskalGeneration()
    {
        Dictionary<Vector2Int, Vector2Int> parent = new Dictionary<Vector2Int, Vector2Int>();
        List<Edge> edges = new List<Edge>();

        // 初始化单元格和收集边
        for (int x = 1; x < width - 1; x += 2)
        {
            for (int y = 1; y < height - 1; y += 2)
            {
                Vector2Int current = new Vector2Int(x, y);
                parent[current] = current;
                maze[x, y].IsWall = false;

                if (x < width - 2)
                    edges.Add(new Edge(current, new Vector2Int(x + 2, y)));
                if (y < height - 2)
                    edges.Add(new Edge(current, new Vector2Int(x, y + 2)));
            }
        }

        // 随机打乱边
        for (int i = edges.Count - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            Edge temp = edges[i];
            edges[i] = edges[j];
            edges[j] = temp;
        }

        // 连接不在同一集合的边
        foreach (Edge edge in edges)
        {
            if (Find(parent, edge.from) != Find(parent, edge.to))
            {
                Union(parent, edge.from, edge.to);
                ConnectCells(edge.from, edge.to);
            }
        }
    }

    // 修改递归分割算法
    private void RecursiveDivision(int x1, int y1, int x2, int y2)
    {
        // 检查边界条件
        if (x1 >= x2 - 1 || y1 >= y2 - 1) return;

        // 计算当前区域的宽度和高度
        int areaWidth = x2 - x1;
        int areaHeight = y2 - y1;

        // 如果区域太小，不再分割
        if (areaWidth < 4 || areaHeight < 4) return;

        // 选择分割方向
        bool horizontalDivision = areaHeight > areaWidth;
        if (areaWidth == areaHeight)
            horizontalDivision = random.Next(2) == 0;

        if (horizontalDivision)
        {
            // 选择水平分割线（必须是偶数位置）
            int wallY = y1 + 2 + 2 * random.Next((areaHeight - 2) / 2);

            // 创建水平墙
            for (int x = x1; x <= x2; x++)
            {
                if (!IsNearStartOrEnd(x, wallY))
                {
                    maze[x, wallY].IsWall = true;
                }
            }

            // 在墙上开一个通道（必须在奇数位置）
            int passageX = x1 + 1 + 2 * random.Next((areaWidth + 1) / 2);
            if (!IsNearStartOrEnd(passageX, wallY))
            {
                maze[passageX, wallY].IsWall = false;
            }

            // 递归处理上下两个区域
            RecursiveDivision(x1, y1, x2, wallY - 1);
            RecursiveDivision(x1, wallY + 1, x2, y2);
        }
        else
        {
            // 选择垂直分割线（必须是偶数位置）
            int wallX = x1 + 2 + 2 * random.Next((areaWidth - 2) / 2);

            // 创建垂直墙
            for (int y = y1; y <= y2; y++)
            {
                if (!IsNearStartOrEnd(wallX, y))
                {
                    maze[wallX, y].IsWall = true;
                }
            }

            // 在墙上开一个通道（必须在奇数位置）
            int passageY = y1 + 1 + 2 * random.Next((areaHeight + 1) / 2);
            if (!IsNearStartOrEnd(wallX, passageY))
            {
                maze[wallX, passageY].IsWall = false;
            }

            // 递归处理左右两个区域
            RecursiveDivision(x1, y1, wallX - 1, y2);
            RecursiveDivision(wallX + 1, y1, x2, y2);
        }
    }

    private bool IsNearStart(int x, int y)
    {
        return x <= 2 && y <= 2;
    }

    private bool IsNearEnd(int x, int y)
    {
        return x >= width - 3 && y >= height - 3;
    }

    private bool IsNearStartOrEnd(int x, int y)
    {
        return IsNearStart(x, y) || IsNearEnd(x, y);
    }

    private void CreatePassagesInWalls(int x1, int y1, int x2, int y2, int splitX, int splitY)
    {
        List<int> horizontalPassages = new List<int>();
        List<int> verticalPassages = new List<int>();

        // 收集可能的通道位置，确保在边界内
        for (int x = x1 + 1; x < x2; x++)
        {
            if (IsInBounds(x, splitY) && !IsNearStartOrEnd(x, splitY))
                horizontalPassages.Add(x);
        }
        for (int y = y1 + 1; y < y2; y++)
        {
            if (IsInBounds(splitX, y) && !IsNearStartOrEnd(splitX, y))
                verticalPassages.Add(y);
        }

        // 开通道
        if (horizontalPassages.Count > 0)
        {
            int passage = horizontalPassages[random.Next(horizontalPassages.Count)];
            maze[passage, splitY].IsWall = false;
        }
        if (verticalPassages.Count > 0)
        {
            int passage = verticalPassages[random.Next(verticalPassages.Count)];
            maze[splitX, passage].IsWall = false;
        }

        // 随机额外开通道
        int extraPassages = random.Next(2);
        for (int i = 0; i < extraPassages; i++)
        {
            if (random.Next(2) == 0 && horizontalPassages.Count > 0)
            {
                int passage = horizontalPassages[random.Next(horizontalPassages.Count)];
                maze[passage, splitY].IsWall = false;
                horizontalPassages.Remove(passage);
            }
            else if (verticalPassages.Count > 0)
            {
                int passage = verticalPassages[random.Next(verticalPassages.Count)];
                maze[splitX, passage].IsWall = false;
                verticalPassages.Remove(passage);
            }
        }
    }

    // 辅助方法

    private void AddFrontier(Vector2Int cell, List<Vector2Int> frontier)
    {
        for (int i = 0; i < 4; i++)
        {
            int nx = cell.x + directions[i, 0] * 2;
            int ny = cell.y + directions[i, 1] * 2;
            Vector2Int next = new Vector2Int(nx, ny);

            if (IsInBounds(nx, ny) && maze[nx, ny].IsWall && !frontier.Contains(next))
            {
                frontier.Add(next);
            }
        }
    }

    private List<Vector2Int> GetPassageNeighbors(Vector2Int cell)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();
        for (int i = 0; i < 4; i++)
        {
            int nx = cell.x + directions[i, 0] * 2;
            int ny = cell.y + directions[i, 1] * 2;
            if (IsInBounds(nx, ny) && !maze[nx, ny].IsWall)
            {
                neighbors.Add(new Vector2Int(nx, ny));
            }
        }
        return neighbors;
    }

    private void ConnectCells(Vector2Int from, Vector2Int to)
    {
        int mx = (from.x + to.x) / 2;
        int my = (from.y + to.y) / 2;
        maze[mx, my].IsWall = false;
    }

    private Vector2Int Find(Dictionary<Vector2Int, Vector2Int> parent, Vector2Int cell)
    {
        if (parent[cell] != cell)
        {
            parent[cell] = Find(parent, parent[cell]);
        }
        return parent[cell];
    }

    private void Union(Dictionary<Vector2Int, Vector2Int> parent, Vector2Int a, Vector2Int b)
    {
        Vector2Int rootA = Find(parent, a);
        Vector2Int rootB = Find(parent, b);
        if (rootA != rootB)
        {
            parent[rootB] = rootA;
        }
    }

    private bool IsInBounds(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }

    private void SetBoundaries()
    {
        for (int x = 0; x < width; x++)
        {
            maze[x, 0].IsWall = true;
            maze[x, height - 1].IsWall = true;
        }
        for (int y = 0; y < height; y++)
        {
            maze[0, y].IsWall = true;
            maze[width - 1, y].IsWall = true;
        }

        maze[1, 1].IsWall = false;
        maze[1, 2].IsWall = false;
        maze[2, 1].IsWall = false;

        maze[width - 2, height - 2].IsWall = false;
        maze[width - 2, height - 3].IsWall = false;
        maze[width - 3, height - 2].IsWall = false;
    }

    private void SetStartAndEnd()
    {
        maze[1, 1].IsWall = false;
        maze[1, 2].IsWall = false;
        maze[width - 2, height - 2].IsWall = false;
        maze[width - 2, height - 3].IsWall = false;
        maze[width - 2, height - 2].IsEnd = true;
    }

    // 用于Kruskal算法的边结构
    private struct Edge
    {
        public Vector2Int from;
        public Vector2Int to;

        public Edge(Vector2Int from, Vector2Int to)
        {
            this.from = from;
            this.to = to;
        }
    }
}
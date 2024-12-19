using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 迷宫生成器Kruskal部分
/// 使用Kruskal最小生成树算法生成迷宫
/// </summary>
/// <remarks>
/// 算法流程：
/// 1. 初始化所有单元格为独立集合
/// 2. 收集所有可能的边
/// 3. 随机打乱边的顺序
/// 4. 依次处理每条边：
///    - 检查两端单元格是否连通
///    - 若不连通则合并集合并打通墙壁
/// 
/// 特点：
/// - 生成的迷宫较为均衡
/// - 分支较多
/// - 路径分布较为随机
/// </remarks>
public partial class MazeGenerator
{
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
    private void KruskalGeneration()
    {
        Dictionary<Vector2Int, Vector2Int> parent = new Dictionary<Vector2Int, Vector2Int>();
        List<Edge> edges = new List<Edge>();

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
}
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 迷宫生成器类
/// 负责使用多种算法生成随机迷宫
/// </summary>
/// <remarks>
/// 主要功能：
/// 1. 迷宫生成：
///    - DFS算法：深度优先搜索生成
///    - Prim算法：最小生成树法
///    - Kruskal算法：并查集生成
///    - 递归分割：空间递归划分
/// 2. 迷宫处理：
///    - SetBoundaries()：设置边界
///    - SetStartAndEnd()：设置起终点
///    - InitializeMaze()：初始化迷宫
/// 
/// 使用方式：
/// - 创建MazeGenerator实例
/// - 调用GenerateMaze()生成迷宫
/// - 随机选择一种生成算法
/// </remarks>
public partial class MazeGenerator
{
    private MazeCell[,] maze;
    private int width;
    private int height;
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

    private void ConnectCells(Vector2Int from, Vector2Int to)
    {
        int mx = (from.x + to.x) / 2;
        int my = (from.y + to.y) / 2;
        maze[mx, my].IsWall = false;
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

}
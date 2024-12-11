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

        CarvePassages(1, 1);

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

    /// <summary>
    /// 使用深度优先搜索算法开凿通道
    /// </summary>
    /// <param name="x">当前X坐标</param>
    /// <param name="y">当前Y坐标</param>
    private void CarvePassages(int x, int y)
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
                CarvePassages(nextX, nextY);
            }
        }
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

    private bool IsInBounds(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }
}
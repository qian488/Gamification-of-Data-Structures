using UnityEngine;
using System.Collections.Generic;

public partial class MazeGenerator
{
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
}
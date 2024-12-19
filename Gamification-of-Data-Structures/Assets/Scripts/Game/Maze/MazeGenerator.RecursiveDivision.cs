using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 迷宫生成器递归分割部分
/// 使用递归分割算法生成迷宫
/// </summary>
/// <remarks>
/// 算法流程：
/// 1. 从空房间开始
/// 2. 递归地添加墙壁分割空间：
///    - 随机选择分割方向
///    - 在分割线上开通道
///    - 递归处理子区域
/// 3. 保证通路：
///    - 检查是否有通道
///    - 保护起点和终点区域
/// 
/// 特点：
/// - 生成的迷宫较为规则
/// - 容易形成大的开放空间
/// - 墙壁分布均匀
/// </remarks>
public partial class MazeGenerator
{
    private void RecursiveDivision(int x1, int y1, int x2, int y2)
    {
        if (x1 >= x2 - 1 || y1 >= y2 - 1) return;

        int areaWidth = x2 - x1;
        int areaHeight = y2 - y1;

        if (areaWidth < 4 || areaHeight < 4) return;

        bool divideHorizontally = random.Next(2) == 0;
        
        if (areaWidth < areaHeight)
        {
            divideHorizontally = true;  
        }
        else if (areaHeight < areaWidth)
        {
            divideHorizontally = false;  
        }

        if (divideHorizontally)
        {
            // 选择水平墙的位置（必须是偶数位置）
            int wallY = y1 + 1 + 2 * random.Next((areaHeight - 1) / 2);
            
            // 创建水平墙
            bool hasPassage = false;  // 添加标记确保一定有通道
            for (int x = x1; x <= x2; x++)
            {
                if (!IsNearStartOrEnd(x, wallY))
                {
                    maze[x, wallY].IsWall = true;
                }
                else
                {
                    // 如果是起点或终点区域，确保这里是通道
                    maze[x, wallY].IsWall = false;
                    hasPassage = true;
                }
            }

            // 如果还没有通道，确保创建一个
            if (!hasPassage)
            {
                // 在墙上开一个通道（必须在奇数位置）
                int passageX;
                do
                {
                    passageX = x1 + 1 + 2 * random.Next((areaWidth + 1) / 2);
                } while (IsNearStartOrEnd(passageX, wallY));  // 避免与起点终点区域重叠
                
                maze[passageX, wallY].IsWall = false;
            }

            // 递归处理上下两个区域
            RecursiveDivision(x1, y1, x2, wallY - 1);
            RecursiveDivision(x1, wallY + 1, x2, y2);
        }
        else
        {
            // 选择垂直墙的位置（必须是偶数位置）
            int wallX = x1 + 1 + 2 * random.Next((areaWidth - 1) / 2);
            
            // 创建垂直墙
            bool hasPassage = false;
            for (int y = y1; y <= y2; y++)
            {
                if (!IsNearStartOrEnd(wallX, y))
                {
                    maze[wallX, y].IsWall = true;
                }
                else
                {
                    maze[wallX, y].IsWall = false;
                    hasPassage = true;
                }
            }

            // 如果还没有通道，确保创建一个
            if (!hasPassage)
            {
                int passageY;
                do
                {
                    passageY = y1 + 1 + 2 * random.Next((areaHeight + 1) / 2);
                } while (IsNearStartOrEnd(wallX, passageY));
                
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
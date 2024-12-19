using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 迷宫生成器DFS部分
/// 使用深度优先搜索算法生成迷宫
/// </summary>
/// <remarks>
/// 算法流程：
/// 1. 从起点开始递归访问
/// 2. 随机选择未访问的相邻单元格
/// 3. 打通当前单元格与选中单元格的墙
/// 4. 递归处理选中的单元格
/// 5. 回溯处理其他方向
/// 
/// 特点：
/// - 生成的迷宫趋向于产生长廊道
/// - 路径较为曲折
/// - 分支相对较少
/// </remarks>
public partial class MazeGenerator
{
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
}
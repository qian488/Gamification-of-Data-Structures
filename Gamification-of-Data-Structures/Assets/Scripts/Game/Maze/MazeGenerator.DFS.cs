using UnityEngine;
using System.Collections.Generic;

public partial class MazeGenerator
{
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
}
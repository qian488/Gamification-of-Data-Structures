using UnityEngine;
using System.Collections.Generic;

public partial class MazeGenerator
{
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
    
}
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class BFSPathFinder : PathFinder
{
    private Queue<Vector2Int> queue;
    private bool pathFound;

    public BFSPathFinder(MazeCell[,] maze) : base(maze)
    {
        queue = new Queue<Vector2Int>();
        pathFound = false;
    }

    public override IEnumerator FindPath()
    {
        // 播放寻路开始音效
        MusicManager.GetInstance().PlaySFX("path_search", false);
        
        queue.Enqueue(startPos);
        visited[startPos.x, startPos.y] = true;

        while (queue.Count > 0 && !pathFound)
        {
            Vector2Int current = queue.Dequeue();
            
            // 更新UI显示
            GameUIManager.GetInstance().UpdateVisualizerData(queue);
            
            // 标记当前访问的节点
            if (maze[current.x, current.y].CellObject != null)
            {
                maze[current.x, current.y].CellObject.GetComponent<Renderer>().material.color = Color.yellow;
            }

            if (current == endPos)
            {
                pathFound = true;
                break;
            }

            foreach (Vector2Int dir in directions)
            {
                Vector2Int next = new Vector2Int(current.x + dir.x, current.y + dir.y);
                if (IsValid(next))
                {
                    queue.Enqueue(next);
                    visited[next.x, next.y] = true;
                    parentMap[next] = current;
                }
            }

            yield return new WaitForSeconds(0.1f);
        }

        if (pathFound)
        {
            List<Vector2Int> path = ReconstructPath();
            foreach (Vector2Int pos in path)
            {
                if (maze[pos.x, pos.y].CellObject != null)
                {
                    maze[pos.x, pos.y].CellObject.GetComponent<Renderer>().material.color = Color.green;
                }
                yield return new WaitForSeconds(0.05f);
            }
        }
    }
} 
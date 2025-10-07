using UnityEngine;
using System.Collections.Generic;

public class PathFinder : MonoBehaviour
{
    public Graph_datastructure graph;

    public List<int> FindShortestPath(int start, int goal)
    {
        Queue<List<int>> queue = new Queue<List<int>>();
        HashSet<int> visited = new HashSet<int>();

        queue.Enqueue(new List<int> { start });
        visited.Add(start);

        while (queue.Count > 0)
        {
            List<int> path = queue.Dequeue();
            int current = path[path.Count - 1];

            if (current == goal) return path;

            foreach (var neighbor in graph.directionalAdjList[current].Values)
            {
                if (!visited.Contains(neighbor))
                {
                    visited.Add(neighbor);
                    List<int> newPath = new List<int>(path) { neighbor };
                    queue.Enqueue(newPath);
                }
            }
        }
        return null;
    }
}

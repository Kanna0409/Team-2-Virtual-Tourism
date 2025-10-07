using UnityEngine;
using System.Collections.Generic;

public class Graph_datastructure : MonoBehaviour
{
    public List<GameObject> spheres;
    public Dictionary<int, Dictionary<string, int>> directionalAdjList = new Dictionary<int, Dictionary<string, int>>();

    void Start()
    {
        foreach (GameObject sphere in spheres)
        {
            Node node = sphere.GetComponent<Node>();
            Dictionary<string, int> dirMap = new Dictionary<string, int>();

            foreach (DirectionalNeighbor neighbor in node.directionalNeighbors)
            {
                Node neighborNode = neighbor.neighborObject.GetComponent<Node>();
                if (neighborNode != null && !dirMap.ContainsKey(neighbor.direction.ToLower()))
                {
                    dirMap.Add(neighbor.direction.ToLower(), neighborNode.index);
                }
            }

            directionalAdjList.Add(node.index, dirMap);
        }

        PrintDirectionalGraph();
    }

    void PrintDirectionalGraph()
    {
        foreach (var kvp in directionalAdjList)
        {
            string output = $"Node {kvp.Key} connections: ";
            foreach (var dir in kvp.Value)
            {
                output += $"[{dir.Key} -> {dir.Value}] ";
            }
            Debug.Log(output);
        }
    }
}

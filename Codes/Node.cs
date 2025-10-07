using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class DirectionalNeighbor{
    public string direction; 
    public GameObject neighborObject;
    public GameObject arrowPrefab;
}

public class Node : MonoBehaviour{
    public int index;
    public string displayName;
    public List<DirectionalNeighbor> directionalNeighbors;
    public float captureYaw;

}

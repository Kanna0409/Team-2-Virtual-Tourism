using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
public class VRNavigator : MonoBehaviour
{
    public Graph_datastructure graph;
    public int currentIndex = 0;
    public Transform xrrig;
    public int currentDestination = -1;
    public PathFinder pathFinder;
    public Color pathArrowColor = Color.magenta;
    private Dictionary<int, float> photoYaws = new Dictionary<int, float>();
    private List<GameObject> highlightedArrows = new List<GameObject>();
    private float lastCameraYaw;
    public bool debugYaw = false;
    void Start()
    {
        SetActiveSphere(currentIndex);
    }
    public void MoveInDirection(string direction)
    {
        direction = direction.ToLower();

        if (graph.directionalAdjList[currentIndex].ContainsKey(direction))
        {
            int nextIndex = graph.directionalAdjList[currentIndex][direction];
            SetActiveSphere(nextIndex);

            if (currentDestination >= 0 && currentIndex != currentDestination)
            {
                List<int> path = pathFinder.FindShortestPath(currentIndex, currentDestination);
                HighlightPath(path);
            }
            else
            {
                ClearHighlightedArrows();
            }
        }
        else
        {
            Debug.LogWarning($"No neighbor in direction '{direction}' from sphere {currentIndex}");
        }
    }


    void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Arrow_Trigger trigger = hit.collider.GetComponent<Arrow_Trigger>();
                if (trigger != null && trigger.navigator != null)
                {
                    trigger.navigator.MoveInDirection(trigger.direction);
                }
            }
        }
    }

    public void SetDestination(int destinationIndex)
    {
        if (destinationIndex == currentIndex)
        {
            return;
        }

        currentDestination = destinationIndex;
        List<int> path = pathFinder.FindShortestPath(currentIndex, currentDestination);
        HighlightPath(path);
    }

    void RotateNextSphereAccordingToYaw(int nextIndex)
    {
        float currentPhotoYaw = photoYaws.ContainsKey(currentIndex) ? photoYaws[currentIndex] : 0f;
        float nextPhotoYaw = photoYaws.ContainsKey(nextIndex) ? photoYaws[nextIndex] : 0f;
        float cameraYaw = Camera.main.transform.eulerAngles.y;
        float cameraRelativeToCurrentPhoto = Mathf.DeltaAngle(currentPhotoYaw, cameraYaw);
        float yawDeltaBetweenPhotos = Mathf.DeltaAngle(currentPhotoYaw, nextPhotoYaw);
        float requiredSphereRotation = yawDeltaBetweenPhotos - cameraRelativeToCurrentPhoto;

        GameObject nextSphere = graph.spheres[nextIndex];
        nextSphere.transform.rotation = Quaternion.Euler(0f, requiredSphereRotation, 0f);
    }

    void SetActiveSphere(int index)
    {
        foreach (GameObject sphere in graph.spheres)
            sphere.SetActive(false);

        GameObject targetSphere = graph.spheres[index];
        targetSphere.SetActive(true);

        if (xrrig != null)
            xrrig.position = targetSphere.transform.position;

        currentIndex = index;
    }



    void HighlightPath(List<int> path)
    {
        ClearHighlightedArrows();

        if (path == null || path.Count < 2)
        {
            return;
        }

        for (int i = 0; i < path.Count - 1; i++)
        {
            Node node = graph.spheres[path[i]].GetComponent<Node>();
            int next = path[i + 1];

            foreach (var neighbor in node.directionalNeighbors)
            {
                Node neighborNode = neighbor.neighborObject.GetComponent<Node>();
                if (neighborNode != null && neighborNode.index == next)
                {
                    if (neighbor.arrowPrefab != null && neighbor.arrowPrefab.GetComponent<Renderer>() != null)
                    {
                        neighbor.arrowPrefab.GetComponent<Renderer>().material.color = pathArrowColor;
                        highlightedArrows.Add(neighbor.arrowPrefab);
                    }
                    break;
                }
            }
        }
    }

    float NormalizeAngle(float a)
    {
        a %= 360f;
        if (a < 0f) a += 360f;
        return a;
    }

    void ClearHighlightedArrows()
    {
        foreach (var arrow in highlightedArrows)
        {
            if (arrow != null && arrow.GetComponent<Renderer>() != null)
                arrow.GetComponent<Renderer>().material.color = Color.white;
        }
        highlightedArrows.Clear();
    }
}


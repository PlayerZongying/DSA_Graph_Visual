using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintManager : MonoBehaviour
{
    public enum PaintMode
    {
        Walls,
        StartEnd
    }

    public PaintMode paintMode;

    private Camera _camera;

    private int nodeLayerMask = 1 << 6;

    // Start is called before the first frame update
    void Start()
    {
        _camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        Paint();
    }

    void Paint()
    {
        switch (paintMode)
        {
            case PaintMode.Walls:
                if (Input.GetMouseButton(0))
                {
                    Node node = DetecteNode();
                    node?.SetNodeWalkable(false);
                }

                else if (Input.GetMouseButton(1))
                {
                    Node node = DetecteNode();
                    node?.SetNodeWalkable(true);
                }
                break;
            case PaintMode.StartEnd:
                if (Input.GetMouseButton(0))
                {
                    Node node = DetecteNode();
                    node?.SetStartNode();
                    // if (node)
                    // {
                    //     ClearVisited(GraphManager.Instance.graph);
                    //     List<Node> neighbors = GraphManager.Instance.GetNeighbors(node);
                    //     foreach (var neighbor in neighbors)
                    //     {
                    //         neighbor.SetVisited();
                    //     }
                    // }
                }

                else if (Input.GetMouseButton(1))
                {
                    Node node = DetecteNode();
                    node?.SetEndNode();
                }
                break;
        }
    }

    Node DetecteNode()
    {
        RaycastHit hit;
        Node node = null;

        Vector3 mousePosition = Input.mousePosition;
        Ray ray = _camera.ScreenPointToRay(mousePosition);

        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, nodeLayerMask))
        {
            // Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.yellow);
            node = hit.transform.GetComponent<Node>();
        }

        return node;
    }

    void ClearVisited(Node[,] nodes)
    {
        foreach (var node in nodes)
        {
            if (node.walkable)
            {
                node.ResetVisited();
            }
        }
    }


    void DetectAndSetNodeWalkable(bool walkable)
    {
        RaycastHit hit;

        Vector3 mousePosition = Input.mousePosition;
        Ray ray = _camera.ScreenPointToRay(mousePosition);

        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, nodeLayerMask))
        {
            // Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.yellow);
            Node node = hit.transform.GetComponent<Node>();
            Debug.Log($"Did Hit ({node.row}, {node.col})");
            node.SetNodeWalkable(walkable);
        }
        else
        {
            // Debug.DrawRay(ray.origin, ray.direction * 1000, Color.white);
            // Debug.Log("Did not Hit");
        }
    }
}
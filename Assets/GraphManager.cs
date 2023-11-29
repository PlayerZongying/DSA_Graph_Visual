using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphManager : MonoBehaviour
{
    public static GraphManager Instance;
    private Camera _camera;

    public GameObject nodePrefab;
    public int rows;
    public int cols;

    public Node[,] graph;

    public Node startNode = null;
    public Node endNode = null;

    private bool reachEnd = false;

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _camera = Camera.main;
        _camera.transform.position += new Vector3((float)(cols - 1) / 2, - (float)(rows - 1) / 2, 0);
        CreateGraph();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (startNode && endNode)
            {
                List<Node> path = DFS();
                // foreach (var node in path)
                // {
                //     print($"({node.col}, {node.row})");
                // }

                for (int i = 1; i < path.Count - 1; i++)
                {
                    Node node = path[i];
                    Color color = Color.HSVToRGB((float) i/(path.Count - 1), 0.5f, 1);
                    
                    node.SetColor(color);
                    print($"({node.col}, {node.row})");
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            ResetVisitedStatus();
        }
    }

    void CreateGraph()
    {
        graph = new Node[rows, cols];
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                // Instantiate a cube (node) at the desired position
                GameObject nodeObject = Instantiate(nodePrefab, new Vector3(j, -i, 0), Quaternion.identity);
                nodeObject.transform.SetParent(transform);

                // Get the Node component and set its row and column properties
                Node nodeComponent = nodeObject.GetComponent<Node>();
                nodeComponent.row = i;
                nodeComponent.col = j;

                // Assign the node to the grid
                graph[i, j] = nodeComponent;
            }
        }
    }
    void ResetVisitedStatus()
    {
        foreach (var node in graph)
        {
            if (node.walkable)
            {
                node.ResetVisited();
            }
        }
    }
    public List<Node> DFS()
    {
        ResetVisitedStatus();
        reachEnd = false;
        List<Node> path = new List<Node>();
        DFSRecursive(startNode, path);

        return path;
    }
    private void DFSRecursive(Node currentNode, List<Node> path)
    {
        if(reachEnd) return;
        currentNode.SetVisited();
        path.Add(currentNode);

        if (currentNode == endNode)
        {
            reachEnd = true;
            return; // End reached
        }

        foreach (Node neighbor in GetNeighbors(currentNode))
        {
            if (!neighbor.visited && neighbor.walkable)
            {
                DFSRecursive(neighbor, path);
            }
        }
    }

    public List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();

        if(node.row > 0 && graph[node.row - 1, node.col].walkable) neighbors.Add(graph[node.row - 1, node.col]);
        if(node.row < rows - 1 && graph[node.row + 1, node.col].walkable) neighbors.Add(graph[node.row + 1, node.col]);
        if(node.col > 0 && graph[node.row, node.col - 1].walkable) neighbors.Add(graph[node.row, node.col - 1]);
        if(node.col < cols - 1 && graph[node.row, node.col + 1].walkable) neighbors.Add(graph[node.row, node.col + 1]);
        
        return neighbors;
    }
    
    

    
}
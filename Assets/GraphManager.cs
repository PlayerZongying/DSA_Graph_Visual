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
        if (Input.GetKeyDown(KeyCode.D))
        {
            ResetVisitedStatus();
            if (startNode && endNode)
            {
                List<Node> path = DFS();
                path = ReconstructPath(startNode, endNode);
                for (int i = 1; i < path.Count - 1; i++)
                {
                    Node node = path[i];
                    Color color = Color.HSVToRGB((float) i/(path.Count - 1), 0.5f, 1);
                    
                    node.SetColor(color);
                    // print($"({node.col}, {node.row})");
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            ResetVisitedStatus();
            if (startNode && endNode)
            {
                List<Node> path = BFS();
                path = ReconstructPath(startNode, endNode);
                for (int i = 1; i < path.Count - 1; i++)
                {
                    Node node = path[i];
                    Color color = Color.HSVToRGB((float) i /(path.Count - 1), 0.5f, 1);
                    
                    node.SetColor(color);
                    // print($"({node.col}, {node.row})");
                }
            }
        }
        
        if (Input.GetKeyDown(KeyCode.A))
        {
            ResetVisitedStatus();
            if (startNode && endNode)
            {
                List<Node> path = AStar(startNode, endNode);

                for (int i = 1; i < path.Count - 1; i++)
                {
                    Node node = path[i];
                    Color color = Color.HSVToRGB((float) i /(path.Count - 1), 0.5f, 1);
                    
                    node.SetColor(color);
                    // print($"({node.col}, {node.row})");
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
            node.parent = null;
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
                neighbor.parent = currentNode;
                DFSRecursive(neighbor, path);
            }
        }
    }
    
    public List<Node> BFS()
    {
        ResetVisitedStatus();

        List<Node> path = new List<Node>();
        Queue<Node> queue = new Queue<Node>();

        queue.Enqueue(startNode);
        startNode.visited = true;

        while (queue.Count > 0)
        {
            Node currentNode = queue.Dequeue();
            path.Add(currentNode);

            if (currentNode == endNode)
            {
                break; // End reached
            }

            foreach (Node neighbor in GetNeighbors(currentNode))
            {
                if (!neighbor.visited && neighbor.walkable)
                {
                    neighbor.parent = currentNode;
                    queue.Enqueue(neighbor);
                    neighbor.SetVisited();
                }
            }
        }

        return path;
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
    
    public List<Node> AStar(Node startNode, Node endNode)
    {
        List<Node> openList = new List<Node>();
        List<Node> closedList = new List<Node>();

        openList.Add(startNode);

        while (openList.Count > 0)
        {
            Node current = GetLowestF(openList);
            current.SetVisited();

            openList.Remove(current);
            closedList.Add(current);

            if (current == endNode)
            {
                // Path found, reconstruct and return it
                return ReconstructPath(startNode, endNode);
            }

            foreach (Node neighbor in GetNeighbors(current))
            {
                if (!neighbor.walkable || closedList.Contains(neighbor))
                {
                    continue;
                }

                float tentativeG = current.g + CalculateDistance(current, neighbor);

                if (!openList.Contains(neighbor) || tentativeG < neighbor.g)
                {
                    neighbor.g = tentativeG;
                    neighbor.h = CalculateDistance(neighbor, endNode);
                    neighbor.parent = current;

                    if (!openList.Contains(neighbor))
                    {
                        openList.Add(neighbor);
                    }
                }
            }
        }
        // No path found
        return null;
    }

    private float CalculateDistance(Node a, Node b)
    {
        float dist = new Vector2(a.row - b.row, a.col - b.col).magnitude;
        // Implement your distance calculation (e.g., Euclidean, Manhattan, etc.)
        // This depends on your specific grid layout and requirements.
        return dist;
    }

    private Node GetLowestF(List<Node> nodes)
    {
        Node lowest = nodes[0];
        foreach (Node node in nodes)
        {
            if ((node.g + node.h) < (lowest.g + lowest.h))
            {
                lowest = node;
            }
        }
        return lowest;
    }

    private List<Node> ReconstructPath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node current = endNode;

        while (current != startNode)
        {
            path.Insert(0, current); // Insert at the beginning of the list
            current = current.parent;
        }

        path.Insert(0, startNode);

        return path;
    }
    
    

    
}
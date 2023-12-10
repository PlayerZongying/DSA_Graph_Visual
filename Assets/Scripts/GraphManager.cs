using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GraphManager : MonoBehaviour
{
    public static GraphManager Instance;
    protected Camera _camera;

    public bool enableTextInput = false;
    protected string txtFilePath = "Assets/AssignmentNodes.txt";
    protected string[] txtLines =
    {
        "oooooooXooooooXooooo",
        "ooooSoooooooooXooooo",
        "oooooooXoooooooooooo",
        "XXXXXXXXooooooXooooo",
        "oooooooXXXXXXXXooooo",
        "oooooooXooooooXXXoXX",
        "ooooooooooooooXooooo",
        "oooooooXooooooXooooo",
        "oooooooXoooooooooooo",
        "oooooooXXXXXXXXooooo",
        "oooooooXooooooXooooo",
        "ooooooooooooooXooooo",
        "oooooooXoooooooooooo",
        "XXoXXXXXooooooXooooo",
        "oooooooXXXXXXXXooooo",
        "oooooooXooooooXXXXXX",
        "ooooooooooooooXooooo",
        "oooooooXooooooXooGoo",
        "oooooooXoooooooooooo",
        "oooooooXooooooXooooo",
    };

    public GameObject nodePrefab;
    public int rows;
    public int cols;

    public Node[,] graph;

    public Node startNode = null;
    public Node endNode = null;

    protected bool reachEnd = false;

    public bool useOldAstar = false;

    protected virtual void Awake()
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
    protected virtual void Start()
    {
        if (enableTextInput)
        {
            // txtLines = File.ReadAllLines(txtFilePath);
            // foreach (string line in lines)
            // {
            //     Debug.Log(line);
            // }

            rows = txtLines.Length;
            cols = txtLines[0].Length;
        }

        _camera = Camera.main;

        if (_camera.orthographicSize < (float)rows / 2) _camera.orthographicSize = (float)rows / 2;

        _camera.transform.position += new Vector3((float)(cols - 1) / 2, -(float)(rows - 1) / 2, 0);
        CreateGraph();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            ResetVisitedStatus();
            if (startNode && endNode)
            {
                float startTime = Time.realtimeSinceStartup;
                List<Node> path = DFS();
                float endTime = Time.realtimeSinceStartup;
                float timeInMs = (endTime - startTime) * 1000;
                UIManager.Instance.TimeDisplay($"{timeInMs:0.00} ms");

                path = ReconstructPath(startNode, endNode);
                DrawPath(path);
            }
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            ResetVisitedStatus();
            if (startNode && endNode)
            {
                float startTime = Time.realtimeSinceStartup;
                List<Node> path = BFS();
                float endTime = Time.realtimeSinceStartup;
                float timeInMs = (endTime - startTime) * 1000;
                UIManager.Instance.TimeDisplay($"{timeInMs:0.00} ms");

                path = ReconstructPath(startNode, endNode);
                DrawPath(path);
            }
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            ResetVisitedStatus();
            if (startNode && endNode)
            {
                float startTime = Time.realtimeSinceStartup;
                List<Node> path;
                
                if (useOldAstar) path = OldAStar(startNode, endNode);
                else path = AStar(startNode, endNode);

                float endTime = Time.realtimeSinceStartup;
                float timeInMs = (endTime - startTime) * 1000;
                UIManager.Instance.TimeDisplay($"{timeInMs:0.00} ms");

                DrawPath(path);
            }
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            ResetVisitedStatus();
        }
    }

    protected virtual void CreateGraph()
    {
        // generate all nodes
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

        // paint if txt input is enabled
        if (enableTextInput)
        {
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (txtLines[i][j] == 'X')
                    {
                        graph[i, j].SetNodeWalkable(false);
                    }
                    else if (txtLines[i][j] == 'S')
                    {
                        graph[i, j].SetStartNode();
                    }
                    else if (txtLines[i][j] == 'G')
                    {
                        graph[i, j].SetEndNode();
                    }
                }
            }
        }
    }

    protected virtual void ResetVisitedStatus()
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

    public void DrawPath(List<Node> path)
    {
        for (int i = 0; i < path.Count; i++)
        {
            Node node = path[i];
            if (node == startNode || node == endNode) continue;

            Color color;
            // color = Color.HSVToRGB((float) i /(path.Count - 1), 0.5f, 1);
            color = Color.Lerp(node.startColor, node.endColor, (float)i / (path.Count - 1));
            node.SetColor(color);
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
        if (reachEnd) return;
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

        if (node.row > 0 && graph[node.row - 1, node.col].walkable) neighbors.Add(graph[node.row - 1, node.col]);
        if (node.row < rows - 1 && graph[node.row + 1, node.col].walkable) neighbors.Add(graph[node.row + 1, node.col]);
        if (node.col > 0 && graph[node.row, node.col - 1].walkable) neighbors.Add(graph[node.row, node.col - 1]);
        if (node.col < cols - 1 && graph[node.row, node.col + 1].walkable) neighbors.Add(graph[node.row, node.col + 1]);

        return neighbors;
    }

    public List<Node> AStar(Node startNode, Node endNode)
    {
        List<Node> openList = new List<Node>();

        openList.Add(startNode);

        while (openList.Count > 0)
        {
            Node current = openList[0];
            current.SetVisited();

            openList.Remove(current);

            if (current == endNode)
            {
                // Path found, reconstruct and return it
                return ReconstructPath(startNode, endNode);
            }

            foreach (Node neighbor in GetNeighbors(current))
            {
                if (!neighbor.walkable || neighbor.visited)
                {
                    continue;
                }

                //hack, for showing the visited neighbors
                if (neighbor != endNode) neighbor.SetColor(neighbor.visitedColor);

                float tentativeG = current.g + CalculateDistance(current, neighbor);

                if (!openList.Contains(neighbor) || tentativeG < neighbor.g)
                {
                    neighbor.g = tentativeG;
                    neighbor.h = CalculateDistance(neighbor, endNode);
                    neighbor.parent = current;

                    if (!openList.Contains(neighbor))
                    {
                        SearchInsert(openList, neighbor);
                    }
                }
            }
        }
        // No path found
        return null;
    }

    private List<Node> OldAStar(Node startNode, Node endNode)
    {
        List<Node> openList = new List<Node>();
        List<Node> closeList = new List<Node>();

        openList.Add(startNode);

        while (openList.Count > 0)
        {
            Node current = GetLowestF(openList);
            current.SetVisited();
            closeList.Add(current);
            openList.Remove(current);

            if (current == endNode)
            {
                // Path found, reconstruct and return it
                return ReconstructPath(startNode, endNode);
            }

            foreach (Node neighbor in GetNeighbors(current))
            {
                if (!neighbor.walkable || closeList.Contains(neighbor))
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
        return dist;
    }

    private void SearchInsert(List<Node> nodes, Node node)
    {
        if (nodes.Count == 0)
        {
            nodes.Add(node);
            return;
        }

        int low = 0;
        int high = nodes.Count - 1;
        while (low <= high)
        {
            int mid = (low + high) / 2;

            if (nodes[mid].h + nodes[mid].g > node.h + node.g)
            {
                high = mid - 1;
            }
            else
            {
                low = mid + 1;
            }
        }

        nodes.Insert(low, node);
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
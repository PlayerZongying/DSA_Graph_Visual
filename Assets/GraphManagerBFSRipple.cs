using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphManagerBFSRipple : GraphManager
{
    public static GraphManagerBFSRipple BFSRippleInstance;
    private NodeBFSRipple[,] graph;
    [Range(0.01f, 1f)] public float timeStep = 0.01f;
    // Start is called before the first frame update

    protected override void Awake()
    {
        base.Awake();
        if (!BFSRippleInstance)
        {
            BFSRippleInstance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    void Start()
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
    void Update()
    {
    }

    protected override void CreateGraph()
    {
        graph = new NodeBFSRipple[rows, cols];
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                // Instantiate a cube (node) at the desired position
                GameObject nodeObject = Instantiate(nodePrefab, new Vector3(j, -i, 0), Quaternion.identity);
                nodeObject.transform.SetParent(transform);

                // Get the Node component and set its row and column properties
                NodeBFSRipple nodeComponent = nodeObject.GetComponent<NodeBFSRipple>();
                nodeComponent.row = i;
                nodeComponent.col = j;

                // Assign the node to the grid
                graph[i, j] = nodeComponent;
            }
        }
    }

    public IEnumerator BFSFromNode(NodeBFSRipple node)
    {
        ResetVisitedStatus();
        Queue<NodeBFSRipple> queue = new Queue<NodeBFSRipple>();

        queue.Enqueue(node);
        node.visited = true;

        while (queue.Count > 0)
        {
            int count = queue.Count;
            List<NodeBFSRipple> layer = new List<NodeBFSRipple>();
            for (int i = 0; i < count; i++)
            {
                NodeBFSRipple currentNode = queue.Dequeue();
                layer.Add(currentNode);

                foreach (NodeBFSRipple neighbor in GetNeighbors(currentNode))
                {
                    if (!neighbor.visited && neighbor.walkable)
                    {
                        neighbor.parent = currentNode;
                        queue.Enqueue(neighbor);
                        neighbor.visited = true;
                    }
                }
            }

            foreach (var nodeInLayer in layer)
            {
                StartCoroutine(nodeInLayer.Change());
            }

            yield return new WaitForSeconds(timeStep);
        }
    }

    public List<NodeBFSRipple> GetNeighbors(NodeBFSRipple node)
    {
        List<NodeBFSRipple> neighbors = new List<NodeBFSRipple>();

        if (node.row > 0 && graph[node.row - 1, node.col].walkable) neighbors.Add(graph[node.row - 1, node.col]);
        if (node.row < rows - 1 && graph[node.row + 1, node.col].walkable) neighbors.Add(graph[node.row + 1, node.col]);
        if (node.col > 0 && graph[node.row, node.col - 1].walkable) neighbors.Add(graph[node.row, node.col - 1]);
        if (node.col < cols - 1 && graph[node.row, node.col + 1].walkable) neighbors.Add(graph[node.row, node.col + 1]);

        return neighbors;
    }

    protected override void ResetVisitedStatus()
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
}
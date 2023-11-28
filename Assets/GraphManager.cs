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

    private Node[,] graph;

    public Node startNode = null;
    public Node endNode = null;

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

    
}
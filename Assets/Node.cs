using UnityEngine;
using UnityEngine.Timeline;

public class Node : MonoBehaviour
{
    public int row;

    public int col;

    public bool walkable = true;

    public bool visited = false;

    // for generating path
    [HideInInspector] public Node parent = null;

    // for a* star
    [HideInInspector] public float g = 0;
    [HideInInspector] public float h = 0;

    public Color walkableColor;
    public Color unwalkableColor;
    public Color visitedColor;
    public Color startColor;
    public Color endColor;
    
    private Material nodeMaterial;
    public Renderer _renderer;

    private MeshRenderer _meshRenderer;

    // Start is called before the first frame update
    void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _renderer = GetComponent<Renderer>();
        SetNodeWalkable(walkable);
    }

    public void SetNodeWalkable(bool walkable)
    {
        if (this == GraphManager.Instance.startNode || this == GraphManager.Instance.endNode) return;
        this.walkable = walkable;
        if (this.walkable)
        {
            _renderer.material.SetColor("_BaseColor", walkableColor);
        }
        else
        {
            _renderer.material.SetColor("_BaseColor", unwalkableColor);
        }
        // _meshRenderer.enabled = walkable;
    }


    public void SetStartNode()
    {
        if (!walkable) return;
        Node prevStart = GraphManager.Instance.startNode;
        prevStart?._renderer.material.SetColor("_BaseColor", walkableColor);

        GraphManager.Instance.startNode = this;
        _renderer.material.SetColor("_BaseColor", startColor);
    }

    public void SetEndNode()
    {
        if (!walkable) return;
        Node prevEnd = GraphManager.Instance.endNode;
        prevEnd?._renderer.material.SetColor("_BaseColor", walkableColor);

        GraphManager.Instance.endNode = this;
        _renderer.material.SetColor("_BaseColor", endColor);
    }

    public void SetVisited()
    {
        visited = true;
        if (this == GraphManager.Instance.startNode || this == GraphManager.Instance.endNode) return;
        _renderer.material.SetColor("_BaseColor", visitedColor);
    }

    public void ResetVisited()
    {
        visited = false;
        if (this == GraphManager.Instance.startNode || this == GraphManager.Instance.endNode) return;
        _renderer.material.SetColor("_BaseColor", walkableColor);
    }

    public void SetColor(Color color)
    {
        _renderer.material.SetColor("_BaseColor", color);
    }

    // Update is called once per frame
    void Update()
    {
    }
}
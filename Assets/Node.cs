using UnityEngine;

public class Node : MonoBehaviour
{
    public int row;

    public int col;

    public bool walkable = true;
    public bool visited = false;

    public Color walkableColor;
    public Color unwalkableColor;
    public Color visitedColor;
    public Color startColor;
    public Color endColor;

    public Material nodeMat;
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
        if(!walkable) return;
        Node prevStart = GraphManager.Instance.startNode;
        prevStart?._renderer.material.SetColor("_BaseColor", walkableColor);

        GraphManager.Instance.startNode = this;
        _renderer.material.SetColor("_BaseColor", startColor);
    }
    
    public void SetEndNode()
    {
        if(!walkable) return;
        Node prevEnd = GraphManager.Instance.endNode;
        prevEnd?._renderer.material.SetColor("_BaseColor", walkableColor);

        GraphManager.Instance.endNode = this;
        _renderer.material.SetColor("_BaseColor", endColor);
    }

    public void SetVisited()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }
}
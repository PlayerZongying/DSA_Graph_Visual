using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintManagerBFSRipple : MonoBehaviour
{
        public enum PaintMode
    {
        Walls,
        Ripple
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
        SwitchPaintMode();
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
            case PaintMode.Ripple:
                if (Input.GetMouseButtonDown(0))
                {
                    NodeBFSRipple node = DetecteNode();
                    if(node) StartCoroutine(GraphManagerBFSRipple.BFSRippleInstance.BFSFromNode(node));;
                }
                break;
        }
    }

    void SwitchPaintMode()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            int cur = (int)paintMode;
            cur++;
            cur %= 2;
            paintMode = (PaintMode)cur;
        }
    }

    NodeBFSRipple DetecteNode()
    {
        RaycastHit hit;
        NodeBFSRipple node = null;

        Vector3 mousePosition = Input.mousePosition;
        Ray ray = _camera.ScreenPointToRay(mousePosition);

        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, nodeLayerMask))
        {
            // Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.yellow);
            node = hit.transform.GetComponent<NodeBFSRipple>();
        }

        return node;
    }
}

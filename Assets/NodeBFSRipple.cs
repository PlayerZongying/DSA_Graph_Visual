using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeBFSRipple : Node
{
    public Gradient changeGradient;
    public float leftActiveTime = 0;
    public float activeTIme = 5;
    public float frequency = 5;
    
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override IEnumerator Change()
    {
        leftActiveTime = activeTIme;
        while (leftActiveTime - Time.deltaTime > 0)
        {
            leftActiveTime -= Time.deltaTime;
            float t = Mathf.Sin((leftActiveTime / activeTIme) * frequency * Mathf.PI) * (leftActiveTime / activeTIme) * 0.5f + 0.5f ;
            
            Color color = changeGradient.Evaluate(t);
            SetColor(color);
            yield return new WaitForEndOfFrame();
        }
    }
}

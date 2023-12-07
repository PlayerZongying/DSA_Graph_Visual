using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public TextMeshProUGUI timeTMP;
    public float fadingTime;
    // Start is called before the first frame update

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

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TimeDisplay(string timeString)
    {
        StartCoroutine(TimeDisplayFadeOut(timeString));
    }

    IEnumerator TimeDisplayFadeOut(string timeString)
    {
        Color textColor = timeTMP.color;
        textColor.a = 1;
        timeTMP.color = textColor;
        timeTMP.text = timeString;

        while (fadingTime - Time.deltaTime > 0)
        {
            textColor.a -= Time.deltaTime / fadingTime;
            timeTMP.color = textColor;
            yield return new WaitForEndOfFrame();
        }
        
    }
}

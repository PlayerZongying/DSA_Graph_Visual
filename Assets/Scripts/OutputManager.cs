using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class OutputManager : MonoBehaviour
{
    public GraphManagerBFSRipple graphManager; 
    private string folderPath = System.IO.Directory.GetCurrentDirectory() + "/Savings/";
    // Start is called before the first frame update
    void Start()
    {
        graphManager = GraphManagerBFSRipple.BFSRippleInstance;
    }

    void WriteDownCurrentGraph()
    {
        Node[,] graph = graphManager.graph;
        List<string> results = new List<string>();
        for (int i = 0; i < graphManager.rows; i++)
        {
            string newLine = "";
            for (int j = 0; j < graphManager.cols; j++)
            {
                char letter = 'o';
                
                if (!graph[i, j].walkable)
                {
                    letter = 'X';
                }
                else if(graph[i, j] == graphManager.startNode)
                {
                    letter = 'S';
                }
                
                else if(graph[i, j] == graphManager.endNode)
                {
                    letter = 'G';
                }

                newLine += letter;
            }
            results.Add(newLine);
        }

        string fileName = DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".txt";;
        string filePath = folderPath + fileName;
        
        try
        {
            // Open a stream writer
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                // Write each string in the list to the file
                foreach (string line in results)
                {
                    writer.WriteLine(line);
                }
            }
            
            print($"{fileName} Saved!");
        }
        catch (Exception e)
        {
            Debug.LogError("Error writing to file: " + e.Message);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            WriteDownCurrentGraph();
        }
    }
}

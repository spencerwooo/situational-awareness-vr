using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using UnityEngine;

public class VisualiseCoords : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod]
    private static void OnRuntimeMethodLoad()
    {
        var data = CsvReader.Read("InteractionLogs-211020-125559");

        for (var i = 0; i < data.Count; i++)
        {
            Debug.Log(data[i].ToString());
            if (i > 10) break;
        }
        
        Debug.Log("Loaded CSV.");
    }
}
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using UnityEngine;
// using CsvHelper;

public class VisualiseCoords : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod]
    private static void OnRuntimeMethodLoad()
    {
        // StreamReader reader = new StreamReader("DataAnalysis/InteractionLogs-211020-125559.csv");
        // CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        //
        // CsvDataReader dr = new CsvDataReader(csv);
        // DataTable dt = new DataTable();
        // dt.Load(dr);
        
        Debug.Log("Loaded CSV.");
    }
}
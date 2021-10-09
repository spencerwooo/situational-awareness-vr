using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractionLogger : MonoBehaviour
{
    public InteractionData interactionData;
    public Button button;

    private bool _loggingEnabled = false;
    private List<string> _loggingData = new List<string>();

    private void Start()
    {
        button.onClick.AddListener(() =>
        {
            if (_loggingEnabled) StopInteractionLogging();
            else StartInteractionLogging();
        });
    }

    private void Update()
    {
        if (!_loggingEnabled) return;

        _loggingData.Add(
            $"{FormatVector3(interactionData.userPosition, log: true)},{FormatVector3(interactionData.userOrientation, log: true)}," +
            $"{interactionData.cameraHitObjectName},{FormatVector3(interactionData.cameraHitPoint, log: true)}," +
            $"{interactionData.cameraHitPointDistance:0.00}," +
            $"{interactionData.controllerHitObjectName},{FormatVector3(interactionData.controllerHitPoint, log: true)}," +
            $"{interactionData.controllerHitPointDistance:0.00}");
    }

    private void StartInteractionLogging()
    {
        Debug.Log("Started user interaction logging.");
        _loggingEnabled = true;

        button.GetComponentInChildren<TextMeshProUGUI>().text = "Logging user interactions...";
        // button.GetComponentInChildren<Image>().color = new Color(80, 197, 68);
    }

    private void StopInteractionLogging()
    {
        Debug.Log("Stopped user interaction logging.");
        _loggingEnabled = false;

        // using a coroutine here as saving to disk requires time so as not to block the main update
        StartCoroutine(LogDataToFile());

        button.GetComponentInChildren<TextMeshProUGUI>().text = "Start logging user interactions";
        // button.GetComponentInChildren<Image>().color = new Color(197, 76, 68);
    }

    private IEnumerator LogDataToFile()
    {
        StreamWriter writer = new StreamWriter("Logs/InteractionLogs.csv");
        writer.WriteLine("user_position,user_orientation,camera_hit_obj,camera_hit_point,camera_hit_dist," +
                         "controller_hit_obj,controller_hit_point,controller_hit_distance");

        foreach (string log in _loggingData)
        {
            writer.WriteLine(log);
        }

        writer.Close();
        yield return true;
    }

    protected internal static string FormatVector3(Vector3 vec, bool log = false)
    {
        // format a Vector3 <a, b, c> to string (a, b, c)
        if (log) return $"\"({vec.x:0.00},{vec.y:0.00},{vec.z:0.00})\"";
        return $"({vec.x:0.00}, {vec.y:0.00}, {vec.z:0.00})";
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractionLogger : MonoBehaviour
{
    public InteractionData interactionData;
    public Button button;

    private bool _loggingEnabled = false;
    private readonly List<string> _loggingData = new List<string>();

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
            $"{FormatVector3(interactionData.userPosition)},{FormatVector3(interactionData.userOrientation)}," +
            $"{FormatVector3(interactionData.cameraHitPoint)}");
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

        foreach (string data in _loggingData)
        {
            Debug.Log(data);
        }

        button.GetComponentInChildren<TextMeshProUGUI>().text = "Start logging user interactions";
        // button.GetComponentInChildren<Image>().color = new Color(197, 76, 68);
    }


    protected internal static string FormatVector3(Vector3 vec)
    {
        // format a Vector3 <a, b, c> to string [a, b, c]
        return $"[{vec.x:0.00}, {vec.y:0.00}, {vec.z:0.00}]";
    }
}
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

    [SerializeField] private int loggingFrequency = 10;
    private bool _loggingEnabled = false;
    private int _framesElapsed = 0;
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
        // perform logging only if triggered by user
        if (!_loggingEnabled) return;

        // log interaction only every `loggingFrequency` frame
        _framesElapsed++;
        if (_framesElapsed % loggingFrequency != 0) return;

        interactionData.frameNumber = _framesElapsed;

        _loggingData.Add(
            $"{_framesElapsed}," +
            $"{Vector3ToString(interactionData.userPosition, log: true)}," +
            $"{Vector3ToString(interactionData.userOrientation, log: true)}," +
            $"{interactionData.cameraHitObjectName}," +
            $"{Vector3ToString(interactionData.cameraHitPoint, log: true)}," +
            $"{interactionData.cameraHitPointDistance:0.00}," +
            $"{interactionData.controllerHitObjectName}," +
            $"{Vector3ToString(interactionData.controllerHitPoint, log: true)}," +
            $"{interactionData.controllerHitPointDistance:0.00}");
    }

    private void StartInteractionLogging()
    {
        Debug.Log("Started user interaction logging.");
        _loggingEnabled = true;

        interactionData.room1StartTime = DateTime.Now;
        button.GetComponentInChildren<TextMeshProUGUI>().text = "LOGGING ...";
        // button.GetComponentInChildren<Image>().color = new Color(80, 197, 68);
    }

    private void StopInteractionLogging()
    {
        Debug.Log("Stopped user interaction logging.");
        _loggingEnabled = false;

        // using a coroutine here as saving to disk requires time so as not to block the main update
        StartCoroutine(LogDataToFile());

        button.GetComponentInChildren<TextMeshProUGUI>().text = "START LOGGING";
        // button.GetComponentInChildren<Image>().color = new Color(197, 76, 68);
    }

    private IEnumerator LogDataToFile()
    {
        if (!Directory.Exists("./Logs"))
        {
            Directory.CreateDirectory("./Logs");
        }

        string currentTime = DateTime.Now.ToString("yyMMdd-HHmmss");

        StreamWriter logWriter = new StreamWriter($"Logs/InteractionLogs-{currentTime}.csv");
        logWriter.WriteLine("frame_no,user_position,user_orientation,camera_hit_obj,camera_hit_point,camera_hit_dist," +
                            "controller_hit_obj,controller_hit_point,controller_hit_distance");

        foreach (string log in _loggingData)
        {
            logWriter.WriteLine(log);
        }

        logWriter.Close();

        StreamWriter timeWriter = new StreamWriter($"Logs/SummaryLogs-{currentTime}.txt");
        timeWriter.WriteLine(
            ($"Room 1 Time: {(interactionData.room2StartTime - interactionData.room1StartTime):g}"));
        timeWriter.WriteLine("Room 1 Start Frame: 0");
        timeWriter.WriteLine(
            ($"Room 2 Time: {(interactionData.room3StartTime - interactionData.room2StartTime):g}"));
        timeWriter.WriteLine($"Room 2 Start Frame: {interactionData.room2StartFrame}");
        timeWriter.WriteLine(
            ($"Room 3 Time: {(interactionData.room3EndTime - interactionData.room3StartTime):g}"));
        timeWriter.WriteLine($"Room 3 Start Frame: {interactionData.room3StartFrame}");
        timeWriter.Close();

        yield return true;
    }

    protected internal static string Vector3ToString(Vector3 vec, bool log = false)
    {
        // format a Vector3 <a, b, c> to string (a, b, c) to display on screen / save to log file
        return log ? $"\"({vec.x:0.00},{vec.y:0.00},{vec.z:0.00})\"" : $"({vec.x:0.00}, {vec.y:0.00}, {vec.z:0.00})";
    }
}

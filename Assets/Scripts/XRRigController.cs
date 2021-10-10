using System;
using System.Collections;
using UnityEngine;
using TMPro;

public class XRRigController : MonoBehaviour
{
    public InteractionData interactionData;

    public TextMeshProUGUI hitObjectText;
    public TextMeshProUGUI hitPointText;
    public TextMeshProUGUI distanceText;
    public TextMeshProUGUI framePerSecText;
    public TextMeshProUGUI playerPositionText;
    public TextMeshProUGUI playerOrientationText;

    [SerializeField] private GameObject mainCamera;

    private const float RayLength = 30.0f;
    private RaycastHit _vision;

    private int _frameCount = 0;
    private double _elapsedTime = 0.0f;
    private double _framePerSecond = 0.0f;
    private const float UpdateRate = 4.0f;

    private void Update()
    {
        UpdateFPS();
        UpdatePlayerInteractionData();
    }

    private void FixedUpdate()
    {
        HeadMountRaycast();
    }

    private void HeadMountRaycast()
    {
        int layerMask = 1 << 8;
        layerMask = ~layerMask;

        Vector3 headsetPos = mainCamera.transform.position;
        Vector3 headsetDir = mainCamera.transform.forward;

        Debug.DrawRay(headsetPos, headsetDir * RayLength, Color.red, 0.5f);

        if (Physics.Raycast(headsetPos, headsetDir, out _vision, RayLength, layerMask))
        {
            hitObjectText.text = _vision.collider.name;
            hitPointText.text = InteractionLogger.Vector3ToString(_vision.point);
            distanceText.text = $"{_vision.distance:0.00}";

            interactionData.cameraHitPoint = _vision.point;
            interactionData.cameraHitObjectName = _vision.collider.name;
            interactionData.cameraHitPointDistance = _vision.distance;
        }
    }

    private void UpdatePlayerInteractionData()
    {
        Vector3 playerPosition = mainCamera.transform.position;
        Vector3 playerOrientation = mainCamera.transform.forward;

        framePerSecText.text = $"{_framePerSecond:0}";
        playerPositionText.text = InteractionLogger.Vector3ToString(playerPosition);
        playerOrientationText.text = InteractionLogger.Vector3ToString(playerOrientation);

        interactionData.userPosition = playerPosition;
        interactionData.userOrientation = playerOrientation;
    }

    private void UpdateFPS()
    {
        // Borrowed from: https://answers.unity.com/questions/64331/accurate-frames-per-second-count.html
        _frameCount++;
        _elapsedTime += Time.smoothDeltaTime;

        if (_elapsedTime > 1.0 / UpdateRate)
        {
            _framePerSecond = _frameCount / _elapsedTime;
            _frameCount = 0;
            _elapsedTime -= 1.0 / UpdateRate;
        }
    }
}
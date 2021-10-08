using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class XRRigController : MonoBehaviour
{
  public float sampleRate = 2.0f;
  public TextMeshProUGUI hitObjectText;
  public TextMeshProUGUI hitPointText;
  public TextMeshProUGUI distanceText;
  public TextMeshProUGUI framePerSecText;
  public TextMeshProUGUI playerPositionText;
  public TextMeshProUGUI playerOrientationText;

  private const float RayLength = 30.0f;
  private GameObject _mainCamera;
  private RaycastHit _vision;

  private int _frameCount = 0;
  private double _elapsedTime = 0.0f;
  private double _framePerSecond = 0.0f;
  private const float UpdateRate = 4.0f;

  private void Awake()
  {
    if (_mainCamera == null)
    {
      _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
    }
  }

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

    Vector3 headsetPos = _mainCamera.transform.position;
    Vector3 headsetDir = _mainCamera.transform.forward;

    Debug.DrawRay(headsetPos, headsetDir * RayLength, Color.red, 0.5f);

    if (Physics.Raycast(headsetPos, headsetDir, out _vision, RayLength, layerMask))
    {
      hitObjectText.text = _vision.collider.name;
      hitPointText.text = FormatVector3(_vision.point);
      distanceText.text = $"{_vision.distance:0.00}";
    }
  }

  private void UpdatePlayerInteractionData()
  {
    Vector3 playerPosition = _mainCamera.transform.position;
    Vector3 playerOrientation = _mainCamera.transform.forward;

    framePerSecText.text = $"{_framePerSecond:0}";
    playerPositionText.text = FormatVector3(playerPosition);
    playerOrientationText.text = FormatVector3(playerOrientation);
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

  private static string FormatVector3(Vector3 vec)
  {
    // format a Vector3 <a, b, c> to string [a, b, c]
    return $"[{vec.x:0.00}, {vec.y:0.00}, {vec.z:0.00}]";
  }
}

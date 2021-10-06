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

  private GameObject mainCamera;
  private float rayLength = 30.0f;
  private RaycastHit vision;

  private int frameCount = 0;
  private double dt = 0.0f, fps = 0.0f;
  private float updateRate = 4.0f;

  private void Awake()
  {
    if (mainCamera == null)
    {
      mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
    }
  }

  private void Update()
  {
    updateFPS();
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

    Debug.DrawRay(headsetPos, headsetDir * rayLength, Color.red, 0.5f);

    if (Physics.Raycast(headsetPos, headsetDir, out vision, rayLength, layerMask))
    {
      hitObjectText.text = vision.collider.name;
      hitPointText.text = FormatVector3(vision.point);
      distanceText.text = string.Format("{0:0.00}", vision.distance);
    }
  }

  private void UpdatePlayerInteractionData()
  {
    Vector3 playerPosition = mainCamera.transform.position;
    Vector3 playerOrientation = mainCamera.transform.forward;

    framePerSecText.text = string.Format("{0:0}", fps);
    playerPositionText.text = FormatVector3(playerPosition);
    playerOrientationText.text = FormatVector3(playerOrientation);
  }

  private void updateFPS()
  {
    // TODO: https://answers.unity.com/questions/64331/accurate-frames-per-second-count.html
    frameCount++;
    dt += Time.smoothDeltaTime;

    if (dt > 1.0 / updateRate)
    {
      fps = frameCount / dt;
      frameCount = 0;
      dt -= 1.0 / updateRate;
    }
  }

  private string FormatVector3(Vector3 vec)
  {
    // format a Vector3 <a, b, c> to string [a, b, c]
    return string.Format("[{0:0.00}, {1:0.00}, {2:0.00}]", vec.x, vec.y, vec.z);
  }
}

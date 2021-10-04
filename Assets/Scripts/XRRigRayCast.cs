using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class XRRigRayCast : MonoBehaviour
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

  private void Awake()
  {
    if (mainCamera == null)
    {
      mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
    }
  }

  private void FixedUpdate()
  {
    HeadMountRaycast();
    UpdatePlayerInteractionData();
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
      hitPointText.text = string.Format("[{0:0.00}, {1:0.00}, {2:0.00}]", vision.point.x, vision.point.y, vision.point.z);
      distanceText.text = string.Format("{0:0.00}", vision.distance);
    }
  }

  private void UpdatePlayerInteractionData()
  {
    Vector3 playerPosition = mainCamera.transform.position;
    Vector3 playerOrientation = mainCamera.transform.forward;

    framePerSecText.text = string.Format("{0}", getFramePerSec());
    playerPositionText.text = string.Format("[{0:0.00}, {1:0.00}, {2:0.00}]", playerPosition.x, playerPosition.y, playerPosition.z);
    playerOrientationText.text = string.Format("[{0:0.00}, {1:0.00}, {2:0.00}]", playerOrientation.x, playerOrientation.y, playerOrientation.z);
  }

  private float getFramePerSec()
  {
    return 1.0f / Time.deltaTime;
  }
}

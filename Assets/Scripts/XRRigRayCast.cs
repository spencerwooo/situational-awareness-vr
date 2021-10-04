using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XRRigRayCast : MonoBehaviour
{
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
      if (vision.collider.CompareTag("Interactive"))
      {
        Debug.Log(vision.collider.name);
      }
    }
  }
}

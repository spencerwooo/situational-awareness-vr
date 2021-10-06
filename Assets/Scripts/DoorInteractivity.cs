using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DoorInteractivity : MonoBehaviour
{
  public InputActionReference TriggerReference = null;
  public bool DoorLocked = true;

  [SerializeField] private Animator doorAnimator;
  [SerializeField] private GameObject lightingTop;
  [SerializeField] private GameObject lightingBottom;
  [SerializeField] private Material lightingRed;
  [SerializeField] private Material lightingGreen;

  private void Awake()
  {
    TriggerReference.action.started += DoorController;
  }

  private void OnDestroy()
  {
    TriggerReference.action.started -= DoorController;
  }

  public void DoorController(InputAction.CallbackContext ctx)
  {
    if (DoorLocked)
    {
      doorUnlock();
    }
    else
    {
      doorLock();
    }
  }

  private void doorUnlock()
  {
    Debug.Log("Door opening.");
    DoorLocked = false;
    doorAnimator.Play("DoorOpen", 0, 0.0f);
    lightingTop.GetComponent<Renderer>().material = lightingGreen;
    lightingBottom.GetComponent<Renderer>().material = lightingGreen;
  }

  private void doorLock()
  {
    Debug.Log("Door closing.");
    DoorLocked = true;
    doorAnimator.Play("DoorClose", 0, 0.0f);
    lightingTop.GetComponent<Renderer>().material = lightingRed;
    lightingBottom.GetComponent<Renderer>().material = lightingRed;
  }
}

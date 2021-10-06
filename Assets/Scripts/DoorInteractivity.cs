using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DoorInteractivity : MonoBehaviour
{
  public InputActionReference triggerReference = null;

  [SerializeField] private Animator doorAnimator;

  private void Awake()
  {
    triggerReference.action.started += DoorUnlock;
  }

  private void OnDestroy()
  {
    triggerReference.action.started -= DoorUnlock;
  }

  public void DoorUnlock(InputAction.CallbackContext ctx)
  {
    Debug.Log("Door unlocked. Performing door open animation.");
    doorAnimator.Play("DoorOpen", 0, 0.0f);
  }
}

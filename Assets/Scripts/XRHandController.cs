using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit;

public class XRHandController : MonoBehaviour
{
  public InputActionReference triggerReference = null;
  [SerializeField] private XRRayInteractor rayInteractor;

  private GameObject _triggeredDoor = null;
  private RaycastHit _rayInteractable;

  private void Awake()
  {
    triggerReference.action.started += DoorController;
  }

  private void OnDestroy()
  {
    triggerReference.action.started -= DoorController;
  }

  private void DoorController(InputAction.CallbackContext ctx)
  {
    if (rayInteractor.TryGetCurrent3DRaycastHit(out _rayInteractable))
    {
      if (_rayInteractable.collider.CompareTag("EscapeDoor"))
      {
        _triggeredDoor = _rayInteractable.collider.gameObject.transform.parent.gameObject;
      }
    }

    if (_triggeredDoor)
    {
      DoorInteractivity doorController = _triggeredDoor.GetComponent<DoorInteractivity>();

      if (doorController.doorLocked)
      {
        doorController.DoorUnlock();
      }
      else
      {
        doorController.DoorLock();
      }
    }
  }
}

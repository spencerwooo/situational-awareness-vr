using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class XRHandController : MonoBehaviour
{
  public InputActionReference TriggerReference = null;
  [SerializeField] private XRRayInteractor rayInteractor;

  private GameObject triggeredDoor = null;
  private RaycastHit rayInteractable;

  private void Awake()
  {
    TriggerReference.action.started += DoorController;
  }

  private void OnDestroy()
  {
    TriggerReference.action.started -= DoorController;
  }

  private void DoorController(InputAction.CallbackContext ctx)
  {
    if (rayInteractor.TryGetCurrent3DRaycastHit(out rayInteractable))
    {
      if (rayInteractable.collider.CompareTag("EscapeDoor"))
      {
        triggeredDoor = rayInteractable.collider.gameObject.transform.parent.gameObject;
      }
    }

    if (triggeredDoor)
    {
      DoorInteractivity doorController = triggeredDoor.GetComponent<DoorInteractivity>();

      if (doorController.DoorLocked)
      {
        doorController.doorUnlock();
      }
      else
      {
        doorController.doorLock();
      }
    }
  }
}

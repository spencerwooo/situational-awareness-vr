using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class XRHandController : MonoBehaviour
{
    public InputActionReference triggerReference = null;
    public InteractionData interactionData;
    
    [SerializeField] private XRRayInteractor rayInteractor;

    private GameObject _triggeredDoor;
    private RaycastHit _rayInteractable;

    private void Awake()
    {
        triggerReference.action.started += DoorController;
    }

    private void OnDestroy()
    {
        triggerReference.action.started -= DoorController;
    }

    private void FixedUpdate()
    {
        if (rayInteractor.TryGetCurrent3DRaycastHit(out _rayInteractable))
        {
            interactionData.controllerHitPoint = _rayInteractable.point;
            interactionData.controllerHitObjectName = _rayInteractable.collider.name;
            interactionData.controllerHitPointDistance = _rayInteractable.distance;
        }
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
            var doorController = _triggeredDoor.GetComponent<DoorInteractivity>();

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
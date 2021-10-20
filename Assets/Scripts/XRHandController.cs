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
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject winText;

    private GameObject _triggeredDoor;
    private GameObject _teleporter;
    private GameObject _triggerCube;
    private RaycastHit _rayInteractable;

    private void Awake()
    {
        triggerReference.action.started += InteractableController;
    }

    private void OnDestroy()
    {
        triggerReference.action.started -= InteractableController;
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

    private void InteractableController(InputAction.CallbackContext ctx)
    {
        if (rayInteractor.TryGetCurrent3DRaycastHit(out _rayInteractable))
        {
            // activated for now, will not be used in the final puzzle
            if (_rayInteractable.collider.CompareTag("EscapeDoor"))
            {
                _triggeredDoor = _rayInteractable.collider.gameObject.transform.parent.gameObject;
            }

            if (_rayInteractable.collider.CompareTag("Teleporter"))
            {
                _teleporter = _rayInteractable.collider.gameObject;
            }

            if (_rayInteractable.collider.CompareTag("TriggerCube"))
            {
                _triggerCube = _rayInteractable.collider.gameObject.transform.parent.gameObject;
            }
        }

        if (_teleporter)
        {
            // teleport player to the next escape room
            switch (_teleporter.gameObject.name)
            {
                case "Teleporter 1":
                    interactionData.room2StartTime = DateTime.Now;
                    break;
                case "Teleporter 2":
                    interactionData.room3StartTime = DateTime.Now;
                    break;
                case "Teleporter 3":
                    interactionData.room3EndTime = DateTime.Now;
                    break;
            }

            Teleporter teleportTarget = _teleporter.GetComponent<Teleporter>();
            if (teleportTarget == null) return;
            if (teleportTarget.teleporterActivated) return;

            player.transform.position = teleportTarget.teleportTarget.transform.position;
            teleportTarget.teleporterActivated = true;
            winText.SetActive(false);

            _teleporter = null;
        }

        if (_triggerCube)
        {
            _triggerCube.GetComponent<CubeNumberCycler>().TriggerNumberCycle();
            _triggerCube = null;
        }

        // TODO: activated for now, will not be used in the final game logic
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

            _triggeredDoor = null;
        }
    }
}
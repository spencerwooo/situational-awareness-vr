using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorMatTrigger : MonoBehaviour
{
  [SerializeField] private GameObject connectedDoor;
  [SerializeField] private GameObject targetTrigger;

  private DoorInteractivity doorInteractivity;

  private void Awake()
  {
    doorInteractivity = connectedDoor.GetComponent<DoorInteractivity>();
  }

  private void OnTriggerEnter(Collider other)
  {
    if (other.gameObject == targetTrigger)
    {
      if (doorInteractivity.DoorLocked)
      {
        doorInteractivity.doorUnlock();
      }
    }
  }

  private void OnTriggerExit(Collider other)
  {
    if (!doorInteractivity.DoorLocked)
    {
      doorInteractivity.doorLock();
    }
  }
}

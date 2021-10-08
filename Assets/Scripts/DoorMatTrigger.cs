using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorMatTrigger : MonoBehaviour
{
  [SerializeField] private GameObject connectedDoor;
  [SerializeField] private GameObject targetTrigger;

  private DoorInteractivity _doorInteractivity;

  private void Awake()
  {
    _doorInteractivity = connectedDoor.GetComponent<DoorInteractivity>();
  }

  private void OnTriggerEnter(Collider other)
  {
    if (other.gameObject == targetTrigger)
    {
      if (_doorInteractivity.doorLocked)
      {
        _doorInteractivity.DoorUnlock();
      }
    }
  }

  private void OnTriggerExit(Collider other)
  {
    if (!_doorInteractivity.doorLocked)
    {
      _doorInteractivity.DoorLock();
    }
  }
}

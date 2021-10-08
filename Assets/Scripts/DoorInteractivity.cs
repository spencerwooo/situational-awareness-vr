using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class DoorInteractivity : MonoBehaviour
{
  public bool doorLocked = true;

  [SerializeField] private Animator doorAnimator;
  [SerializeField] private GameObject lightingTop;
  [SerializeField] private GameObject lightingBottom;
  [SerializeField] private GameObject[] connectedWires;
  [SerializeField] private Material lightingRed;
  [SerializeField] private Material lightingGreen;
  [SerializeField] private Material wireNoPower;
  [SerializeField] private Material wirePowerUp;


  public void DoorUnlock()
  {
    if (!doorLocked) return;
    
    Debug.Log($"Door {gameObject.name} opening.");
    doorLocked = false;

    foreach (GameObject wire in connectedWires)
    {
      wire.GetComponent<Renderer>().material = wirePowerUp;
    }

    doorAnimator.Play("DoorOpen", 0, 0.0f);
    lightingTop.GetComponent<Renderer>().material = lightingGreen;
    lightingBottom.GetComponent<Renderer>().material = lightingGreen;
  }

  public void DoorLock()
  {
    if (doorLocked) return;
    Debug.Log($"Door {gameObject.name} closing.");
    doorLocked = true;

    foreach (var wire in connectedWires)
    {
      wire.GetComponent<Renderer>().material = wireNoPower;
    }

    doorAnimator.Play("DoorClose", 0, 0.0f);
    lightingTop.GetComponent<Renderer>().material = lightingRed;
    lightingBottom.GetComponent<Renderer>().material = lightingRed;
  }
}

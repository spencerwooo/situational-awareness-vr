using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorInteractivity : MonoBehaviour
{
  public bool DoorLocked = true;

  [SerializeField] private Animator doorAnimator;
  [SerializeField] private GameObject lightingTop;
  [SerializeField] private GameObject lightingBottom;
  [SerializeField] private Material lightingRed;
  [SerializeField] private Material lightingGreen;

  public void doorUnlock()
  {
    if (DoorLocked)
    {

      Debug.Log(string.Format("Door {0} opening.", gameObject.name));
      DoorLocked = false;
      doorAnimator.Play("DoorOpen", 0, 0.0f);
      lightingTop.GetComponent<Renderer>().material = lightingGreen;
      lightingBottom.GetComponent<Renderer>().material = lightingGreen;
    }
  }

  public void doorLock()
  {
    if (!DoorLocked)
    {

      Debug.Log(string.Format("Door {0} closing.", gameObject.name));
      DoorLocked = true;
      doorAnimator.Play("DoorClose", 0, 0.0f);
      lightingTop.GetComponent<Renderer>().material = lightingRed;
      lightingBottom.GetComponent<Renderer>().material = lightingRed;
    }
  }
}

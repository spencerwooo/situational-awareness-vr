using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorInteractivity : MonoBehaviour
{
  [SerializeField] private Animator doorAnimator;

  private void OnMouseEnter()
  {
    Debug.Log("Mouse entered.");
    DoorOpen();
  }

  public void DoorOpen()
  {
    Debug.Log("Performing door open animation.");
    doorAnimator.Play("DoorOpen", 0, 0.0f);
    gameObject.SetActive(false);
  }
}

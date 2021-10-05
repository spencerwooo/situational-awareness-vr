using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRHandController : MonoBehaviour
{
  // https://forum.unity.com/threads/door-open-close-in-c.410811/
  // https://forum.unity.com/threads/any-example-of-the-new-2019-1-xr-input-system.629824/#post-4360375
  public bool isTriggerPressed;
  public ActionBasedController controller;

  void Start()
  {
    if (controller == null)
    {
      controller = GetComponent<ActionBasedController>();
    }
  }

  void Update()
  {
    // Debug.Log(controller.selectAction.action.ReadValue<bool>());
  }
}

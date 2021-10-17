using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapLayerWhenGrabbed : MonoBehaviour
{
    public void ChangeLayerOnGrab()
    {
        gameObject.layer = 8;
    }

    public void RevertLayerOnLeave()
    {
        gameObject.layer = 0;
    }
}
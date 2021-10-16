using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyRotator : MonoBehaviour
{
    private void Update()
    {
        Vector3 rotation = new Vector3(15, 30, 45);
        transform.Rotate(rotation * Time.deltaTime);
    }
}
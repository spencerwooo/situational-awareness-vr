using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileTrigger : MonoBehaviour
{
    [SerializeField] private Material tileIdle;
    [SerializeField] private Material tileActive;
    [SerializeField] private Material tileWrong;
    [SerializeField] private Material tileSolved;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger enter");
        if (other.gameObject.CompareTag("Player"))
        {
            gameObject.GetComponent<MeshRenderer>().material = tileActive;
        }
    }
}

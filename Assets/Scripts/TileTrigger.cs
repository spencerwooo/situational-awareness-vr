using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public enum TileState
{
    Idle,
    Active,
    Wrong,
    Solved
}

public class TileTrigger : MonoBehaviour
{
    public TileState tileState = TileState.Idle;

    [SerializeField] private Material tileIdle;
    [SerializeField] private Material tileActive;
    [SerializeField] private Material tileWrong;

    [SerializeField] private GameObject key;
    private KeyController _keyController;

    private void Start()
    {
        _keyController = key.GetComponent<KeyController>();
    }

    private async void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger enter");

        if (tileState == TileState.Solved || tileState == TileState.Wrong) return;
        if (!other.gameObject.CompareTag("Player")) return;

        switch (tileState)
        {
            case TileState.Idle:
                // tile from idle to active as was stepped on
                gameObject.GetComponent<MeshRenderer>().material = tileActive;
                tileState = TileState.Active;
                break;

            case TileState.Active:
                // tile from active to wrong as stepped on repeatedly
                gameObject.GetComponent<MeshRenderer>().material = tileWrong;
                tileState = TileState.Wrong;

                // wait for 1 second and revert tile state to idle
                await UseDelay();
                gameObject.GetComponent<MeshRenderer>().material = tileIdle;
                tileState = TileState.Idle;
                break;
        }
        
        _keyController.ValidatePuzzle();
    }

    private static async Task UseDelay()
    {
        await Task.Delay(1000);
    }
}
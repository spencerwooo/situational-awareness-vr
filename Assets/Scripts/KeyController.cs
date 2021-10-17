using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class KeyController : MonoBehaviour
{
    [SerializeField] private GameObject connectedDoor;

    [SerializeField] private Material tileSolved;

    [SerializeField] private Material keyUnlocked;
    [SerializeField] private Material keyLocked;

    private GameObject[] _tiles;
    private TileTrigger[] _tileTriggers;
    private DoorInteractivity _doorInteractivity;

    private void Start()
    {
        _tiles = GameObject.FindGameObjectsWithTag("TriggerTile");
        _tileTriggers = _tiles.Select(t => t.GetComponent<TileTrigger>()).ToArray();

        _doorInteractivity = connectedDoor.GetComponent<DoorInteractivity>();
    }

    private void Update()
    {
        Vector3 rotation = new Vector3(15, 30, 45);
        transform.Rotate(rotation * Time.deltaTime);
    }

    public void ValidatePuzzle()
    {
        if (_tileTriggers.All(t => t.tileState == TileState.Active))
        {
            foreach (var t in _tiles)
            {
                t.GetComponent<MeshRenderer>().material = tileSolved;
                t.GetComponent<TileTrigger>().tileState = TileState.Solved;
            }

            GetComponent<MeshRenderer>().material = keyUnlocked;
            if (_doorInteractivity.doorLocked)
            {
                _doorInteractivity.DoorUnlock();
            }
        }
        else
        {
            GetComponent<MeshRenderer>().material = keyLocked;
            _doorInteractivity.DoorLock();
        }
    }
}
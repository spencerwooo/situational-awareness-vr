using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PuzzleTrigger : MonoBehaviour
{
    [SerializeField] private GameObject exitDoor;
    [SerializeField] private GameObject[] puzzleCubes;

    private DoorInteractivity _doorInteractivity;
    private CubeNumberCycler[] _cubeNumberCyclers;

    private void Start()
    {
        _doorInteractivity = exitDoor.GetComponent<DoorInteractivity>();
        _cubeNumberCyclers = puzzleCubes.Select(cube => cube.GetComponent<CubeNumberCycler>()).ToArray();
    }

    public void ValidatePuzzle()
    {
        bool[] puzzleSolvedBools = _cubeNumberCyclers.Select(cycler => cycler.puzzleSolved).ToArray();

        if (puzzleSolvedBools.All(b => b))
        {
            if (_doorInteractivity.doorLocked)
            {
                _doorInteractivity.DoorUnlock();
            }
        }
        else
        {
            _doorInteractivity.DoorLock();
        }
    }
}
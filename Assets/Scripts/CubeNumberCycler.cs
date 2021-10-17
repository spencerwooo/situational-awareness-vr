using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeNumberCycler : MonoBehaviour
{
    public bool puzzleSolved = false;

    [SerializeField] private GameObject puzzleCube;
    [SerializeField] private GameObject cubeNumber;
    [SerializeField] private int targetValue;

    [SerializeField] private Material energizedCube;
    [SerializeField] private Material cubeNoEnergy;

    [SerializeField] private GameObject puzzleCubeTrigger;
    private PuzzleTrigger _puzzleTrigger;

    private void Start()
    {
        _puzzleTrigger = puzzleCubeTrigger.GetComponent<PuzzleTrigger>();
    }

    public void TriggerNumberCycle()
    {
        // handle puzzle cube triggered by xr interactor
        TextMesh cubeNumberText = cubeNumber.GetComponent<TextMesh>();
        int number = int.Parse(cubeNumberText.text);

        number = (number + 1) % 4;
        number = number == 0 ? 4 : number;
        cubeNumberText.text = number.ToString();

        if (number == targetValue)
        {
            puzzleCube.GetComponent<Renderer>().material = energizedCube;
            puzzleSolved = true;
        }
        else
        {
            puzzleCube.GetComponent<Renderer>().material = cubeNoEnergy;
            puzzleSolved = false;
        }

        _puzzleTrigger.ValidatePuzzle();
    }
}
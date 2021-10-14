using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeNumberCycler : MonoBehaviour
{
    [SerializeField] private GameObject puzzleCube;
    [SerializeField] private GameObject cubeNumber;
    [SerializeField] private int targetValue;

    [SerializeField] private Material energizedCube;
    [SerializeField] private Material cubeNoEnergy;

    public void TriggerNumberCycle()
    {
        TextMesh cubeNumberText = cubeNumber.GetComponent<TextMesh>();
        int number = int.Parse(cubeNumberText.text);

        number = (number + 1) % 4;
        number = number == 0 ? 4 : number;
        cubeNumberText.text = number.ToString();

        puzzleCube.GetComponent<Renderer>().material = number == targetValue ? energizedCube : cubeNoEnergy;
    }
}
using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class HueShifter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI winText;

    private void Update()
    {
        // change color from RGB color space to HSV color space, then shift hue every delta time
        Color.RGBToHSV(winText.color, out var h, out var s, out var v);
        winText.color = Color.HSVToRGB(h + Time.deltaTime * .25f, s, v);
    }
}
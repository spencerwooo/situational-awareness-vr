using System;
using UnityEngine;

[CreateAssetMenu]
public class InteractionData : ScriptableObject
{
    [NonSerialized] public string cameraHitObjectName;
    [NonSerialized] public Vector3 cameraHitPoint;
    [NonSerialized] public float cameraHitPointDistance;

    [NonSerialized] public Vector3 userPosition;
    [NonSerialized] public Vector3 userOrientation;

    [NonSerialized] public string controllerHitObjectName;
    [NonSerialized] public Vector3 controllerHitPoint;
    [NonSerialized] public float controllerHitPointDistance;
}
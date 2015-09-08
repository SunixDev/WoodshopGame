using UnityEngine;
using System.Collections;

public class CameraPositioning : MonoBehaviour 
{
    [Header("Distance Variables")]
    public float DistanceFromPoint = 2.0f;
    public float MinDistance = 0.5f;
    public float MaxDistance = 5.0f;

    [Header("Vertical Rotation Limits")]
    public float yMinRotation = 10.0f;
    public float yMaxRotation = 80.0f;

    [Header("New Rotation")]
    public float Vertical = 90;
    public float Horizontal = 90;
    public bool AllowRotation = true;
}

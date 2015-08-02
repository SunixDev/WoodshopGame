﻿using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour 
{
    public bool MovementEnabled = true;

    [Header("Distance Variables")]
    public Transform LookAtPoint;
    public float DistanceFromPoint = 2.0f;
    public float MinDistance = 0.5f;
    public float MaxDistance = 5.0f;

    [Header("Speed Input")]
    [Range(5.0f, 20.0f)]
    public float SensitivityX = 6.0f;
    [Range(5.0f, 20.0f)]
    public float SensitivityY = 6.0f;
    [Range(1.0f, 5.0f)]
    public float ZoomSensitivity = 1.0f;

    [Header("Vertical Rotaion Clamp")]
    public float yMinRotation = 10.0f;
    public float yMaxRotation = 80.0f;

    [Header("Initial Rotaion")]
    public float Vertical = 90;
    public float Horizontal = 90;

    private float xMovement;
    private float yMovement;

	void Start () 
    {
        xMovement = Vertical;
        yMovement = Horizontal;
	}
	
	void Update () 
    {
        if (MovementEnabled)
        {
            Vector3 distanceVector = new Vector3(0.0f, 0.0f, -DistanceFromPoint);
            Quaternion rotation = Quaternion.Euler(yMovement, xMovement, 0.0f);
            Vector3 finalPosition = LookAtPoint.position + (rotation * distanceVector);

            transform.position = finalPosition;
            transform.LookAt(LookAtPoint.position);
        }
    }

    public void OrbitCamera(Gesture gesture)
    {
        if (MovementEnabled)
        {
            xMovement += gesture.deltaPosition.x * (SensitivityX / 2.0f) * Time.deltaTime;
            yMovement -= gesture.deltaPosition.y * (SensitivityY / 2.0f) * Time.deltaTime;
            yMovement = ClampAngle(yMovement);
        }
    }

    public void ZoomAwayFromPoint(Gesture gesture)
    {
        if (MovementEnabled)
        {
            float zoomAmount = gesture.deltaPinch * (ZoomSensitivity / 10.0f) * Time.deltaTime;
            DistanceFromPoint += zoomAmount;
            DistanceFromPoint = Mathf.Clamp(DistanceFromPoint, MinDistance, MaxDistance);
        }
    }

    public void ZoomTowardsPoint(Gesture gesture)
    {
        if (MovementEnabled)
        {
            float zoomAmount = gesture.deltaPinch * (ZoomSensitivity / 10.0f) * Time.deltaTime;
            DistanceFromPoint -= zoomAmount;
            DistanceFromPoint = Mathf.Clamp(DistanceFromPoint, MinDistance, MaxDistance);
        }
    }

    public void Drag(Gesture gesture)
    {
        if (MovementEnabled)
        {
            OrbitCamera(gesture);
        }
    }

    private float ClampAngle(float amount)
    {
        float fullRotation = 360;
        if (amount < -fullRotation) amount += fullRotation;
        if (amount > fullRotation) amount -= fullRotation;
        return Mathf.Clamp(amount, yMinRotation, yMaxRotation);
    }





    void OnEnable()
    {
        EasyTouch.On_Swipe += OrbitCamera;
        EasyTouch.On_PinchIn += ZoomAwayFromPoint;
        EasyTouch.On_PinchOut += ZoomTowardsPoint;
        EasyTouch.On_Drag += Drag;
    }

    void OnDestroy()
    {
        EasyTouch.On_Swipe -= OrbitCamera;
        EasyTouch.On_PinchIn -= ZoomAwayFromPoint;
        EasyTouch.On_PinchOut -= ZoomTowardsPoint;
        EasyTouch.On_Drag -= Drag;
    }
}
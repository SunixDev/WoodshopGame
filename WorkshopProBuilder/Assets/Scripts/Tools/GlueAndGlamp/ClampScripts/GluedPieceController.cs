﻿using UnityEngine;
using System.Collections;

public class GluedPieceController : MonoBehaviour 
{
    public float RotationSpeed = 5.0f;
    public bool RotateX_Axis = false;
    public bool RotateY_Axis = true;

    private Transform objTransform;
    private bool selected;

    void Awake()
    {
        objTransform = transform;
        selected = false;
    }

    public void RotatePiece(Gesture gesture)
    {
        if (gesture.pickedObject != null && gesture.touchCount == 1 && !gesture.IsOverUIElement())
        {
            if (selected)
            {
                if (RotateY_Axis)
                {
                    float yRotation = gesture.deltaPosition.x * RotationSpeed * 0.1f;
                    objTransform.Rotate(0f, -yRotation, 0f, Space.World);
                }
                if (RotateX_Axis)
                {
                    float xRotation = gesture.deltaPosition.x * RotationSpeed * 0.1f;
                    objTransform.Rotate(xRotation, 0, 0f, Space.World);
                }
            }
            else
            {
                string pickedObjectTag = gesture.pickedObject.tag;
                selected = (pickedObjectTag == "Piece" || pickedObjectTag == "WoodProject" || pickedObjectTag == "Tool");
            }
        }
    }

    public void StopRotatingPiece(Gesture gesture)
    {
        selected = false;
    }



    private void EnableTouchEvents()
    {
        EasyTouch.On_Drag += RotatePiece;
        EasyTouch.On_DragEnd += StopRotatingPiece;
    }

    private void DisableTouchEvents()
    {
        EasyTouch.On_Drag -= RotatePiece;
        EasyTouch.On_DragEnd -= StopRotatingPiece;
    }

    void OnEnable()
    {
        EnableTouchEvents();
    }
    void OnDisable()
    {
        DisableTouchEvents();
    }
    void OnDestory()
    {
        DisableTouchEvents();
    }
}

//private float ClampAngle(float amount)
//{
//    float fullRotation = 360.0f;
//    if (amount < 0.0f)
//        amount += fullRotation;
//    if (amount > fullRotation)
//        amount -= fullRotation;
//    return Mathf.Clamp(amount, 0, 360);
//}
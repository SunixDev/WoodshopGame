using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class BoardController : MonoBehaviour
{
    public bool Moveable;
    public bool IsMoving { get; set; }
    public Transform RotationPoint;
    public Rigidbody objRigidbody { get; set; }
    public WoodMaterialObject Manager { get; set; }

    private Vector3 deltaPosition;
    private Vector2 previousTouchPosition;
    private Vector3 direction;
    private bool directionIsSet;

    void Start()
    {
        objRigidbody = GetComponent<Rigidbody>();
        Manager = GetComponent<WoodMaterialObject>();
        deltaPosition = Vector3.zero;
        previousTouchPosition = Vector3.zero;
        direction = Vector3.zero;
        directionIsSet = false;
    }

    public void OnTouch(Gesture gesture)
    {
        if (Moveable && Manager.ContainsPiece(gesture.pickedObject))
        {
            IsMoving = true;
            previousTouchPosition = gesture.position;
            Vector3 position = gesture.GetTouchToWorldPoint(gesture.pickedObject.transform.position);
            deltaPosition = position - objRigidbody.position;
        }
    }

    public void MoveObject(Gesture gesture)
    {
        if (Moveable && Manager.ContainsPiece(gesture.pickedObject))
        {
            if (!directionIsSet)
            {
                direction = DetermineDirection(gesture.swipe);
                directionIsSet = true;
            }
            IsMoving = (previousTouchPosition != gesture.position);
            Vector3 position = gesture.GetTouchToWorldPoint(gesture.pickedObject.transform.position);
            Vector3 nextPosition = position - deltaPosition;
            float x = (direction.x == 0.0f) ? objRigidbody.position.x : nextPosition.x;
            float y = objRigidbody.position.y;
            float z = (direction.z == 0.0f) ? objRigidbody.position.z : nextPosition.z;
            objRigidbody.position = new Vector3(x, y, z);
            previousTouchPosition = gesture.position;

        }
    }

    public void OnTouchRelease(Gesture gesture)
    {
        IsMoving = false;
        directionIsSet = false;
        deltaPosition = Vector3.zero;
        previousTouchPosition = Vector3.zero;
        direction = Vector3.zero;
    }

    public void RotateAroundPoint(Gesture gesture)
    {
        if (Moveable && Manager.ContainsPiece(gesture.pickedObject) && RotationPoint != null)
        {
            transform.RotateAround(RotationPoint.position, Vector3.up, -gesture.twistAngle);
        }
        else if (Moveable && Manager.ContainsPiece(gesture.pickedObject) && RotationPoint == null)
        {
            Vector3 position = gesture.GetTouchToWorldPoint(gesture.pickedObject.transform.position);
            transform.Rotate(Vector3.up, -gesture.twistAngle, Space.World);
        }
    }

    private Vector3 DetermineDirection(EasyTouch.SwipeDirection swipeDirection)
    {
        Vector3 dir = Vector3.zero;
        if (swipeDirection == EasyTouch.SwipeDirection.Up || swipeDirection == EasyTouch.SwipeDirection.Down)
        {
            dir = Vector3.forward;
        }
        else if (swipeDirection == EasyTouch.SwipeDirection.Left || swipeDirection == EasyTouch.SwipeDirection.Right)
        {
            dir = Vector3.right;
        }
        return dir;
    }


    void OnEnable()
    {
        EasyTouch.On_DragStart += OnTouch;
        EasyTouch.On_Drag += MoveObject;
        EasyTouch.On_DragEnd += OnTouchRelease;
        EasyTouch.On_Twist += RotateAroundPoint;
    }

    void OnDestroy()
    {
        EasyTouch.On_DragStart -= OnTouch;
        EasyTouch.On_Drag -= MoveObject;
        EasyTouch.On_DragEnd -= OnTouchRelease;
        EasyTouch.On_Twist -= RotateAroundPoint;
    }
}
/*
public void RotateObject(Gesture gesture)
{
    Vector3 position = gesture.GetTouchToWorldPoint(objTransform.position);
    objTransform.RotateAround(position, Vector3.forward, gesture.twistAngle);
}
*/
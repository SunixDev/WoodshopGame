using UnityEngine;
using System.Collections;

public class PieceController : MonoBehaviour 
{
    public bool Moveable { get; set; }

    private Transform objTransform;
    private Vector3 deltaPosition;
    private Vector3 deltaPositionForRotation;

    void Start()
    {
        Moveable = true;
        objTransform = transform;
        deltaPosition = Vector3.zero;
    }

    void Update()
    {
        //objTransform.Rotate(transform.up, -10.0f);
    }

    public void MovePieceStart(Gesture gesture)
    {
        if (gesture.pickedObject == gameObject && Moveable && gesture.touchCount == 1)
        {
            Vector3 position = gesture.GetTouchToWorldPoint(gesture.pickedObject.transform.position);
            deltaPosition = position - transform.position;
        }
    }

    public void MovePiece(Gesture gesture)
    {
        if (gesture.pickedObject == gameObject && Moveable && gesture.touchCount == 1)
        {
            Vector3 position = gesture.GetTouchToWorldPoint(gesture.pickedObject.transform.position);
            transform.position = position - deltaPosition;
        }
    }

    public void RotatePieceStart(Gesture gesture)
    {
        if (gesture.pickedObject == gameObject && Moveable && gesture.touchCount == 2)
        {
            Vector3 position = gesture.GetTouchToWorldPoint(gesture.pickedObject.transform.position);
            deltaPositionForRotation = position - transform.position;
        }
    }

    public void RotatePiece(Gesture gesture)
    {
        if (gesture.pickedObject == gameObject && Moveable && gesture.touchCount == 2)
        {
            Vector3 position = gesture.GetTouchToWorldPoint(objTransform.position);
            deltaPositionForRotation = position - deltaPositionForRotation;
            Vector3 axis = GetRotationAxis(gesture.swipe);
            objTransform.Rotate(axis * deltaPositionForRotation.magnitude * 2.0f, Space.World);
        }
    }

    public void TwistRotateObject(float twistAngle)
    {
        objTransform.Rotate(new Vector3(0.0f, 0.0f, twistAngle), Space.World);
    }

    private Vector3 GetRotationAxis(EasyTouch.SwipeDirection direction)
    {
        Vector3 axis = new Vector3();

        if (direction == EasyTouch.SwipeDirection.Up)
        {
            axis = Vector3.right;
        }
        else if (direction == EasyTouch.SwipeDirection.Down)
        {
            axis = Vector3.right * -1;
        }
        else if (direction == EasyTouch.SwipeDirection.Left)
        {
            axis = Vector3.up;
        }
        else if (direction == EasyTouch.SwipeDirection.Right)
        {
            axis = Vector3.up * -1;
        }

        return axis;
    }
}

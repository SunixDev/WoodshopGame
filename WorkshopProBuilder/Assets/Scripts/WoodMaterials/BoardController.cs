using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class BoardController : MonoBehaviour
{
    public enum ControlType
    {
        Freeform,
        Slide
    }

    public bool Moveable;
    public Transform RotationPoint;
    public bool RestrictX;
    public bool RestrictZ;
    public ControlType control = ControlType.Freeform;
    public Rigidbody objRigidbody { get; set; }
    public WoodMaterialObject WoodObject { get; set; }

    private bool selected = false;
    private bool directionSelected = false;
    private Vector3 direction;

    private Vector3 deltaPosition;

    void Start()
    {
        objRigidbody = GetComponent<Rigidbody>();
        WoodObject = GetComponent<WoodMaterialObject>();
        deltaPosition = Vector3.zero;
    }

    void Update()
    {
        if (control == ControlType.Freeform)
        {
            EasyTouch.SetStationaryTolerance(5.0f);
        }
        else
        {
            EasyTouch.SetStationaryTolerance(5.0f);
        }
    }

    public void OnTouchStart(Gesture gesture)
    {
        if (Moveable && WoodObject.ContainsPiece(gesture.pickedObject))
        {
            selected = true;
        }
    }

    public void OnDragStart(Gesture gesture)
    {
        if (Moveable && selected)
        {
            Vector3 position = gesture.GetTouchToWorldPoint(gesture.pickedObject.transform.position);
            deltaPosition = position - objRigidbody.position;
        }
    }

    public void OnDragRelease(Gesture gesture)
    {
        selected = false;
        directionSelected = false;
    }

    public void MoveObject(Gesture gesture)
    {
        if (Moveable && selected)
        {
            Vector3 position = gesture.GetTouchToWorldPoint(gesture.pickedObject.transform.position);
            Vector3 nextPosition = position - deltaPosition;
            nextPosition = DetermineRestrictions(nextPosition);
            if (control == ControlType.Freeform)
            {
                objRigidbody.position = new Vector3(nextPosition.x, objRigidbody.position.y, nextPosition.z);
            }
            else
            {
                if (!directionSelected)
                {
                    direction = GetDirectionVector(gesture.swipe);
                }
                float x = (direction.x > 0.0f) ? nextPosition.x : objRigidbody.position.x;
                float y = objRigidbody.position.y;
                float z = (direction.z > 0.0f) ? nextPosition.z : objRigidbody.position.z;
                objRigidbody.position = new Vector3(x, y, z);
            }
        }
    }

    public void RotateAroundPoint(Gesture gesture)
    {
        if (Moveable && selected)
        {
            if (RotationPoint != null)
            {
                transform.RotateAround(RotationPoint.position, Vector3.up, -gesture.twistAngle);
            }
            else
            {
                transform.Rotate(Vector3.up, -gesture.twistAngle, Space.World);
            }
        }
    }

    private Vector3 GetDirectionVector(EasyTouch.SwipeDirection swipe)
    {
        Vector3 direction = new Vector3();
        if (swipe == EasyTouch.SwipeDirection.Up || swipe == EasyTouch.SwipeDirection.Down)
        {
            directionSelected = true;
            direction = Vector3.forward;
        }
        else if (swipe == EasyTouch.SwipeDirection.Left || swipe == EasyTouch.SwipeDirection.Right)
        {
            directionSelected = true;
            direction = Vector3.right;
        }

        return direction;
    }

    private Vector3 DetermineRestrictions(Vector3 updatedVector)
    {
        Vector3 constrainedVector = new Vector3();
        constrainedVector.x = (RestrictX) ? objRigidbody.position.x : updatedVector.x;
        constrainedVector.y = objRigidbody.position.y;
        constrainedVector.z = (RestrictZ) ? objRigidbody.position.z : updatedVector.z;
        return constrainedVector;
    }





    void OnEnable()
    {
        EasyTouch.On_TouchDown += OnTouchStart;
        EasyTouch.On_DragStart += OnDragStart;
        EasyTouch.On_Drag += MoveObject;
        EasyTouch.On_Twist += RotateAroundPoint;
        EasyTouch.On_DragEnd += OnDragRelease;
    }

    void OnDisable()
    {
        EasyTouch.On_TouchDown -= OnTouchStart;
        EasyTouch.On_DragStart -= OnDragStart;
        EasyTouch.On_Drag -= MoveObject;
        EasyTouch.On_Twist -= RotateAroundPoint;
        EasyTouch.On_DragEnd -= OnDragRelease;
    }

    void OnDestroy()
    {
        EasyTouch.On_TouchDown -= OnTouchStart;
        EasyTouch.On_DragStart -= OnDragStart;
        EasyTouch.On_Drag -= MoveObject;
        EasyTouch.On_Twist -= RotateAroundPoint;
        EasyTouch.On_DragEnd -= OnDragRelease;
    }
}
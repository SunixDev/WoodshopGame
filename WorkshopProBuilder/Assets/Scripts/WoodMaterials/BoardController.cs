using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class BoardController : MonoBehaviour
{
    public bool Moveable;
    public Transform RotationPoint;
    public bool RestrictX;
    public bool RestrictZ;
    public bool PointRotation;
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
        //Debug.Log("transform: " + transform.position);
        //Debug.Log("objRigidbody: " + objRigidbody.position);
    }

    public void OnTouchStart(Gesture gesture)
    {
        if (Moveable && WoodObject.ContainsPiece(gesture.pickedObject) && gesture.touchCount == 1)
        {
            selected = true;
        }
    }

    public void OnTouchStart_TwoFingers(Gesture gesture)
    {
        if (Moveable && WoodObject.ContainsPiece(gesture.pickedObject) && gesture.touchCount == 2)
        {
            selected = true;
        }
    }

    public void OnDragStart(Gesture gesture)
    {
        if (Moveable && selected && gesture.touchCount == 1)
        {
            Vector3 position = gesture.GetTouchToWorldPoint(gesture.pickedObject.transform.position);
            deltaPosition = position;
        }
    }

    public void OnDragStart_TwoFingers(Gesture gesture)
    {
        if (Moveable && selected && gesture.touchCount == 2)
        {
            Vector3 position = gesture.GetTouchToWorldPoint(gesture.pickedObject.transform.position);
            deltaPosition = position;// -objRigidbody.position;
        }
    }

    public void OnDragRelease(Gesture gesture)
    {
        selected = false;
        directionSelected = false;
    }

    public void OnDragRelease_TwoFingers(Gesture gesture)
    {
        if (gesture.touchCount <= 0)
        {
            OnDragRelease(gesture);
        }
    }

    public void MoveObject_SingleFingerTouch(Gesture gesture)
    {
        if (Moveable && selected && gesture.touchCount == 1)
        {
            MoveObject(gesture);            
        }
    }

    public void MoveObject_TwoFingerTouch(Gesture gesture)
    {
        if (Moveable && selected && gesture.touchCount == 2)
        {
           MoveObject(gesture);
        }
    }

    public void RotateAroundPoint(Gesture gesture)
    {
        if (Moveable && selected && gesture.touchCount == 2)
        {
            if (RotationPoint != null)
            {
                transform.RotateAround(RotationPoint.position, Vector3.up, -gesture.twistAngle);
            }
            else
            {
                Vector3 axis = Vector3.up;
                Vector3 position = gesture.GetTouchToWorldPoint(gesture.pickedObject.transform.position);
                transform.RotateAround(position, axis, -gesture.twistAngle);
            }
        }
    }

    private Vector3 GetSwipeDirectionVector(EasyTouch.SwipeDirection swipe)
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
        constrainedVector.x = (RestrictX) ? 0.0f : updatedVector.x; //objRigidbody.position.x
        constrainedVector.y = 0.0f;
        constrainedVector.z = (RestrictZ) ? 0.0f : updatedVector.z;
        return constrainedVector;
    }

    private void MoveObject(Gesture gesture)
    {
        Vector3 position = gesture.GetTouchToWorldPoint(gesture.pickedObject.transform.position);
        Vector3 nextPosition = position - deltaPosition;
        deltaPosition = position;
        nextPosition = DetermineRestrictions(nextPosition);
        objRigidbody.position += nextPosition;
    }

#region EventSubscriptions
    void OnEnable()
    {
        SubscribeAll();
    }

    void OnDisable()
    {
        UnsubscribeAll();
    }

    void OnDestroy()
    {
        UnsubscribeAll();
    }

    private void SubscribeAll()
    {
        EasyTouch.On_TouchStart += OnTouchStart;
        EasyTouch.On_TouchStart2Fingers += OnTouchStart_TwoFingers;

        EasyTouch.On_DragStart += OnDragStart;
        EasyTouch.On_DragStart2Fingers += OnDragStart_TwoFingers;

        EasyTouch.On_Drag += MoveObject_SingleFingerTouch;
        EasyTouch.On_Drag2Fingers += MoveObject_TwoFingerTouch;

        EasyTouch.On_Twist += RotateAroundPoint;
        EasyTouch.On_DragEnd += OnDragRelease;
    }

    private void UnsubscribeAll()
    {
        EasyTouch.On_TouchStart -= OnTouchStart;
        EasyTouch.On_TouchStart2Fingers -= OnTouchStart_TwoFingers;

        EasyTouch.On_DragStart -= OnDragStart;
        EasyTouch.On_DragStart2Fingers -= OnDragStart_TwoFingers;

        EasyTouch.On_Drag -= MoveObject_SingleFingerTouch;
        EasyTouch.On_Drag2Fingers -= MoveObject_TwoFingerTouch;

        EasyTouch.On_Twist -= RotateAroundPoint;
        EasyTouch.On_DragEnd -= OnDragRelease;
    }
#endregion

}



//float x = nextPosition.x; //(direction.x > 0.0f) ? nextPosition.x : objRigidbody.position.x;
//float y = objRigidbody.position.y;
//float z = nextPosition.z; //(direction.z > 0.0f) ? nextPosition.z : objRigidbody.position.z;
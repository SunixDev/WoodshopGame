using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ClampController : MonoBehaviour 
{
    public bool Moveable;
    public Transform objTransform { get; set; }
    public List<Collider> Colliders;
    public Transform RotationPoint;

    private bool selected = false;
    private Vector3 previousPosition;

    void Start()
    {
        objTransform = GetComponent<Transform>();
    }

    public void ResetSelection()
    {
        Moveable = false;
        selected = false;
    }

    public void OnTouchStart(Gesture gesture)
    {
        if (gesture.pickedObject != null)
        {
            if (Moveable && ContainsCollider(gesture.pickedObject.GetComponent<Collider>()))
            {
                selected = true;
            }
        }
    }

    public void OnDragStart(Gesture gesture)
    {
        if (Moveable && selected && gesture.touchCount == 1)
        {
            Vector3 position = gesture.GetTouchToWorldPoint(transform.position);
            previousPosition = position;
        }
    }

    public void OnDragRelease(Gesture gesture)
    {
        selected = false;
        previousPosition = Vector3.zero;
    }

    public void OnDragRelease_TwoFingers(Gesture gesture)
    {
        OnDragRelease(gesture);
    }

    public void MoveObject_SingleFingerTouch(Gesture gesture)
    {
        if (Moveable && selected && gesture.touchCount == 1)
        {
            MoveObject(gesture);
        }
    }

    public void Rotate(Gesture gesture)
    {
        if (Moveable && selected && gesture.touchCount == 2)
        {
            Vector3 axis = transform.up;
            transform.Rotate(axis, -gesture.twistAngle);
        }
    }

    public void RotateAtPoint(float angle)
    {
        transform.RotateAround(RotationPoint.position, transform.forward, 90.0f);
    }

    private void MoveObject(Gesture gesture)
    {
        Vector3 position = gesture.GetTouchToWorldPoint(transform.position);
        Vector3 nextPosition = position - previousPosition;
        previousPosition = position;
        objTransform.position += nextPosition;
    }

    private bool ContainsCollider(Collider collider)
    {
        bool containsCollider = false;
        for (int i = 0; i < Colliders.Count && !containsCollider; i++)
        {
            containsCollider = (Colliders[i] == collider);
        }
        return containsCollider;
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
        //EasyTouch.On_DoubleTap += ResetRotationWithDoubleTap;

        EasyTouch.On_DragStart += OnDragStart;

        EasyTouch.On_Drag += MoveObject_SingleFingerTouch;

        EasyTouch.On_Twist += Rotate;
        EasyTouch.On_DragEnd += OnDragRelease;
        EasyTouch.On_TwistEnd += OnDragRelease;
    }

    private void UnsubscribeAll()
    {
        EasyTouch.On_TouchStart -= OnTouchStart;
        //EasyTouch.On_DoubleTap -= ResetRotationWithDoubleTap;

        EasyTouch.On_DragStart -= OnDragStart;

        EasyTouch.On_Drag -= MoveObject_SingleFingerTouch;

        EasyTouch.On_Twist -= Rotate;
        EasyTouch.On_DragEnd -= OnDragRelease;
        EasyTouch.On_TwistEnd -= OnDragRelease;
    }
    #endregion
}



//public void ResetRotationWithDoubleTap(Gesture gesture)
//{
//    if (Moveable && selected)
//    {
//        ResetRotation();
//        selected = false;
//    }
//}

//public void ResetRotation()
//{
//    transform.rotation = Quaternion.identity;
//}
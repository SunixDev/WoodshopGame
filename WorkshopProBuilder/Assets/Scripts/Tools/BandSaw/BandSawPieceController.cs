using UnityEngine;
using System.Collections;

public class BandSawPieceController : MonoBehaviour 
{
    public bool Moveable = true;
    public Transform objTransform { get; set; }
    public WoodMaterialObject WoodObject { get; set; }
    public bool BeingCut { get; set; }
    public Vector3 RotationPoint { get; set; }

    private bool selected = false;
    private Vector3 previousPosition = Vector3.zero;

    void Start()
    {
        objTransform = GetComponent<Transform>();
        WoodObject = GetComponent<WoodMaterialObject>();
        BeingCut = false;
    }

    public void OnTouchStart(Gesture gesture)
    {
        if (gesture.pickedObject != null)
        {
            if (Moveable && WoodObject.ContainsPiece(gesture.pickedObject) && gesture.touchCount == 2)
            {
                selected = true;
            }
        }
    }

    public void OnDragStart(Gesture gesture)
    {
        if (Moveable && selected && gesture.touchCount == 2)
        {
            Vector3 position = gesture.GetTouchToWorldPoint(transform.position);
            previousPosition = position;
        }
    }

    public void MoveObject(Gesture gesture)
    {
        if (Moveable && selected && gesture.touchCount == 2)
        {
            Move(gesture);
        }
    }

    public void RotateAroundPoint(Gesture gesture)
    {
        if (Moveable && selected && gesture.touchCount == 2)
        {
            Vector3 axis = Vector3.up;
            if (BeingCut)
            {
                transform.RotateAround(RotationPoint, axis, -gesture.twistAngle);
            }
            else
            {
                Vector3 position = gesture.GetTouchToWorldPoint(transform.position);
                transform.RotateAround(position, axis, -gesture.twistAngle);
            }
        }
    }

    public void OnDragRelease(Gesture gesture)
    {
        selected = false;
        previousPosition = Vector3.zero;
    }

    private void Move(Gesture gesture)
    {
        Vector3 position = gesture.GetTouchToWorldPoint(transform.position);
        Vector3 nextPosition = position - previousPosition;
        if (BeingCut)
        {
            nextPosition = new Vector3(0.0f, 0.0f, nextPosition.z);
        }
        else
        {
            nextPosition = new Vector3(nextPosition.x, 0.0f, nextPosition.z);
        }
        objTransform.position += nextPosition;
        previousPosition = position;
    }



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
        EasyTouch.On_TouchStart2Fingers += OnTouchStart;

        EasyTouch.On_DragStart2Fingers += OnDragStart;

        EasyTouch.On_Drag2Fingers += MoveObject;

        EasyTouch.On_Twist += RotateAroundPoint;
        EasyTouch.On_DragEnd2Fingers += OnDragRelease;
        EasyTouch.On_TwistEnd += OnDragRelease;
    }

    private void UnsubscribeAll()
    {
        EasyTouch.On_TouchStart2Fingers -= OnTouchStart;

        EasyTouch.On_DragStart2Fingers -= OnDragStart;

        EasyTouch.On_Drag2Fingers -= MoveObject;

        EasyTouch.On_Twist -= RotateAroundPoint;
        EasyTouch.On_DragEnd2Fingers -= OnDragRelease;
        EasyTouch.On_TwistEnd -= OnDragRelease;
    }
}


//if (objRigidbody.position.x > MaxLimit_X || objRigidbody.position.x < MinLimit_X)
//{
//    float x = objRigidbody.position.x;
//    objRigidbody.position = new Vector3(Mathf.Clamp(x, MinLimit_X, MaxLimit_X), objRigidbody.position.y, objRigidbody.position.z);
//}
//if (objRigidbody.position.z > MaxLimit_Z || objRigidbody.position.z < MinLimit_Z)
//{
//    float z = objRigidbody.position.z;
//    objRigidbody.position = new Vector3(objRigidbody.position.x, objRigidbody.position.y, Mathf.Clamp(z, MinLimit_Z, MaxLimit_Z));
//}
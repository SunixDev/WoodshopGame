using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoardController : MonoBehaviour
{
    public bool Moveable;
    public bool RestrictX;
    public bool RestrictZ;
    public Rigidbody objRigidbody { get; set; }
    public Transform objTransform { get; set; }
    public WoodMaterialObject WoodObject { get; set; }
    public float MaxLimit_X;
    public float MinLimit_X;
    public float MaxLimit_Z;
    public float MinLimit_Z;
    public bool UseRigidbody = true;

    private bool selected = false;

    private Vector3 previousPosition = Vector3.zero;
    private Vector3 rotationPoint = Vector3.zero;

    void Start()
    {
        objRigidbody = GetComponent<Rigidbody>();
        if (objRigidbody == null && UseRigidbody)
        {
            objRigidbody = gameObject.AddComponent<Rigidbody>();
            objRigidbody.useGravity = true;
        }
        objTransform = GetComponent<Transform>();
        WoodObject = GetComponent<WoodMaterialObject>();
    }

    public void OnTouchStart(Gesture gesture)
    {
        if (gesture.pickedObject != null && gesture.touchCount == 1)
        {
            if (Moveable && WoodObject.ContainsPiece(gesture.pickedObject))
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
        rotationPoint = Vector3.zero;
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

    public void RotateAroundPoint(Gesture gesture)
    {
        if (Moveable && selected && gesture.touchCount == 2)
        {
            if (rotationPoint == Vector3.zero)
            {
                Vector3 position = gesture.GetTouchToWorldPoint(transform.position);
                rotationPoint = position;
            }
            Vector3 axis = Vector3.up;
            transform.RotateAround(rotationPoint, axis, -gesture.twistAngle);
        }
    }

    public void ResetRotationWithDoubleTap(Gesture gesture)
    {
        if (Moveable && selected)
        {
            ResetRotation();
            selected = false;
        }
    }

    public void ResetRotation()
    {
        transform.rotation = Quaternion.identity;
    }

    public void ApplyRotation(Vector3 axis, float angle)
    {
        transform.Rotate(axis, angle, Space.World);
    }

    public void ChangeOrientation()
    {
        transform.position = transform.position + new Vector3(0.0f, 5.0f, 0.0f);
        transform.Rotate(new Vector3(0.0f, 0.0f, 1.0f), 90.0f, Space.Self);

        Ray ray = new Ray(transform.position, -Vector3.up);
        RaycastHit hitOntoTable;
        int layermask  = 1 << 9;
        if (Physics.Raycast(ray, out hitOntoTable, 10.0f, layermask))
        {
            Debug.Log("hitOntoTable: " + hitOntoTable.collider.gameObject);
            ray = new Ray(hitOntoTable.point, Vector3.up);
            RaycastHit hitOntoWood;
            if (Physics.Raycast(ray, out hitOntoWood))
            {
                Debug.Log("hitOntoWood: " + hitOntoWood.collider.gameObject);
                Vector3 displacement = hitOntoTable.point - hitOntoWood.point;
                transform.position = transform.position + displacement;
            }
        }
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
        Vector3 position = gesture.GetTouchToWorldPoint(transform.position);
        Vector3 nextPosition = position - previousPosition;
        previousPosition = position;
        nextPosition = DetermineRestrictions(nextPosition);
        if (UseRigidbody)
        {
            objRigidbody.position += nextPosition;
            if (objRigidbody.position.x > MaxLimit_X || objRigidbody.position.x < MinLimit_X)
            {
                float x = objRigidbody.position.x;
                objRigidbody.position = new Vector3(Mathf.Clamp(x, MinLimit_X, MaxLimit_X), objRigidbody.position.y, objRigidbody.position.z);
            }
            if (objRigidbody.position.z > MaxLimit_Z || objRigidbody.position.z < MinLimit_Z)
            {
                float z = objRigidbody.position.z;
                objRigidbody.position = new Vector3(objRigidbody.position.x, objRigidbody.position.y, Mathf.Clamp(z, MinLimit_Z, MaxLimit_Z));
            }
        }
        else
        {
            objTransform.position += nextPosition;
            if (objTransform.position.x > MaxLimit_X || objTransform.position.x < MinLimit_X)
            {
                float x = objTransform.position.x;
                objTransform.position = new Vector3(Mathf.Clamp(x, MinLimit_X, MaxLimit_X), objTransform.position.y, objTransform.position.z);
            }
            if (objTransform.position.z > MaxLimit_Z || objTransform.position.z < MinLimit_Z)
            {
                float z = objTransform.position.z;
                objTransform.position = new Vector3(objTransform.position.x, objTransform.position.y, Mathf.Clamp(z, MinLimit_Z, MaxLimit_Z));
            }
        }
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
        EasyTouch.On_DoubleTap += ResetRotationWithDoubleTap;

        EasyTouch.On_DragStart += OnDragStart;

        EasyTouch.On_Drag += MoveObject_SingleFingerTouch;

        EasyTouch.On_Twist += RotateAroundPoint;
        EasyTouch.On_DragEnd += OnDragRelease;
        EasyTouch.On_TwistEnd += OnDragRelease;
    }

    private void UnsubscribeAll()
    {
        EasyTouch.On_TouchStart -= OnTouchStart;
        EasyTouch.On_DoubleTap -= ResetRotationWithDoubleTap;

        EasyTouch.On_DragStart -= OnDragStart;

        EasyTouch.On_Drag -= MoveObject_SingleFingerTouch;

        EasyTouch.On_Twist -= RotateAroundPoint;
        EasyTouch.On_DragEnd -= OnDragRelease;
        EasyTouch.On_TwistEnd -= OnDragRelease;
    }
#endregion

}






//private Vector3 GetSwipeDirectionVector(EasyTouch.SwipeDirection swipe)
//{
//    Vector3 direction = new Vector3();
//    if (swipe == EasyTouch.SwipeDirection.Up || swipe == EasyTouch.SwipeDirection.Down)
//    {
//        directionSelected = true;
//        direction = Vector3.forward;
//    }
//    else if (swipe == EasyTouch.SwipeDirection.Left || swipe == EasyTouch.SwipeDirection.Right)
//    {
//        directionSelected = true;
//        direction = Vector3.right;
//    }

//    return direction;
//}


//float x = nextPosition.x; //(direction.x > 0.0f) ? nextPosition.x : objRigidbody.position.x;
//float y = objRigidbody.position.y;
//float z = nextPosition.z; //(direction.z > 0.0f) ? nextPosition.z : objRigidbody.position.z;
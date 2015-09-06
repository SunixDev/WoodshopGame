using UnityEngine;
using System.Collections;

public class BandSawPieceController : MonoBehaviour 
{
    public enum ControlType
    {
        Freeform,
        Slide
    }

    public bool Moveable;
    public Transform RotationPoint;
    public bool PointRotation;
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





    //void OnEnable()
    //{
    //    EasyTouch.On_TouchDown += OnTouchStart;
    //    EasyTouch.On_DragStart += OnDragStart;
    //    EasyTouch.On_Drag += MoveObject;
    //    EasyTouch.On_Twist += RotateAroundPoint;
    //    EasyTouch.On_DragEnd += OnDragRelease;
    //}

    //void OnDisable()
    //{
    //    EasyTouch.On_TouchDown -= OnTouchStart;
    //    EasyTouch.On_DragStart -= OnDragStart;
    //    EasyTouch.On_Drag -= MoveObject;
    //    EasyTouch.On_Twist -= RotateAroundPoint;
    //    EasyTouch.On_DragEnd -= OnDragRelease;
    //}

    //void OnDestroy()
    //{
    //    EasyTouch.On_TouchDown -= OnTouchStart;
    //    EasyTouch.On_DragStart -= OnDragStart;
    //    EasyTouch.On_Drag -= MoveObject;
    //    EasyTouch.On_Twist -= RotateAroundPoint;
    //    EasyTouch.On_DragEnd -= OnDragRelease;
    //}
}

using UnityEngine;
using System.Collections;

public class PieceController : MonoBehaviour 
{
    public bool Moveable;
    [Range(0.0f, 1.0f)]
    public float ActionTimeToDrag = 0.2f;
    public static float RotationSpeed = 250.0f;

    private Transform objTransform;
    private bool Selected;
    private bool Rotating;
    private Vector3 rotationAxis;
    private float totalRotation;

    void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        Moveable = true;
        Selected = false;
        Rotating = false;
        objTransform = transform;
        totalRotation = 0.0f;
    }

    void Update()
    {
        float deltaTime = Time.deltaTime;
        if (Rotating)
        {
            totalRotation += RotationSpeed * deltaTime;
            if (totalRotation >= 90.0f)
            {
                totalRotation -= RotationSpeed * deltaTime;
                float remainingRotation = 90.0f - totalRotation;
                objTransform.Rotate(rotationAxis, remainingRotation, Space.World);
                totalRotation = 0.0f;
                Rotating = false;
            }
            else
            {
                objTransform.Rotate(rotationAxis, RotationSpeed * Time.deltaTime, Space.World);
            }

        }
    }

    public void OnTouch(Gesture gesture)
    {
        Selected = (gesture.pickedObject == gameObject);
    }

    public void MovePiece(Gesture gesture)
    {
        if (Moveable && Selected && !Rotating && gesture.pickedObject == gameObject && gesture.touchCount == 1 && gesture.actionTime >= ActionTimeToDrag)
        {
            objTransform.position = gesture.GetTouchToWorldPoint(objTransform.position);
        }
    }

    public void PlayerRotation(Gesture gesture)
    {
        if (Moveable && Selected && !Rotating && gesture.pickedObject == gameObject && gesture.touchCount == 1 && gesture.actionTime < ActionTimeToDrag)
        {
            rotationAxis = GetRotationAxis(gesture.swipe);
            Rotating = true;
        }
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
            axis = Vector3.left;
        }
        else if (direction == EasyTouch.SwipeDirection.Left)
        {
            axis = Vector3.up;
        }
        else if (direction == EasyTouch.SwipeDirection.Right)
        {
            axis = Vector3.down;
        }

        return axis;
    }

    private float ClampAngle(float amount)
    {
        float fullRotation = 360.0f;
        if (amount < 0.0f) 
            amount += fullRotation;
        if (amount > fullRotation) 
            amount -= fullRotation;
        return Mathf.Clamp(amount, 0, 360);
    }





    private void EnableTouchEvents()
    {
        EasyTouch.On_Drag += MovePiece;
        EasyTouch.On_TouchStart += OnTouch;
        EasyTouch.On_DragEnd += PlayerRotation;
    }

    private void DisableTouchEvents()
    {
        EasyTouch.On_Drag -= MovePiece;
        EasyTouch.On_TouchStart -= OnTouch;
        EasyTouch.On_DragEnd -= PlayerRotation;
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

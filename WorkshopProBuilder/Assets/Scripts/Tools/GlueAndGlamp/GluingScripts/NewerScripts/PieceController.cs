using UnityEngine;
using System.Collections;

public enum PieceControlState
{
    Drag,
    Rotate
}

public class PieceController : MonoBehaviour 
{
    public float RotationSpeed = 5f;
    public bool isMoving { get; private set; }
    public PieceControlState state = PieceControlState.Drag;

    private Transform pieceTransform;
    private Vector3 offset;

    void Start()
    {
        pieceTransform = transform;
        isMoving = false;
    }

    public void OnPieceTouched(Gesture gesture)
    {
        if (gesture.pickedObject == gameObject && gesture.touchCount == 1 && !gesture.isOverGui && !gesture.IsOverUIElement())
        {
            if (state == PieceControlState.Drag)
            {
                offset = pieceTransform.position - gesture.GetTouchToWorldPoint(pieceTransform.position);
            }
            isMoving = true;
        }
    }

    public void MovePiece(Gesture gesture)
    {
        if (gesture.pickedObject == gameObject && gesture.touchCount == 1 && isMoving && state == PieceControlState.Drag && !gesture.isOverGui && !gesture.IsOverUIElement())
        {
            pieceTransform.position = gesture.GetTouchToWorldPoint(pieceTransform.position) + offset;
        }
    }

    public void RotatePiece(Gesture gesture)
    {
        if (gesture.pickedObject == gameObject && gesture.touchCount == 1 && isMoving && state == PieceControlState.Rotate && !gesture.isOverGui && !gesture.IsOverUIElement())
        {
            float xRotation = gesture.deltaPosition.x * RotationSpeed * 0.1f;
            float yRotation = gesture.deltaPosition.y * RotationSpeed * 0.1f;
            pieceTransform.Rotate(yRotation, -xRotation, 0f, Space.World);
        }
    }

    public void OnPieceReleased(Gesture gesture)
    {
        if (gesture.pickedObject == gameObject && gesture.touchCount == 1 && isMoving)
        {
            isMoving = false;
        }
    }





    private void EnableTouchEvents()
    {
        EasyTouch.On_TouchStart += OnPieceTouched;
        EasyTouch.On_TouchUp += OnPieceReleased;
        EasyTouch.On_Drag += MovePiece;
        EasyTouch.On_Drag += RotatePiece;
    }

    private void DisableTouchEvents()
    {
        EasyTouch.On_TouchStart -= OnPieceTouched;
        EasyTouch.On_TouchUp -= OnPieceReleased;
        EasyTouch.On_Drag -= MovePiece;
        EasyTouch.On_Drag -= RotatePiece;
    }

    void OnEnable()
    {
        EnableTouchEvents();
    }

    void OnDisable()
    {
        DisableTouchEvents();
    }

    void OnDestroy()
    {
        DisableTouchEvents();
    }
}

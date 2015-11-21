using UnityEngine;
using System.Collections;

public class GluedPieceController : MonoBehaviour 
{
    public float RotationSpeed = 5.0f;

    private Transform objTransform;

    void Awake()
    {
        objTransform = transform;
    }

    public void RotatePiece(Gesture gesture)
    {
        if (gesture.pickedObject != null && gesture.touchCount == 1)
        {
            if (gesture.pickedObject.tag == "Piece" || gesture.pickedObject.tag == "WoodProject")
            {
                float yRotation = gesture.deltaPosition.x * RotationSpeed * 0.1f;
                objTransform.Rotate(0f, -yRotation, 0.0f, Space.World);
            }
        }
    }



    private void EnableTouchEvents()
    {
        EasyTouch.On_Drag += RotatePiece;
    }

    private void DisableTouchEvents()
    {
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
    void OnDestory()
    {
        DisableTouchEvents();
    }
}

//private float ClampAngle(float amount)
//{
//    float fullRotation = 360.0f;
//    if (amount < 0.0f)
//        amount += fullRotation;
//    if (amount > fullRotation)
//        amount -= fullRotation;
//    return Mathf.Clamp(amount, 0, 360);
//}
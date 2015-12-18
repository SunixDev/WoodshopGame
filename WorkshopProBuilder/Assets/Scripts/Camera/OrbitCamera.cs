using UnityEngine;
using System.Collections;

public class OrbitCamera : MonoBehaviour 
{
    [Header("Distance Variables")]
    public Transform LookAtPoint;
    public float Distance = 2f;
    public float MinDistance = 0.5f;
    public float MaxDistance = 5f;

    [Header("Initial Rotation")]
    public float Vertical = 0f;
    public float Horizontal = 0f;

    [Header("Speed Input")]
    [Range(1f, 5f)]
    public float SensitivityX = 2f;
    [Range(1f, 5f)]
    public float SensitivityY = 2f;
    [Range(0.1f, 5f)]
    public float ZoomSensitivity = 1f;

    [Header("Vertical Rotation Clamp")]
    public float MinRotationY = 1f;
    public float MaxRotationY = 179f;

    [Header("Horizontal Rotation Clamp")]
    public float MinRotationX = -180f;
    public float MaxRotationX = 180f;

    [Header("Movement Toggle")]
    public bool EnableZoom = true;
    public bool EnableOrbit = true;
    public bool EnableCameraControl = true;
    public bool EnableLookAt = true;

    private float xMovement;
    private float yMovement;

    void Awake()
    {
        xMovement = Vertical;
        yMovement = Horizontal;
    }

    void Update()
    {
        if (EnableCameraControl)
        {
            float finalDistance = Mathf.Clamp(Distance, MinDistance, MaxDistance);
            Vector3 direction = new Vector3(0.0f, 0.0f, -finalDistance);
            Quaternion rotation = Quaternion.Euler(yMovement, xMovement, 0.0f);
            Vector3 finalPosition = LookAtPoint.position + (rotation * direction);
            //transform.position = Vector3.MoveTowards(transform.position, finalPosition, 0.1f);
            transform.position = finalPosition;
        }
        if (EnableLookAt)
        {
            transform.LookAt(LookAtPoint.position);
        }
    }

    public void MoveCamera(Gesture gesture)
    {
        if (gesture.touchCount == 1 && EnableOrbit && EnableCameraControl && gesture.pickedObject == null && gesture.pickedUIElement == null && !gesture.IsOverUIElement())
        {
            xMovement += gesture.deltaPosition.x * SensitivityX * 0.1f;
            xMovement = ClampAngle(xMovement, MinRotationX, MaxRotationX);

            yMovement -= gesture.deltaPosition.y * SensitivityY * 0.1f;
            yMovement = ClampAngle(yMovement, MinRotationY, MaxRotationY);
        }
    }

    public void ZoomOut(Gesture gesture)
    {
        if (gesture.touchCount == 2 && EnableZoom && EnableCameraControl && gesture.pickedObject == null && gesture.pickedUIElement == null && !gesture.IsOverUIElement())
        {
            float zoomAmount = gesture.deltaPinch * ZoomSensitivity * 0.01f;
            Distance += zoomAmount;
            Distance = Mathf.Clamp(Distance, MinDistance, MaxDistance);
        }
    }

    public void ZoomIn(Gesture gesture)
    {
        if (gesture.touchCount == 2 && EnableZoom && EnableCameraControl && gesture.pickedObject == null && gesture.pickedUIElement == null && !gesture.IsOverUIElement())
        {
            float zoomAmount = gesture.deltaPinch * ZoomSensitivity * 0.01f;
            Distance -= zoomAmount;
            Distance = Mathf.Clamp(Distance, MinDistance, MaxDistance);
        }
    }

    private float ClampAngle(float amount, float min, float max)
    {
        float fullRotation = 360;
        if (amount < -fullRotation) amount += fullRotation;
        if (amount > fullRotation) amount -= fullRotation;
        return Mathf.Clamp(amount, min, max);
    }

    public void ChangeAngle(float v, float h)
    {
        xMovement = v;
        yMovement = h;
    }

    public float GetVerticalAngle()
    {
        return xMovement;
    }

    public float GetHorizontalAngle()
    {
        return yMovement;
    }


    private void SubscribeEvents()
    {
        //Orbit Controls
        EasyTouch.On_Swipe += MoveCamera;

        //Zoom Controls
        EasyTouch.On_PinchIn += ZoomOut;
        EasyTouch.On_PinchOut += ZoomIn;
    }

    private void UnsubscribeEvents()
    {
        //Orbit Controls
        EasyTouch.On_Swipe -= MoveCamera;

        //Zoom Controls
        EasyTouch.On_PinchIn -= ZoomOut;
        EasyTouch.On_PinchOut -= ZoomIn;
    }

    void OnEnable()
    {
        SubscribeEvents();
    }
    void OnDisable()
    {
        UnsubscribeEvents();
    }
    void OnDestroy()
    {
        UnsubscribeEvents();
    }
}

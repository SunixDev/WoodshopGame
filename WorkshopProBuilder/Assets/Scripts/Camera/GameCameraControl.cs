using UnityEngine;
using System.Collections;

public class GameCameraControl : MonoBehaviour 
{
    [Header("Distance Variables")]
    public Transform LookAtPoint;
    public float Distance = 2.0f;
    public float MinDistance = 0.5f;
    public float MaxDistance = 5.0f;

    [Header("Initial Rotation")]
    public float Vertical = 90;
    public float Horizontal = 90;

    [Header("Speed Input")]
    [Range(1.0f, 5.0f)]
    public float SensitivityX = 2.0f;
    [Range(1.0f, 5.0f)]
    public float SensitivityY = 2.0f;
    [Range(0.1f, 5.0f)]
    public float ZoomSensitivity = 1.0f;

    [Header("Vertical Rotation Clamp")]
    public float MinRotationY = 1.0f;
    public float MaxRotationY = 179.0f;

    [Header("Horizontal Rotation Clamp")]
    public float MinRotationX = 0.0f;
    public float MaxRotationX = 180.0f;

    private float xMovement;
    private float yMovement;

    void Awake()
    {
        xMovement = Vertical;
        yMovement = Horizontal;
    }

    void Update()
    {
        float finalDistance = Mathf.Clamp(Distance, MinDistance, MaxDistance);
        Vector3 direction = new Vector3(0.0f, 0.0f, -finalDistance);
        Quaternion rotation = Quaternion.Euler(yMovement, xMovement, 0.0f);
        Vector3 finalPosition = LookAtPoint.position + (rotation * direction);
        //transform.position = Vector3.MoveTowards(transform.position, finalPosition, 0.1f);
        transform.position = finalPosition;
        transform.LookAt(LookAtPoint.position);
    }

    public void OrbitCamera(Gesture gesture)
    {
        if (gesture.touchCount == 1 && gesture.pickedObject == null)
        {
            xMovement += gesture.deltaPosition.x * SensitivityX * 0.1f;
            xMovement = ClampAngle(xMovement, MinRotationX, MaxRotationX);

            yMovement -= gesture.deltaPosition.y * SensitivityY * 0.1f;
            yMovement = ClampAngle(yMovement, MinRotationY, MaxRotationY);
        }
    }

    public void ZoomOut(Gesture gesture)
    {
        if (gesture.touchCount == 2)
        {
            float zoomAmount = gesture.deltaPinch * ZoomSensitivity * 0.1f;
            Distance += zoomAmount;
            Distance = Mathf.Clamp(Distance, MinDistance, MaxDistance);
        }
    }

    public void ZoomIn(Gesture gesture)
    {
        if (gesture.touchCount == 2)
        {
            float zoomAmount = gesture.deltaPinch * ZoomSensitivity * 0.1f;
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



    private void SubscribeEvents()
    {
        //Orbit Controls
        EasyTouch.On_Swipe += OrbitCamera;

        //Zoom Controls
        EasyTouch.On_PinchIn += ZoomOut;
        EasyTouch.On_PinchOut += ZoomIn;
    }

    private void UnsubscribeEvents()
    {
        //Orbit Controls
        EasyTouch.On_Swipe -= OrbitCamera;

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

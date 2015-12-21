using UnityEngine;
using System.Collections;

public enum ZoomType
{
    ZoomToLookAt,
    MoveLookAt
}

public enum PanType
{
    XY_Plane,
    XZ_Plane
}


public enum CameraType
{
    FirstPerson,
    Overlook
}

public class CameraControl : MonoBehaviour 
{
    public bool MovementEnabled = true;

    [Header("Distance Variables")]
    public Transform LookAtPoint;
    public float DistanceFromPoint = 2.0f;
    public float MinDistance = 0.5f;
    public float MaxDistance = 5.0f;
    //public PanType PanningType = PanType.XZ_Plane;
    //public ZoomType ZoomInType = ZoomType.ZoomToLookAt;
    public CameraType type = CameraType.Overlook;
    public PanType panningType = PanType.XZ_Plane;

    [Header("Speed Input")]
    [Range(5.0f, 20.0f)]
    public float SensitivityX = 6.0f;
    [Range(5.0f, 20.0f)]
    public float SensitivityY = 6.0f;
    [Range(0.1f, 5.0f)]
    public float ZoomSensitivity = 1.0f;
    [Range(0.1f, 5.0f)]
    public float PanSensitivity = 2.0f;

    [Header("Vertical Rotation Clamp")]
    public float yMinRotation = 10.0f;
    public float yMaxRotation = 80.0f;

    [Header("Initial Rotation")]
    public float Vertical = 90;
    public float Horizontal = 90;

    [Header("Movement Toogle")]
    public bool AllowRotation = true;
    public bool AllowPanning = true;
    public bool AllowZooming = true;

    private float xMovement;
    private float yMovement;
    private Vector3 lookAtPointOffset = Vector3.zero;
    private Vector2 previousFingerPosition = Vector2.zero;
    private Vector3 previousCameraPosition = Vector3.zero;
    private bool canPan = true;
    private bool canOrbit = true;
    private bool canZoom = true;

	void Start () 
    {
        xMovement = Vertical;
        yMovement = Horizontal;
        previousCameraPosition = transform.position;
        yMovement = ClampAngle(yMovement);
	}

    void Update()
    {
        if (type == CameraType.Overlook)
        {
            float distance = DistanceFromPoint;
            distance = Mathf.Clamp(distance, MinDistance, MaxDistance);
            Vector3 distanceVector = new Vector3(0.0f, 0.0f, -distance);
            Quaternion rotation = Quaternion.Euler(yMovement, xMovement, 0.0f);
            Vector3 finalPosition = (LookAtPoint.position + lookAtPointOffset) + (rotation * distanceVector);

            //transform.position = Vector3.MoveTowards(transform.position, finalPosition, 0.1f);
            transform.position = finalPosition;
            transform.LookAt(LookAtPoint.position + lookAtPointOffset);
            if (previousCameraPosition == transform.position)
            {
                ResetMovementOptions(null);
            }
            previousCameraPosition = transform.position;
        }
    }

    public void ResetMovementOptions(Gesture gesture)
    {
        canPan = true;
        canZoom = true;
        canOrbit = true;
    }



    public void StartCameraOrbit(Gesture gesture)
    {
        if (MovementEnabled && gesture.touchCount == 1 && AllowRotation && canOrbit)// && ValidTag(tag))
        {
            canPan = false;
            canZoom = false;
            canOrbit = true;
        }
    }

    public void OrbitCamera(Gesture gesture)
    {
        if (MovementEnabled && gesture.touchCount == 1 && AllowRotation && canOrbit)
        {
            if (type == CameraType.Overlook)
            {
                xMovement += gesture.deltaPosition.x * (SensitivityX / 2.0f) * Time.deltaTime;
                yMovement -= gesture.deltaPosition.y * (SensitivityY / 2.0f) * Time.deltaTime;
                yMovement = ClampAngle(yMovement);
            }
            else
            {
                float yRotation = gesture.deltaPosition.x * (SensitivityX / 2.0f) * Time.deltaTime;
                float xRotation = -gesture.deltaPosition.y * (SensitivityY / 2.0f) * Time.deltaTime;
                transform.Rotate(0.0f, yRotation, 0.0f, Space.World);
                transform.Rotate(xRotation, 0.0f, 0.0f, Space.Self);
            }
        }
    }



    public void StartCameraZoom(Gesture gesture)
    {
        if (MovementEnabled && gesture.touchCount == 2 && canZoom && AllowZooming)
        {
            canPan = false;
            canZoom = true;
            canOrbit = false;
        }
    }

    public void ZoomAwayFromPoint(Gesture gesture)
    {
        if (MovementEnabled && gesture.touchCount == 2 && canZoom && AllowZooming)
        {
            float zoomAmount = gesture.deltaPinch * (ZoomSensitivity / 10.0f) * Time.deltaTime;
            if (type == CameraType.Overlook)
            {
                DistanceFromPoint += zoomAmount;
                DistanceFromPoint = Mathf.Clamp(DistanceFromPoint, MinDistance, MaxDistance);
            }
            else
            {
                Vector3 movement = transform.forward.normalized * zoomAmount;
                transform.position -= movement;
            }
        }
    }

    public void ZoomTowardsPoint(Gesture gesture)
    {
        if (MovementEnabled && gesture.touchCount == 2 && canZoom && AllowZooming)
        {
            float zoomAmount = gesture.deltaPinch * (ZoomSensitivity / 10.0f) * Time.deltaTime;
            if (type == CameraType.Overlook)
            {
                DistanceFromPoint -= zoomAmount;
                DistanceFromPoint = Mathf.Clamp(DistanceFromPoint, MinDistance, MaxDistance);
            }
            else
            {
                Vector3 movement = transform.forward.normalized * zoomAmount;
                transform.position += movement;
            }
        }
    }



    public void PanCamera(Gesture gesture)
    {
        if (MovementEnabled && gesture.touchCount == 2 && canPan && AllowPanning)
        {
            if (previousFingerPosition == Vector2.zero)
            {
                previousFingerPosition = gesture.position;
                canPan = true;
                canZoom = false;
                canOrbit = false;
            }
            else
            {
                Vector2 currentPosition = gesture.position;
                if (currentPosition != previousFingerPosition)
                {
                    Vector3 deltaPosition = gesture.deltaPosition * (PanSensitivity * Time.deltaTime);
                    if (panningType == PanType.XZ_Plane)
                    {
                        lookAtPointOffset += (Quaternion.Euler(new Vector3(0.0f, xMovement, 0.0f)) * new Vector3(-deltaPosition.x, 0.0f, -deltaPosition.y));
                    }
                    else
                    {
                        Vector3 movement = transform.rotation * new Vector3(-deltaPosition.x, -deltaPosition.y, 0.0f);
                        lookAtPointOffset += movement;
                    }
                    previousFingerPosition = currentPosition;
                }
            }
        }
    }

    public void StopCameraPan(Gesture gesture)
    {
        if (MovementEnabled && gesture.touchCount == 2 && canPan && AllowPanning)
        {
            previousFingerPosition = Vector2.zero;
            lookAtPointOffset = Vector3.zero;
            canPan = true;
            canZoom = false;
            canOrbit = false;
        }
    }

    public void EnableMovement(bool enable)
    {
        MovementEnabled = enable;
    }

    public void ChangeLookAtPoint(Transform point)
    {
        LookAtPoint = point;
        ResetLookAtOffset();
    }

    public void SnapToRotation(float vertical, float horizontal)
    {
        xMovement = vertical;
        yMovement = horizontal;
        yMovement = ClampAngle(yMovement);
    }

    private float ClampAngle(float amount)
    {
        float fullRotation = 360;
        if (amount < -fullRotation) amount += fullRotation;
        if (amount > fullRotation) amount -= fullRotation;
        return Mathf.Clamp(amount, yMinRotation, yMaxRotation);
    }

    public void ChangeDistanceVariables(float distance, float min, float max)
    {
        DistanceFromPoint = distance;
        MinDistance = min;
        MaxDistance = max;
    }

    public void ChangeSensitivity(float xSensitivity, float ySensitivity, float zoomSensitivity)
    {
        SensitivityX = xSensitivity;
        SensitivityY = ySensitivity;
        ZoomSensitivity = zoomSensitivity;
    }

    public void ChangeVerticalRotationLimit(float yMin, float yMax)
    {
        yMinRotation = yMin;
        yMaxRotation = yMax;
    }

    public void ChangeAngle(float vertical, float horizontal)
    {
        xMovement = vertical;
        yMovement = horizontal;
        yMovement = ClampAngle(yMovement);
    }

    public void EnableRotation(bool enable)
    {
        AllowRotation = enable;
    }

    public void EnablePanning(bool enable)
    {
        AllowPanning = enable;
    }
    public void EnableZooming(bool enable)
    {
        AllowZooming = enable;
    }

    public void ResetLookAtOffset()
    {
        previousFingerPosition = Vector3.zero;
        lookAtPointOffset = Vector3.zero;
    }


    private bool ValidTag(string tag)
    {
        return (tag != "Piece" && tag != "Leftover" && tag != "WoodStrip" && tag != "Tool" && tag != "DadoBlock" && tag != "GlueBox");
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

    private void SubscribeEvents()
    {
        EasyTouch.On_TouchStart += StartCameraOrbit;
        EasyTouch.On_Swipe += OrbitCamera;
        EasyTouch.On_Drag += OrbitCamera;

        EasyTouch.On_Pinch += StartCameraZoom;
        EasyTouch.On_PinchIn += ZoomAwayFromPoint;
        EasyTouch.On_PinchOut += ZoomTowardsPoint;

        EasyTouch.On_Swipe2Fingers += PanCamera;
        EasyTouch.On_SwipeEnd2Fingers += ResetMovementOptions;

        EasyTouch.On_TouchUp += ResetMovementOptions;
        EasyTouch.On_TouchUp2Fingers += ResetMovementOptions;
    }

    private void UnsubscribeEvents()
    {
        EasyTouch.On_TouchStart -= StartCameraOrbit;
        EasyTouch.On_Swipe -= OrbitCamera;
        EasyTouch.On_Drag -= OrbitCamera;

        EasyTouch.On_Pinch -= StartCameraZoom;
        EasyTouch.On_PinchIn -= ZoomAwayFromPoint;
        EasyTouch.On_PinchOut -= ZoomTowardsPoint;
        EasyTouch.On_PinchEnd -= ResetMovementOptions;

        EasyTouch.On_Swipe2Fingers -= PanCamera;
        EasyTouch.On_SwipeEnd2Fingers -= ResetMovementOptions;

        EasyTouch.On_TouchUp -= ResetMovementOptions;
        EasyTouch.On_TouchUp2Fingers -= ResetMovementOptions;
    }
}

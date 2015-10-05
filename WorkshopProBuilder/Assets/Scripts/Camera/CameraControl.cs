using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour 
{
    public bool MovementEnabled = true;

    [Header("Distance Variables")]
    public Transform LookAtPoint;
    public float DistanceFromPoint = 2.0f;
    public float MinDistance = 0.5f;
    public float MaxDistance = 5.0f;

    [Header("Speed Input")]
    [Range(5.0f, 20.0f)]
    public float SensitivityX = 6.0f;
    [Range(5.0f, 20.0f)]
    public float SensitivityY = 6.0f;
    [Range(0.1f, 5.0f)]
    public float ZoomSensitivity = 1.0f;

    [Header("Vertical Rotation Clamp")]
    public float yMinRotation = 10.0f;
    public float yMaxRotation = 80.0f;

    [Header("Initial Rotation")]
    public float Vertical = 90;
    public float Horizontal = 90;
    public bool AllowRotation = true;

    private float xMovement;
    private float yMovement;
    private Vector3 panMovement = Vector3.zero;
    private Vector3 previousFingerPosition = Vector3.zero;
    private Vector3 previousCameraPosition = Vector3.zero;
    private bool canPan = true;
    private bool canOrbit = true;
    private bool canZoom = true;

	void Start () 
    {
        xMovement = Vertical;
        yMovement = Horizontal;
        previousCameraPosition = transform.position;
	}

    void Update()
    {
        float distance = DistanceFromPoint;
        distance = Mathf.Clamp(distance, MinDistance, MaxDistance);
        Vector3 distanceVector = new Vector3(0.0f, 0.0f, -distance);
        Quaternion rotation = Quaternion.Euler(yMovement, xMovement, 0.0f);
        Vector3 finalPosition = (LookAtPoint.position) + (rotation * distanceVector);

        //transform.position = Vector3.MoveTowards(transform.position, finalPosition, 0.1f);
        transform.position = finalPosition;
        transform.LookAt(LookAtPoint.position);
        if (previousCameraPosition == transform.position)
        {
            ResetMovementOptions(null);
        }
        else
        {
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
        //string tag = (gesture.pickedObject == null) ? "Untagged" : gesture.pickedObject.tag;
        if (MovementEnabled && gesture.touchCount == 1 && AllowRotation && canOrbit)// && ValidTag(tag))
        {
            canPan = false;
            canZoom = false;
            canOrbit = true;
        }
    }

    public void OrbitCamera(Gesture gesture)
    {
        //string tag = (gesture.pickedObject == null) ? "Untagged" : gesture.pickedObject.tag;
        if (MovementEnabled && gesture.touchCount == 1 && AllowRotation && canOrbit)// && ValidTag(tag))
        {
            xMovement += gesture.deltaPosition.x * (SensitivityX / 2.0f) * Time.deltaTime;
            yMovement -= gesture.deltaPosition.y * (SensitivityY / 2.0f) * Time.deltaTime;
            yMovement = ClampAngle(yMovement);
        }
    }



    public void StartCameraZoom(Gesture gesture)
    {
        //string tag = (gesture.pickedObject == null) ? "Untagged" : gesture.pickedObject.tag;
        if (MovementEnabled && gesture.touchCount == 2 && canZoom)// && ValidTag(tag))
        {
            canPan = false;
            canZoom = true;
            canOrbit = false;
        }
    }

    public void ZoomAwayFromPoint(Gesture gesture)
    {
        //string tag = (gesture.pickedObject == null) ? "Untagged" : gesture.pickedObject.tag;
        if (MovementEnabled && gesture.touchCount == 2 && canZoom)// && ValidTag(tag))
        {
            float zoomAmount = gesture.deltaPinch * (ZoomSensitivity / 10.0f) * Time.deltaTime;
            DistanceFromPoint += zoomAmount;
            DistanceFromPoint = Mathf.Clamp(DistanceFromPoint, MinDistance, MaxDistance);
        }
    }

    public void ZoomTowardsPoint(Gesture gesture)
    {
        //string tag = (gesture.pickedObject == null) ? "Untagged" : gesture.pickedObject.tag;
        if (MovementEnabled && gesture.touchCount == 2 && canZoom)// && ValidTag(tag))
        {
            float zoomAmount = gesture.deltaPinch * (ZoomSensitivity / 10.0f) * Time.deltaTime;
            DistanceFromPoint -= zoomAmount;
            DistanceFromPoint = Mathf.Clamp(DistanceFromPoint, MinDistance, MaxDistance);
        }
    }



    public void PanCamera(Gesture gesture)
    {
        if (MovementEnabled && gesture.touchCount == 2 && canPan)// && ValidTag(tag))
        {
            if (previousFingerPosition == Vector3.zero)
            {
                previousFingerPosition = gesture.deltaPosition;// + panMovement);
                canPan = true;
                canZoom = false;
                canOrbit = false;
            }
            else
            {
                Vector3 currentPosition = gesture.deltaPosition;// + panMovement);
                if (currentPosition != previousFingerPosition)
                {
                    Vector3 deltaPosition = currentPosition - previousFingerPosition;
                    previousFingerPosition = currentPosition;
                    LookAtPoint.position += new Vector3(deltaPosition.x, 0.0f, deltaPosition.z);
                }
            }
        }
    }

    public void StopCameraPan(Gesture gesture)
    {
        if (MovementEnabled && gesture.touchCount == 2 && canPan)// && ValidTag(tag))
        {
            previousFingerPosition = Vector3.zero;
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
        previousFingerPosition = Vector3.zero;
        panMovement = Vector3.zero;
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

    public void SetupCamera(CameraPositioning setup)
    {
        DistanceFromPoint = setup.DistanceFromPoint;
        MinDistance = setup.MinDistance;
        MaxDistance = setup.MaxDistance;
        yMinRotation = setup.yMinRotation;
        yMaxRotation = setup.yMaxRotation;
        xMovement = setup.Vertical;
        yMovement = setup.Horizontal;
        yMovement = ClampAngle(yMovement);
        AllowRotation = setup.AllowRotation;
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

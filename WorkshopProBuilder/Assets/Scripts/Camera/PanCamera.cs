using UnityEngine;
using System.Collections;

public enum PanDirection
{
    XY_Plane,
    XZ_Plane
}

[System.Serializable]
public class CameraBoundary
{
    public float MaxVerticalBounds;
    public float MinVerticalBounds;
    public float MaxHorizontalBounds;
    public float MinHorizontalBounds;

    public void ApplyBounds(float bounds)
    {
        MaxVerticalBounds = bounds;
        MinVerticalBounds = -bounds;
        MaxHorizontalBounds = bounds;
        MinHorizontalBounds = -bounds;
    }

    public void ApplyVerticalBounds(float bounds)
    {
        if (bounds > 0)
        {
            MaxVerticalBounds = bounds;
            MinVerticalBounds = -bounds;
        }
        else
        {
            Debug.Log("Cannot apply negative vertical boundaries to camera");
        }
    }

    public void ApplyVerticalBounds(float min, float max)
    {
        if (max <= min)
        {
            Debug.LogError("The vertical max is less than the minimum");
        }
        else
        {
            MaxVerticalBounds = max;
            MinVerticalBounds = min;
        }
    }

    public void ApplyHorizontalBounds(float bounds)
    {
        if (bounds > 0)
        {
            MaxHorizontalBounds = bounds;
            MinHorizontalBounds = -bounds;
        }
        else
        {
            Debug.Log("Cannot apply negative horizontal boundaries to camera");
        }
    }

    public void ApplyHorizontalBounds(float min, float max)
    {
        if (max <= min)
        {
            Debug.LogError("The horizontal max is less than the minimum");
        }
        else
        {
            MaxHorizontalBounds = max;
            MinHorizontalBounds = min;
        }
    }
}

public class PanCamera : MonoBehaviour
{
    [Header("Distance Variables")]
    public Transform LookAtPoint;
    public float Distance = 2f;
    public float MinDistance = 0.5f;
    public float MaxDistance = 5f;
    public bool BoundaryEnabled = false;
    public CameraBoundary bounds;

    [Header("Viewing Angle")]
    public float Vertical = 0f;
    public float Horizontal = 0f;

    [Header("Speed Input")]
    [Range(0.1f, 5.0f)]
    public float PanSensitivity = 2.0f;
    [Range(0.1f, 5f)]
    public float ZoomSensitivity = 1f;

    [Header("Movement Toggle")]
    public PanDirection MovementPlane = PanDirection.XZ_Plane;
    public bool EnableZoom = true;
    public bool EnablePanning = true;
    public bool EnableCameraControl = true;
    public bool EnableLookAt = true;
    public bool RestrictHorizontalMovement = false;
    public bool RestrictVerticalMovement = false;

    private Vector3 panOffset = Vector3.zero;
    private bool panning = false;
    private Vector2 previousFingerPosition = Vector2.zero;

    void Update()
    {
        Vector3 pannedLookAtPosition = LookAtPoint.position + panOffset;
        if (EnableCameraControl)
        {
            float finalDistance = Mathf.Clamp(Distance, MinDistance, MaxDistance);
            Vector3 direction = new Vector3(0.0f, 0.0f, -finalDistance);
            Quaternion rotation = Quaternion.Euler(Horizontal, Vertical, 0.0f);
            Vector3 finalPosition = pannedLookAtPosition + (rotation * direction);
            transform.position = finalPosition;
        }
        if (EnableLookAt)
        {
            transform.LookAt(pannedLookAtPosition);
        }
    }

    private Vector3 AdjustToBoundary(Vector3 position)
    {
        Vector3 adjustedPosition = position;
        if (MovementPlane == PanDirection.XY_Plane)
        {
            adjustedPosition.y = Mathf.Clamp(position.y, bounds.MinVerticalBounds, bounds.MaxVerticalBounds);
        }
        else if (MovementPlane == PanDirection.XZ_Plane)
        {
            adjustedPosition.z = Mathf.Clamp(position.z, bounds.MinVerticalBounds, bounds.MaxVerticalBounds);
        }
        adjustedPosition.x = Mathf.Clamp(position.x, bounds.MinHorizontalBounds, bounds.MaxHorizontalBounds);
        return adjustedPosition;
    }

    public void MoveCamera(Gesture gesture)
    {
        if (gesture.touchCount == 1 && EnableZoom && EnableCameraControl && gesture.pickedObject == null && gesture.pickedUIElement == null && !gesture.isOverGui)
        {
            if (panning)
            {
                Vector2 currentFingerPosition = gesture.position;
                if (currentFingerPosition != previousFingerPosition)
                {
                    Vector3 deltaPosition = gesture.deltaPosition * PanSensitivity * 0.01f;//(PanSensitivity * Time.deltaTime);
                    Vector3 movement;
                    float x = (RestrictHorizontalMovement) ? 0f : -deltaPosition.x;
                    float y = (RestrictVerticalMovement) ? 0f : -deltaPosition.y;
                    if (MovementPlane == PanDirection.XZ_Plane)
                    {
                        movement = (Quaternion.Euler(new Vector3(0.0f, Vertical, 0.0f)) * new Vector3(x, 0.0f, y));
                    }
                    else
                    {
                        movement = transform.rotation * new Vector3(x, y, 0.0f);
                    }
                    panOffset += movement;
                    if (BoundaryEnabled)
                    {
                        panOffset = AdjustToBoundary(panOffset);
                    }
                    previousFingerPosition = currentFingerPosition;
                }
            }
            else
            {
                previousFingerPosition = gesture.position;
                panning = true;
            }
        }
    }

    public void ZoomOut(Gesture gesture)
    {
        if (gesture.touchCount == 2 && EnableZoom && EnableCameraControl && gesture.pickedObject == null && gesture.pickedUIElement == null && !gesture.isOverGui)
        {
            float zoomAmount = gesture.deltaPinch * ZoomSensitivity * 0.01f;
            Distance += zoomAmount;
            Distance = Mathf.Clamp(Distance, MinDistance, MaxDistance);
        }
    }

    public void ZoomIn(Gesture gesture)
    {
        if (gesture.touchCount == 2 && EnableZoom && EnableCameraControl && gesture.pickedObject == null && gesture.pickedUIElement == null && !gesture.isOverGui)
        {
            float zoomAmount = gesture.deltaPinch * ZoomSensitivity * 0.01f;
            Distance -= zoomAmount;
            Distance = Mathf.Clamp(Distance, MinDistance, MaxDistance);
        }
    }

    public void OnRelease(Gesture gesture)
    {
        panning = false;
    }

    public void ChangeDistanceConstraints(float distance, float min, float max)
    {
        Distance = distance;
        MinDistance = min;
        MaxDistance = max;
    }

    public void ChangeAngle(float v, float h)
    {
        Vertical = v;
        Horizontal = h;
    }

    public void ResetPanMovement()
    {
        panOffset = Vector3.zero;
    }



    private void SubscribeEvents()
    {
        EasyTouch.On_TouchUp += OnRelease;

        //Orbit Controls
        EasyTouch.On_Swipe += MoveCamera;

        //Zoom Controls
        EasyTouch.On_PinchIn += ZoomOut;
        EasyTouch.On_PinchOut += ZoomIn;
    }

    private void UnsubscribeEvents()
    {
        EasyTouch.On_TouchUp -= OnRelease;

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

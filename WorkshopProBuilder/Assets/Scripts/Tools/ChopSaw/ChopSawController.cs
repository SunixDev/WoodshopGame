using UnityEngine;
using System.Collections;

public class ChopSawController : MonoBehaviour 
{
    public bool Moveable;
    public Transform BladeTransform;
    public Blade Blade;

    [Header("Rotation Limits (0 to 360)")]
    public float UpperLimit = 345;
    public float LowerLimit = 30;

    private bool selected;
    private Vector3 previousPosition;
    private Vector3 originalBladeEdgePosition;

    void Start()
    {
        selected = false;
    }

    public void OnSawArmTouch(Gesture gesture)
    {
        if (gesture.pickedObject != null)
        {
            if (gesture.pickedObject.transform.parent != null)
            {
                GameObject obj = gesture.pickedObject.transform.parent.gameObject;
                if (Moveable && obj == gameObject)
                {
                    selected = true;
                }
            }
        }
    }

    public void OnSawArmRelease(Gesture gesture)
    {
        if (selected)
        {
            selected = false;
            transform.localRotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, 0.0f));
            Blade.ResetEdgePosition();
        }
    }

    public void OnMoveSawArm(Gesture gesture)
    {
        if (Moveable && selected && (gesture.swipe == EasyTouch.SwipeDirection.Up || gesture.swipe == EasyTouch.SwipeDirection.Down) )
        {
            previousPosition = BladeTransform.position;
            float direction = (gesture.swipe == EasyTouch.SwipeDirection.Up) ? -1.0f : 1.0f;
            float deltaMagnitude = gesture.deltaPosition.magnitude;
            if (deltaMagnitude > 4.0f)
                deltaMagnitude = 4.0f;
            transform.Rotate(Vector3.right, (deltaMagnitude * direction), Space.Self);
            if (transform.eulerAngles.x > LowerLimit && transform.eulerAngles.x < 180)
            {
                transform.eulerAngles = new Vector3(LowerLimit, transform.eulerAngles.y, transform.eulerAngles.z);
            }
            else if (transform.eulerAngles.x < UpperLimit && transform.eulerAngles.x > 180)
            {
                transform.eulerAngles = new Vector3(UpperLimit, transform.eulerAngles.y, transform.eulerAngles.z);
            }
            else
            {
                float difference = BladeTransform.position.y - previousPosition.y;
                Vector3 newEdgePosition = new Vector3(Blade.EdgePosition().x, Blade.EdgePosition().y + difference, Blade.EdgePosition().z);
                Blade.SetEdgePosition(newEdgePosition);
            }
        }
    }

    public void EnableMovement(bool enable)
    {
        Moveable = enable;
    }

    void OnEnable()
    {
        EasyTouch.On_TouchStart += OnSawArmTouch;
        EasyTouch.On_TouchUp += OnSawArmRelease;
        EasyTouch.On_Drag += OnMoveSawArm;
    }

    void OnDisable()
    {
        EasyTouch.On_TouchStart -= OnSawArmTouch;
        EasyTouch.On_TouchUp -= OnSawArmRelease;
        EasyTouch.On_Drag -= OnMoveSawArm;
    }

    void OnDestory()
    {
        EasyTouch.On_TouchStart -= OnSawArmTouch;
        EasyTouch.On_TouchUp -= OnSawArmRelease;
        EasyTouch.On_Drag -= OnMoveSawArm;
    }
}
using UnityEngine;
using System.Collections;

public class ChopSawController : MonoBehaviour 
{
    public bool Moveable;

    [Header("Rotation Limits (0 to 360)")]
    public float UpperLimit = 345;
    public float LowerLimit = 30;

    private bool selected;
    private Vector3 previousPosition;

    void Start()
    {
        Moveable = true;
        selected = false;
    }

    public void OnSawArmTouch(Gesture gesture)
    {
        GameObject obj = gesture.pickedObject.transform.parent.gameObject;
        if (Moveable && obj == gameObject)
        {
            selected = true;
        }
    }

    public void OnSawArmRelease(Gesture gesture)
    {
        selected = false;
        transform.eulerAngles = new Vector3(0.0f, transform.eulerAngles.y, transform.eulerAngles.z);
    }

    public void OnMoveSawArm(Gesture gesture)
    {
        if (Moveable && selected && (gesture.swipe == EasyTouch.SwipeDirection.Up || gesture.swipe == EasyTouch.SwipeDirection.Down) )
        {
            float direction = (gesture.swipe == EasyTouch.SwipeDirection.Up) ? -1.0f : 1.0f;
            float deltaMagnitude = gesture.deltaPosition.magnitude;
            transform.Rotate(Vector3.right, (deltaMagnitude * direction), Space.Self);
            if (transform.eulerAngles.x > LowerLimit && transform.eulerAngles.x < 180)
            {
                transform.eulerAngles = new Vector3(LowerLimit, transform.eulerAngles.y, transform.eulerAngles.z);
            }
            else if (transform.eulerAngles.x < UpperLimit && transform.eulerAngles.x > 180)
            {
                transform.eulerAngles = new Vector3(UpperLimit, transform.eulerAngles.y, transform.eulerAngles.z);
            }
        }
    }

    void OnEnable()
    {
        EasyTouch.On_TouchDown += OnSawArmTouch;
        EasyTouch.On_TouchUp += OnSawArmRelease;
        EasyTouch.On_Drag += OnMoveSawArm;
    }

    void OnDisable()
    {
        EasyTouch.On_TouchDown -= OnSawArmTouch;
        EasyTouch.On_TouchUp -= OnSawArmRelease;
        EasyTouch.On_Drag -= OnMoveSawArm;
    }

    void OnDestory()
    {
        EasyTouch.On_TouchDown -= OnSawArmTouch;
        EasyTouch.On_TouchUp -= OnSawArmRelease;
        EasyTouch.On_Drag -= OnMoveSawArm;
    }
}
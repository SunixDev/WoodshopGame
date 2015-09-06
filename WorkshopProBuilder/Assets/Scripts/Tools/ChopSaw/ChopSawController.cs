using UnityEngine;
using System.Collections;

public class ChopSawController : MonoBehaviour 
{
    public bool Moveable;
    public Transform Blade;
    public Transform BladeEdge;

    [Header("Rotation Limits (0 to 360)")]
    public float UpperLimit = 345;
    public float LowerLimit = 30;

    private bool selected;
    private Vector3 previousPosition;
    private Vector3 originalBladeEdgePosition;

    void Start()
    {
        Moveable = true;
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
                    originalBladeEdgePosition = BladeEdge.position;
                }
            }
        }
    }

    public void OnSawArmRelease(Gesture gesture)
    {
        if (selected)
        {
            selected = false;
            transform.eulerAngles = new Vector3(0.0f, transform.eulerAngles.y, transform.eulerAngles.z);
            BladeEdge.position = originalBladeEdgePosition;
        }
    }

    public void OnMoveSawArm(Gesture gesture)
    {
        if (Moveable && selected && (gesture.swipe == EasyTouch.SwipeDirection.Up || gesture.swipe == EasyTouch.SwipeDirection.Down) )
        {
            previousPosition = Blade.position;
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
            else
            {
                float difference = Blade.position.y - previousPosition.y;
                BladeEdge.position = new Vector3(BladeEdge.position.x, BladeEdge.position.y + difference, BladeEdge.position.z);
            }
        }
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
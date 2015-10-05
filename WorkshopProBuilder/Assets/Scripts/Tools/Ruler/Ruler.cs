using UnityEngine;
using System.Collections;

public class Ruler : MonoBehaviour 
{
    private float maxSwipeTime;
    private float totalSwipeTime;
    private bool markerSet = false;
    private LineRenderer line;

    public void AddMark(Gesture gesture)
    {
        if (gesture.pickedObject != null)
        {
            if (gesture.pickedObject.tag == "Piece" || gesture.pickedObject.tag == "Leftover")
            {

            }
        }
    }

    public void SwipeMark(Gesture gesture)
    {
        if (gesture.pickedObject != null)
        {
            if ((gesture.pickedObject.tag == "Piece" || gesture.pickedObject.tag == "Leftover") && gesture.actionTime < 0.1f)
            {

            }
            else if (gesture.actionTime > 0.1f)
            {

            }
        }
    }

    public void LeaveMark(Gesture gesture)
    {
        if (gesture.pickedObject != null)
        {
            if (gesture.pickedObject.tag == "Piece" || gesture.pickedObject.tag == "Leftover")
            {

            }
        }
    }

    void OnEnable()
    {
        SubscribeAll();
    }

    void OnDisable()
    {
        UnsubscribeAll();
    }

    void OnDestroy()
    {
        UnsubscribeAll();
    }

    private void SubscribeAll()
    {
        //EasyTouch.On_TouchStart += SelectRuler;
    }

    private void UnsubscribeAll()
    {
        //EasyTouch.On_TouchStart -= SelectRuler;
    }
}

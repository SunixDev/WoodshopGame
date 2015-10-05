using UnityEngine;
using System.Collections;

public class RulerMovement : MonoBehaviour 
{
    public float DistanceFromCenterLimit;

    private bool selected = false;
    private Vector3 previousPosition = Vector3.zero;
    private Vector3 originalPosition;

    void Start()
    {
        originalPosition = transform.position;
    }

    void Update()
    {
        //Debug.Log(Vector3.Distance(originalPosition, transform.position));
    }

    public void SelectRuler(Gesture gesture)
    {
        //Debug.Log(gesture.pickedObject);
        if (gesture.pickedObject != null && !selected)
        {
            if (gesture.pickedObject == gameObject && gesture.isOverGui)
            {
                selected = true;
                previousPosition = gesture.GetTouchToWorldPoint(transform.position);
            }
        }
    }

    public void MoveRuler(Gesture gesture)
    {
        if (selected && gesture.isOverGui && gesture.touchCount == 1)
        {
            Vector3 currentPosition = gesture.GetTouchToWorldPoint(transform.position);
            Vector3 deltaPosition = currentPosition - previousPosition;
            deltaPosition = new Vector3(deltaPosition.x * Mathf.Abs(transform.right.x),
                                        deltaPosition.y * Mathf.Abs(transform.right.y),
                                        deltaPosition.z * Mathf.Abs(transform.right.z));
            transform.position += deltaPosition;
            previousPosition = currentPosition;
            if (PastLimit())
            {
                Vector3 direction = (transform.position - originalPosition).normalized;
                Vector3 limitedVector = direction * DistanceFromCenterLimit;
                transform.position = originalPosition + limitedVector;
            }
        }
    }

    public void FreeUpRuler(Gesture gesture)
    {
        if (selected && gesture.isOverGui)
        {
            selected = false;
        }
    }

    private bool PastLimit()
    {
        Vector3 toRulerPosition = transform.position - originalPosition;
        return (toRulerPosition.magnitude > DistanceFromCenterLimit);
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
        EasyTouch.On_TouchStart += SelectRuler;
        EasyTouch.On_Drag += MoveRuler;
        EasyTouch.On_TouchUp += FreeUpRuler;
    }

    private void UnsubscribeAll()
    {
        EasyTouch.On_TouchStart -= SelectRuler;
        EasyTouch.On_Drag -= MoveRuler;
        EasyTouch.On_TouchUp -= FreeUpRuler;
    }
}

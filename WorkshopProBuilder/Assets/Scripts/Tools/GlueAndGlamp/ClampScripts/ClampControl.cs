using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ClampControl : MonoBehaviour 
{
    public bool Moveable = true;
    public List<Collider> Colliders;
    public bool Dragging { get; private set; }

    private Transform objTransform;
    private Vector3 offset;
    private Clamp clamp;

    void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        Dragging = false;
        objTransform = transform;
        clamp = gameObject.GetComponent<Clamp>();
    }

    public void ResetSelection()
    {
        Moveable = false;
        Dragging = false;
    }

    public void OnTouch(Gesture gesture)
    {
        if (gesture.pickedObject != null)
        {
            if (Moveable && ContainsCollider(gesture.pickedObject.GetComponent<Collider>()))
            {
                offset = objTransform.position - gesture.GetTouchToWorldPoint(objTransform.position);
                Dragging = true;
            }
        }
    }

    private bool ContainsCollider(Collider collider)
    {
        bool containsCollider = false;
        for (int i = 0; i < Colliders.Count && !containsCollider; i++)
        {
            containsCollider = (Colliders[i] == collider);
        }
        return containsCollider;
    }

    public void DragClamp(Gesture gesture)
    {
        if (Moveable && Dragging && gesture.touchCount == 1)
        {
            objTransform.position = gesture.GetTouchToWorldPoint(objTransform.position) + offset;
        }
    }

    public void Deselect(Gesture gesture)
    {
        Dragging = false;
    }





    private void EnableTouchEvents()
    {
        EasyTouch.On_Drag += DragClamp;
        EasyTouch.On_TouchStart += OnTouch;
        EasyTouch.On_TouchUp += Deselect;
    }

    private void DisableTouchEvents()
    {
        EasyTouch.On_Drag -= DragClamp;
        EasyTouch.On_TouchStart -= OnTouch;
        EasyTouch.On_TouchUp -= Deselect;
    }

    void OnEnable()
    {
        EnableTouchEvents();
    }

    void OnDisable()
    {
        DisableTouchEvents();
    }

    void OnDestory()
    {
        DisableTouchEvents();
    }
}
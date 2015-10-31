using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ClampControl : MonoBehaviour 
{
    public bool Moveable = true;
    public List<Collider> Colliders;
    public float RotationSpeed = 2.0f;
    public Quaternion CurrentRotation { get; private set; }
    public Transform objTransform { get; private set; }

    private bool selected = false;

    void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        objTransform = transform;
    }

    public void ResetSelection()
    {
        Moveable = false;
        selected = false;
    }

    public void OnTouch(Gesture gesture)
    {
        if (gesture.pickedObject != null)
        {
            if (Moveable && ContainsCollider(gesture.pickedObject.GetComponent<Collider>()))
            {
                selected = true;
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

    public void MovePiece(Gesture gesture)
    {
        if (Moveable && selected && gesture.touchCount == 1)
        {
            objTransform.position = gesture.GetTouchToWorldPoint(objTransform.position);
        }
    }

    public void Deselect(Gesture gesture)
    {
        selected = false;
    }





    private void EnableTouchEvents()
    {
        EasyTouch.On_Drag += MovePiece;
        EasyTouch.On_TouchStart += OnTouch;
        EasyTouch.On_TouchUp += Deselect;
    }

    private void DisableTouchEvents()
    {
        EasyTouch.On_Drag -= MovePiece;
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
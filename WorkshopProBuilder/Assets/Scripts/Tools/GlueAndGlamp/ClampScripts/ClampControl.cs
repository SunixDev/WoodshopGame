using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ClampControl : MonoBehaviour 
{
    public List<Collider> Colliders;
    public bool Moving { get; private set; }

    private Transform objTransform;
    private Vector3 offset;
    private Clamp clamp;

    void Start()
    {
        Moving = false;
        objTransform = transform;
        clamp = gameObject.GetComponent<Clamp>();
    }

    void Update()
    {
        Vector3 forwardDirection = Camera.main.transform.forward;
        Vector3 upDirection = Camera.main.transform.up;
        transform.rotation = Quaternion.LookRotation(forwardDirection, upDirection);
    }

    //public void ResetSelection()
    //{
    //    Moving = false;
    //}

    //public void OnTouch(Gesture gesture)
    //{
    //    if (gesture.pickedObject != null)
    //    {
    //        if (ContainsCollider(gesture.pickedObject.GetComponent<Collider>()))
    //        {
    //            offset = objTransform.position - gesture.GetTouchToWorldPoint(objTransform.position);
    //            Moving = true;
    //        }
    //    }
    //}

    //public void DragClamp(Gesture gesture)
    //{
    //    if (Moving && gesture.touchCount == 1)
    //    {
    //        objTransform.position = gesture.GetTouchToWorldPoint(objTransform.position) + offset;
    //    }
    //}

    //public void Deselect(Gesture gesture)
    //{
    //    ResetSelection();
    //}

    //private bool ContainsCollider(Collider collider)
    //{
    //    bool containsCollider = false;
    //    for (int i = 0; i < Colliders.Count && !containsCollider; i++)
    //    {
    //        containsCollider = (Colliders[i] == collider);
    //    }
    //    return containsCollider;
    //}





    //private void EnableTouchEvents()
    //{
    //    EasyTouch.On_Drag += DragClamp;
    //    EasyTouch.On_TouchStart += OnTouch;
    //    EasyTouch.On_TouchUp += Deselect;
    //}

    //private void DisableTouchEvents()
    //{
    //    EasyTouch.On_Drag -= DragClamp;
    //    EasyTouch.On_TouchStart -= OnTouch;
    //    EasyTouch.On_TouchUp -= Deselect;
    //}

    //void OnEnable()
    //{
    //    EnableTouchEvents();
    //}
    //void OnDisable()
    //{
    //    DisableTouchEvents();
    //}
    //void OnDestory()
    //{
    //    DisableTouchEvents();
    //}
}
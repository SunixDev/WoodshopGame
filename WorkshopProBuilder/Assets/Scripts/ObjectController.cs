using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class ObjectController : MonoBehaviour 
{
    private Rigidbody objRigidbody;
    private bool selected;
    private Vector3 deltaPosition;

	void Start () 
    {
        objRigidbody = GetComponent<Rigidbody>();
        objRigidbody.useGravity = false;
        objRigidbody.isKinematic = true;
	}

    public void OnDragStart(Gesture gesture)
    {
        if (gesture.pickedObject != null)
        {
            if (gesture.pickedObject.tag == "Tool")
            {
                Transform go = gesture.pickedObject.transform;
                while (go != gameObject.transform && go != null)
                {
                    go = go.transform.parent;
                }
                if (go != null)
                {
                    selected = true;
                    deltaPosition = gesture.GetTouchToWorldPoint(go.position);
                }
                else
                {
                    selected = false;
                }
            }
        }
    }

    public void OnDragEnd(Gesture gesture)
    {
        selected = false;
    }

    public void MoveObject(Gesture gesture)
    {
        if (selected)
        {
            Vector3 position = gesture.GetTouchToWorldPoint(transform.position);
            Vector3 nextPosition = position - deltaPosition;
            deltaPosition = position;
            objRigidbody.position += nextPosition;
        }
    }
	
	void OnEnable () 
    {
        EasyTouch.On_DragStart += OnDragStart;
        EasyTouch.On_Drag += MoveObject;
        EasyTouch.On_DragEnd += OnDragEnd;
	}

    void OnDisable()
    {
        EasyTouch.On_DragStart -= OnDragStart;
        EasyTouch.On_Drag -= MoveObject;
        EasyTouch.On_DragEnd -= OnDragEnd;
    }
}

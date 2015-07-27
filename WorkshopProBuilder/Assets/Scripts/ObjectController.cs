using UnityEngine;
using System.Collections;

public class ObjectController : MonoBehaviour 
{
    public bool ConstrainX;
    public bool ConstrainY;
    public bool ConstrainZ;
    public bool Draggable { get; set; }

    private Vector3 ScreenPoint;
    private bool BeingDragged;
    private Rigidbody rigidbodyObject;

    void Start()
    {
        Draggable = true;
        rigidbodyObject = GetComponent<Rigidbody>();
    }

    void LateUpdate()
    {
        if (Draggable)
        {
            DragObject();
        }
    }

    private void DragObject()
    {
        if (!BeingDragged && Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray cursorRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(cursorRay, out hit) && hit.transform.gameObject.name == gameObject.name)
            {
                ScreenPoint = Camera.main.WorldToScreenPoint(rigidbodyObject.position);
                BeingDragged = true;
            }
        }
        else if (BeingDragged && Input.GetMouseButton(0))
        {
            Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, ScreenPoint.z);
            Vector3 currentPosition = Camera.main.ScreenToWorldPoint(curScreenPoint);

            float xFinal = (ConstrainX) ? transform.position.x : currentPosition.x; //transform.position.x
            float yFinal = (ConstrainY) ? transform.position.y : currentPosition.y; //transform.position.y
            float zFinal = (ConstrainZ) ? transform.position.z : currentPosition.z; //transform.position.z
            rigidbodyObject.position = new Vector3(xFinal, yFinal, zFinal);
        }
        else if (BeingDragged && Input.GetMouseButtonUp(0))
        {
            BeingDragged = false;
        } 
    }
}

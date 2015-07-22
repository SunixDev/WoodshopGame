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

    void Start()
    {
        Draggable = true;
    }

    void Update()
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
            if (Physics.Raycast(cursorRay, out hit) && hit.collider.gameObject.name == gameObject.name)
            {
                ScreenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
                BeingDragged = true;
            }
        }
        else if (BeingDragged && Input.GetMouseButton(0))
        {
            Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, ScreenPoint.z);
            Vector3 position = Camera.main.ScreenToWorldPoint(curScreenPoint);
            float xFinal = (ConstrainX) ? transform.position.x : position.x;
            float yFinal = (ConstrainY) ? transform.position.y : position.y;
            float zFinal = (ConstrainZ) ? transform.position.z : position.z;
            transform.position = new Vector3(xFinal, yFinal, zFinal);
        }
        else if (BeingDragged && Input.GetMouseButtonUp(0))
        {
            BeingDragged = false;
        } 
    }
}

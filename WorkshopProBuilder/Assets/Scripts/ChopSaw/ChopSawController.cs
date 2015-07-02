using UnityEngine;
using System.Collections;

public class ChopSawController : MonoBehaviour 
{
    private float cursorPosition;
    private bool movingSaw;

    private Vector3 screenPoint;
    private Vector3 offset;

    void Start()
    {
        movingSaw = false;
    }
	
	void Update () 
    {
        if (Input.GetMouseButton(0))
        {
            RaycastHit hit;
            Ray cursorRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(cursorRay, out hit))
            {
                string name = hit.collider.name;
                if (!movingSaw && name == "SawHandle")
                {
                    movingSaw = true;
                }

                if (name == "SawHandle" || movingSaw)
                {
                    Vector3 mouse = new Vector3(Input.mousePosition.x, Input.mousePosition.y, hit.transform.position.z);
                    Vector3 mousePosition = Camera.main.ScreenToWorldPoint(mouse);
                    moveSaw(mousePosition.y);
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            movingSaw = false;
        }
	}

    private void moveSaw(float hitPosition)
    {
        if (Input.GetMouseButtonDown(0))
        {
            screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
        }
        else if (Input.GetMouseButton(0))
        {
            Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);

            Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint);
            transform.position = new Vector3(transform.position.x, curPosition.y, transform.position.z);
        }
    }
}



//float cursorDeltaPosition = hitPosition;
//float delta = cursorPosition - cursorDeltaPosition;
//Debug.Log(delta);
//if (delta != 0.0f)
//{
//    transform.position += new Vector3(0.0f, delta, 0.0f);
//}
//cursorPosition = cursorDeltaPosition;
using UnityEngine;
using System.Collections;

public class MoveToTest : MonoBehaviour 
{
    public LayerMask raycastLayerMask;

    void LateUpdate()
    {
        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, 1000f, raycastLayerMask))
            {
                Debug.Log(hit.collider.gameObject);
                transform.position = hit.collider.gameObject.transform.position;
            }
        }
    }
}

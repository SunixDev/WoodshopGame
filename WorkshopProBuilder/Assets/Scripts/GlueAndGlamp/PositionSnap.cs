using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ObjectController))]
public class PositionSnap : MonoBehaviour 
{
    public Transform AnchorPoint;
    public Transform ConnectingAnchorPoint;
    public float SnapOffset = 0.01f;

    private ObjectController controller;

	void Start ()
    {
        controller = GetComponent<ObjectController>();
        controller.Draggable = true;
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            float distance = Vector3.Distance(AnchorPoint.position, ConnectingAnchorPoint.position);
            if (distance <= SnapOffset)
            {
                controller.Draggable = false;
                Vector3 nextPosition = Vector3.MoveTowards(AnchorPoint.position, ConnectingAnchorPoint.position, 1.0f);
                float magnitude = Vector3.Magnitude(nextPosition - AnchorPoint.position);
                Vector3 direction = Vector3.Normalize(nextPosition - AnchorPoint.position);
                transform.position += (direction * magnitude);
            }
        }
        else
        {
            controller.Draggable = true;
        }
    }
}

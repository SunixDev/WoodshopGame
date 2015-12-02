using UnityEngine;
using System.Collections;

public class Clamp : MonoBehaviour 
{
    public Transform ClampHead;
    public Transform ClampHandle;
    public Transform HandleContactPoint;
    [HideInInspector]
    public ClampControl controller;

    private Vector3 originalHandlePosition;

	void Awake () 
    {
        originalHandlePosition = ClampHandle.localPosition;
        controller = gameObject.GetComponent<ClampControl>();
	}

    public void ClampAt(ClampPoint point)
    {
        transform.parent = point.GetParentTransform();
        transform.localRotation = Quaternion.Euler(point.LocalConnectionRotation);

        Vector3 position = ClampHead.position;
        Vector3 nextPosition = Vector3.Lerp(position, point.Position, 1.0f);
        Vector3 totalMovement = CalculateMovementVector(position, nextPosition);
        transform.position += totalMovement;

        ConnectHandle();
        point.Clamped = true;
    }

    private void ConnectHandle()
    {
        Ray ray = new Ray(HandleContactPoint.position, HandleContactPoint.right);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Vector3 position = HandleContactPoint.position;
            Vector3 nextPosition = Vector3.Lerp(position, hit.point, 1.0f);
            Vector3 totalMovement = CalculateMovementVector(position, nextPosition);
            ClampHandle.position += totalMovement;
        }
    }

    private Vector3 CalculateMovementVector(Vector3 currentPosition, Vector3 nextPosition)
    {
        float magnitude = Vector3.Magnitude(nextPosition - currentPosition);
        Vector3 direction = Vector3.Normalize(nextPosition - currentPosition);
        Vector3 totalMovement = (direction * magnitude);
        return totalMovement;
    }

    public void AddClampPoint(GameObject clampPoint)
    {
        if (transform.parent != null)
        {
            clampPoint.transform.position = ClampHead.position;
            clampPoint.transform.parent = transform.parent;
            ClampPoint point = clampPoint.GetComponent<ClampPoint>();
            float x = Mathf.Round(transform.rotation.eulerAngles.x);
            float y = Mathf.Round(transform.rotation.eulerAngles.y);
            float z = Mathf.Round(transform.rotation.eulerAngles.z);
            point.LocalConnectionRotation = new Vector3(x, y, z);
        }
        else
        {
            Debug.LogError("Make sure clamp is parented to the object it will be clamping to.");
        }
    }
}
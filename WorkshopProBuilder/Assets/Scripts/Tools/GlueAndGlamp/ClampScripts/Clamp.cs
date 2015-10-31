using UnityEngine;
using System.Collections;

public class Clamp : MonoBehaviour 
{
    public Transform ClampHead;
    public Transform ClampHandle;
    public Transform HandleContactPoint;
    public bool ClampedDown { get; private set; }

    private Vector3 originalHandlePosition;

	void Start () 
    {
        originalHandlePosition = ClampHandle.localPosition;
	}

    public void ClampAt(ClampPoint point, Transform parentPiece)
    {
        transform.parent = parentPiece;
        transform.localRotation = Quaternion.Euler(point.LocalConnectionRotation);

        Vector3 position = ClampHead.position;
        Vector3 nextPosition = Vector3.MoveTowards(position, point.Position, 1.0f);
        Vector3 totalMovement = CalculateMovementVector(position, nextPosition);
        transform.position += totalMovement;

        ConnectHandle();
        ClampedDown = true;
    }

    public void ReleaseClamp()
    {
        ClampedDown = false;
        transform.parent = null;
        ClampHandle.localPosition = originalHandlePosition;
    }

    public void ConnectHandle()
    {
        Ray ray = new Ray(HandleContactPoint.position, HandleContactPoint.right);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Vector3 position = HandleContactPoint.position;
            Vector3 nextPosition = Vector3.MoveTowards(position, hit.point, 1.0f);
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
}
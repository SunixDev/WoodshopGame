using UnityEngine;
using System.Collections;

public class ClampHeadPoint : MonoBehaviour 
{
    public Transform ClampHead;
    public Transform MoveableHeadPoint;
    public Transform MoveableHeadObject;

    private bool isConnected;

	void Start () 
    {
        isConnected = false;
	}

    public bool IsClampedDown()
    {
        return isConnected;
    }

    public Vector3 ClampHeadPosition()
    {
        return ClampHead.position;
    }

    public void ClampAt(ClampPoint point)
    {
        transform.rotation = Quaternion.Euler(point.ConnectionRotation);
        Vector3 position = ClampHead.position;
        Vector3 nextPosition = Vector3.MoveTowards(position, point.Position, 1.0f);
        float magnitude = Vector3.Magnitude(nextPosition - position);
        Vector3 direction = Vector3.Normalize(nextPosition - position);
        Vector3 totalMovement = (direction * magnitude);
        transform.position += totalMovement;
        ConnectMoveableClamp();
        isConnected = true;
    }

    public void ConnectMoveableClamp()
    {
        Ray ray = new Ray(MoveableHeadPoint.position, MoveableHeadPoint.right);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Vector3 position = MoveableHeadPoint.position;
            Vector3 nextPosition = Vector3.MoveTowards(position, hit.point, 1.0f);
            float magnitude = Vector3.Magnitude(nextPosition - position);
            Vector3 direction = Vector3.Normalize(nextPosition - position);
            Vector3 totalMovement = (direction * magnitude);
            MoveableHeadObject.position += totalMovement;
        }
    }
}


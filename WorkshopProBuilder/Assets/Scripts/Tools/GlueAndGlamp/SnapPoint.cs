using UnityEngine;
using System.Collections;

public class SnapPoint : MonoBehaviour 
{
    public SnapPiece ParentSnapPiece;
    public string ConnectionID = "Default";
    public bool CanConnect = true;
    public bool IsConnected { get; private set; }
    public Vector3 CurrentPosition
    {
        get
        {
            return transform.position;
        }
    }

    void Start()
    {
        HidePoint();
    }

    public bool CanConnectTo(SnapPoint otherPoint)
    {
        return (otherPoint.ConnectionID == ConnectionID) && CanConnect && otherPoint.CanConnect && !otherPoint.IsConnected && !IsConnected;
    }

    public float DistanceFromPoint(SnapPoint otherPoint)
    {
        return Vector3.Distance(CurrentPosition, otherPoint.CurrentPosition);
    }

    public void ConnectToPoint(SnapPoint otherPoint)
    {
        IsConnected = true;
        otherPoint.IsConnected = true;
        ParentSnapPiece.RotateToLocalRotation();
        Vector3 nextPosition = Vector3.MoveTowards(CurrentPosition, otherPoint.CurrentPosition, 1.0f);
        float magnitude = Vector3.Magnitude(nextPosition - CurrentPosition);
        Vector3 direction = Vector3.Normalize(nextPosition - CurrentPosition);
        Vector3 totalMovement = (direction * magnitude);
        ParentSnapPiece.SnapTo(totalMovement);
    }

    public void DisplayPoint()
    {
        GetComponent<MeshRenderer>().enabled = true;
    }

    public void HidePoint()
    {
        GetComponent<MeshRenderer>().enabled = false;
    }
}
using UnityEngine;
using System.Collections;

public class SnapPoint : MonoBehaviour 
{
    public SnapPiece ParentSnapPiece;
    public string ConnectionID = "Default";
    public bool ReadyToConnect { get; set; }
    public bool IsConnected { get; private set; }
    public Vector3 Position
    {
        get
        {
            return transform.position;
        }
    }

    public bool CanConnectTo(SnapPoint otherPoint)
    {
        return (otherPoint.ConnectionID == ConnectionID) && ReadyToConnect && otherPoint.ReadyToConnect && !otherPoint.IsConnected && !IsConnected;
    }

    //public float DistanceFromPoint(SnapPoint otherPoint)
    //{
    //    return Vector3.Distance(Position, otherPoint.Position);
    //}

    public void ConnectPieceToPoint(SnapPoint otherPoint, Transform center)
    {
        IsConnected = true;
        otherPoint.IsConnected = true;
        ParentSnapPiece.SnapTo(center.position);
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

//Vector3 nextPosition = Vector3.MoveTowards(CurrentPosition, otherPoint.CurrentPosition, 1.0f);
//float magnitude = Vector3.Magnitude(nextPosition - CurrentPosition);
//Vector3 direction = Vector3.Normalize(nextPosition - CurrentPosition);
//Vector3 totalMovement = (direction * magnitude);
//ParentSnapPiece.SnapTo(totalMovement);
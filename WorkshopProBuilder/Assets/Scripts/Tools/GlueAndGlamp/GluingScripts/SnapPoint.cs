using UnityEngine;
using System.Collections;

public class SnapPoint : MonoBehaviour 
{
    public SnapPiece ParentSnapPiece;
    public string ConnectionID = "Default";
    public bool ReadyToConnect { get; private set; }
    public bool IsConnected { get; private set; }
    public Vector3 Position
    {
        get
        {
            return transform.position;
        }
    }

    void Awake()
    {
        ActivatePoint();
        IsConnected = false;
    }

    public bool CanConnectTo(SnapPoint otherPoint)
    {
        return (otherPoint.ConnectionID == ConnectionID) && ReadyToConnect && otherPoint.ReadyToConnect && !otherPoint.IsConnected && !IsConnected && otherPoint != this;
    }

    public void ConnectPieceToPoint(SnapPoint otherPoint, Transform center)
    {
        IsConnected = true;
        otherPoint.IsConnected = true;
        ParentSnapPiece.SnapToProject(center.position);
    }

    public void ActivatePoint()
    {
        ReadyToConnect = true;
        GetComponent<MeshRenderer>().enabled = true;
    }

    public void DeactivatePoint()
    {
        ReadyToConnect = false;
        GetComponent<MeshRenderer>().enabled = false;
    }
}
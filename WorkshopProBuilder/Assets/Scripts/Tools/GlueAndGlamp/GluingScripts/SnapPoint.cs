using UnityEngine;
using System.Collections;

public class SnapPoint : MonoBehaviour 
{
    public SnapPiece ParentSnapPiece;
    public string ConnectionID = "Default";
    public bool CanConnect = false;
    public bool IsConnected { get; set; }
    public bool ActiveInStep = false;
    public Vector3 Position
    {
        get
        {
            return transform.position;
        }
    }

    public void Initialize()
    {
        DeactivatePoint();
        IsConnected = false;
    }

    public bool CanConnectTo(SnapPoint otherPoint)
    {
        return (otherPoint.ConnectionID == ConnectionID) && CanConnect && otherPoint.CanConnect && !IsConnected && !otherPoint.IsConnected && otherPoint != this;
    }

    public void ActivatePoint()
    {
        CanConnect = true;
        GetComponent<MeshRenderer>().enabled = true;
    }

    public void DeactivatePoint()
    {
        CanConnect = false;
        GetComponent<MeshRenderer>().enabled = false;
    }
}
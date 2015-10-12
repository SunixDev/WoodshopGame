using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SnapPiece : MonoBehaviour 
{
    public List<SnapPoint> SnapPoints;
    public Vector3 ConnectedLocalRotation;
    public Vector3 ConnectedLocalPosition;
    public bool IsAttached
    {
        get
        {
            bool attached = false;
            for (int i = 0; i < SnapPoints.Count && !attached; i++)
            {
                attached = SnapPoints[i].IsConnected;
            }
            return attached;
        }
    }

    //private List<SnapPiece> PiecesConnectedTo;
    public bool AllPointsConnected
    {
        get
        {
            bool valid = true;
            for (int i = 0; i < SnapPoints.Count && valid; i++)
            {
                valid = SnapPoints[i].IsConnected;
            }
            return valid;
        }
    }

	void Awake () 
    {
        //PiecesConnectedTo = new List<SnapPiece>();
	}

    public void SnapTo(Vector3 movementVector)
    {
        transform.position += movementVector;
    }

    public void RotateToLocalRotation()
    {
        transform.localRotation = Quaternion.Euler(ConnectedLocalRotation);
    }

    public void MoveToLocalLocation()
    {
        transform.localPosition = ConnectedLocalPosition;
    }

    public List<SnapPoint> GetAvailableSnapPoints()
    {
        List<SnapPoint> points = new List<SnapPoint>();
        foreach (SnapPoint p in SnapPoints)
        {
            if (!p.IsConnected)
            {
                points.Add(p);
            }
        }
        return points;
    }
}

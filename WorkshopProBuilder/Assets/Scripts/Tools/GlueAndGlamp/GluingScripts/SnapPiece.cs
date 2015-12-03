using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SnapPiece : MonoBehaviour 
{
    public List<SnapPoint> SnapPoints;
    public Vector3 ConnectedLocalPosition;
    public bool AllPointsConnected
    {
        get
        {
            bool valid = true;
            for (int i = 0; i < SnapPoints.Count && valid; i++)
            {
                if (SnapPoints[i].ActiveInStep)
                {
                    valid = SnapPoints[i].IsConnected;
                }
            }
            return valid;
        }
    }
    public bool AllPointsReadyToConnect
    {
        get
        {
            bool valid = true;
            for (int i = 0; i < SnapPoints.Count && valid; i++)
            {
                if(SnapPoints[i].gameObject.activeInHierarchy)
                {
                    if (SnapPoints[i].ActiveInStep)
                    {
                        valid = SnapPoints[i].ReadyToConnect;
                    }
                }
            }
            return valid;
        }
    }

    public void SnapToProject(Transform connectedProject)
    {
        transform.rotation = Quaternion.identity;
        transform.parent = connectedProject;
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

    //Editor Script Function
    public void UpdateConnectedLocalPosition()
    {
        if (transform.parent != null)
        {
            ConnectedLocalPosition = transform.localPosition;
        }
        else
        {
            Debug.LogError(gameObject + ": The object is not a child of another object in order to get an accurate local position");
        }
    }
}

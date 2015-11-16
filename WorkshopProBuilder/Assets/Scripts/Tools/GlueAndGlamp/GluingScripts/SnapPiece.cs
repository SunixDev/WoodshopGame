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
                valid = SnapPoints[i].IsConnected;
            }
            return valid;
        }
    }

    public void SnapTo(Vector3 centerPoint)
    {
        transform.rotation = Quaternion.identity;
        transform.position = centerPoint += ConnectedLocalPosition;
    }

    //public void RotateToLocalRotation()
    //{
    //    transform.localRotation = Quaternion.Euler(ConnectedLocalRotation);
    //}

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

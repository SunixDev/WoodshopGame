﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SnapPiece : MonoBehaviour 
{
    public List<SnapPoint> SnapPoints;
    public Vector3 ConnectedLocalPosition;
    private bool addedToProject = false;

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
                        valid = SnapPoints[i].CanConnect;
                    }
                }
            }
            return valid;
        }
    }

    void Awake()
    {
        if (SnapPoints == null)
        {
            SnapPoints = new List<SnapPoint>();
        }
    }

    public void SnapToProject(Transform connectedProject)
    {
        transform.parent = connectedProject;
        transform.localRotation = Quaternion.identity;
        transform.localPosition = ConnectedLocalPosition;
        addedToProject = true;
        if (gameObject.GetComponent<PieceController>() != null)
        {
            Destroy(gameObject.GetComponent<PieceController>());
        }
    }

    public bool CanConnectAt(SnapPoint otherPoint)
    {
        bool valid = false;
        if (otherPoint != null)
        {
            for (int i = 0; i < SnapPoints.Count && !valid; i++)
            {
                SnapPoint point = SnapPoints[i];
                valid = point.CanConnectTo(otherPoint);
            }
        }
        return valid;
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

    public bool PieceIsConnectedToProject()
    {
        return addedToProject;
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

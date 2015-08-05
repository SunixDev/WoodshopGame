using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnchorPoint : MonoBehaviour 
{
    public bool CanConnect;
    public bool Connected;
    public List<AnchorPoint> ConnectingAnchorPoints;

    private Transform objTransform;

    void Start()
    {
        objTransform = transform;
        Connected = false;
        if (ConnectingAnchorPoints == null)
        {
            ConnectingAnchorPoints = new List<AnchorPoint>();
        }
    }

    public Vector3 GetPosition()
    {
        return objTransform.position;
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WoodPiece : MonoBehaviour
{
    public string Name;
    public List<SnapPoint> SnapPoints;
    public List<ClampPoint> ClampPoints;
    public List<GlueBox> GlueBoxes;
    public Vector3 Position
    {
        get
        {
            return transform.position;
        }
    }
}

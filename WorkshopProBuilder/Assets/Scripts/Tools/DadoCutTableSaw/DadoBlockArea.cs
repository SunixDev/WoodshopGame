using UnityEngine;
using System.Collections;

public class DadoBlockArea : MonoBehaviour 
{
    public Transform RightEdge;
    public Transform LeftEdge;

    public Vector3 GetTopBorder()
    {
        return new Vector3(0.0f, RightEdge.position.y, 0.0f);
    }

    public Vector3 GetLeftBorder()
    {
        return new Vector3(LeftEdge.position.x, 0.0f, 0.0f);
    }

    public Vector3 GetRightBorder()
    {
        return new Vector3(RightEdge.position.x, 0.0f, 0.0f);
    }

    public Vector3 GetNearestCorner(Vector3 origin)
    {
        float distanceToRight = Vector3.Distance(RightEdge.position, origin);
        float distanceToLeft = Vector3.Distance(LeftEdge.position, origin);
        Vector3 nearestCorner;
        if (distanceToRight < distanceToLeft)
        {
            nearestCorner = RightEdge.position;
        }
        else
        {
            nearestCorner = LeftEdge.position;
        }
        return nearestCorner;
    }

    public bool WithinDadoCut(Vector3 bladeEdgePosition)
    {
        Vector3 fromRight = bladeEdgePosition - RightEdge.position;
        Vector3 fromLeft = bladeEdgePosition - LeftEdge.position;
        Vector3 fromTop = bladeEdgePosition - RightEdge.position;
        bool valid = (fromRight.x < 0 && fromLeft.x > 0 && fromTop.y < 0);
        return valid;
    }

    void OnDrawGizmos()
    {
        if (RightEdge != null && LeftEdge != null)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawLine(RightEdge.position, LeftEdge.position);
        }
    }
}

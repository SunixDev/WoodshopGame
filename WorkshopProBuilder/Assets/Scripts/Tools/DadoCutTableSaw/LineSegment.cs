using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class LineSegment : MonoBehaviour 
{
    public Transform EndPointOne;
    public Transform EndPointTwo;
    public bool CuttingBackwards { get; private set; }

    public void DetermineDirection(Vector3 bladePosition)
    {
        float distanceToPointOne = Vector3.Distance(EndPointOne.position, bladePosition);
        float distanceToPointTwo = Vector3.Distance(EndPointTwo.position, bladePosition);
        if (distanceToPointOne <= distanceToPointTwo)
        {
            CuttingBackwards = false; //From one to two
        }
        else
        {
            CuttingBackwards = true; //From two to one
        }
    }

    public bool ReachedEndOfBlock(Vector3 bladePosition)
    {
        bool passed = false;
        Vector3 blade = new Vector3(0.0f, 0.0f, bladePosition.z);
        Vector3 end = Vector3.zero;
        if (CuttingBackwards)
        {
            end = new Vector3(0.0f, 0.0f, EndPointOne.position.z);
        }
        else
        {
            end = new Vector3(0.0f, 0.0f, EndPointTwo.position.z);
        }

        Vector3 difference = end - bladePosition;
        if (difference.z >= 0.0f)
        {
            passed = true;
        }

        return passed;
    }

    public float GetRemainingDistance(Vector3 bladePosition)
    {
        float distance = 0.0f;
        if (CuttingBackwards)
        {
            distance = Vector3.Distance(EndPointOne.position, bladePosition);
        }
        else
        {
            distance = Vector3.Distance(EndPointTwo.position, bladePosition);
        }
        return distance;
    }

    void OnDrawGizmos()
    {
        if (EndPointOne != null && EndPointTwo != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(EndPointOne.position, EndPointTwo.position);
        }
    }
}

using UnityEngine;
using System.Collections;

public class Line : MonoBehaviour 
{
    public Checkpoint startPoint;
    public GameObject[] pieces;
    public LineCutType cutType;

    private Checkpoint currentPoint;

	void Awake () 
    {
        currentPoint = startPoint;
	}

    public void updateToNextCheckpoint()
    {
        currentPoint = currentPoint.nextCheckpoint;
    }

    public Checkpoint getCurrentPoint()
    {
        return currentPoint;
    }
}

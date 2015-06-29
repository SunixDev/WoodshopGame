using UnityEngine;
using System.Collections;

public class CutLine : MonoBehaviour 
{
    public GameObject startingCheckpoint;

    private GameObject currentCheckPoint;
    private Vector3 startPoint;
    private LineRenderer line;

	void Start () 
    {
        if (startingCheckpoint == null)
        {
            startingCheckpoint = GameObject.FindGameObjectWithTag("StartPoint");
        }
        currentCheckPoint = startingCheckpoint;
        startPoint = startingCheckpoint.transform.position;
        line = GetComponent<LineRenderer>();

        Checkpoint point = startingCheckpoint.GetComponent<Checkpoint>();
        int i = 0;
        while (point != null)
        {
            line.SetPosition(i, point.getPosition());
            point = point.nextCheckpoint;
            i++;
        }
	}
	
	void Update () 
    {
        updateLine();
	}

    public void updateStartPosition(Vector3 position)
    {
        startPoint = position;
    }

    public void UpdateCurrentPosition()
    {
        currentCheckPoint = currentCheckPoint.GetComponent<Checkpoint>().nextCheckpoint.gameObject;
    }

    public GameObject getCurrentCheckpoint()
    {
        return currentCheckPoint;
    }

    private void updateLine()
    {
        Checkpoint point = currentCheckPoint.GetComponent<Checkpoint>();
        int i = 0;
        while (point != null)
        {
            line.SetPosition(i, point.getPosition());
            point = point.nextCheckpoint;
            i++;
        }
    }
}

using UnityEngine;
using System.Collections;

public class ChopSaw : MonoBehaviour 
{
    public GameObject woodMaterial;
    public Transform sawEdge;
    public Line[] cutLines;

    private bool movingSaw;
    private Line currentLine;
    private Checkpoint currentCheckpoint;
    private int lineIndex;
    private int MAX_LINES;
    private bool complete;
    private Vector3 maxHeight;
    private GameObject handle;

	void Start () 
    {
        lineIndex = 0;
        MAX_LINES = cutLines.Length;
        currentLine = cutLines[lineIndex];
        complete = false;
        handle = transform.FindChild("SawHandle").gameObject;
        maxHeight = handle.transform.position;
	}

    void Update()
    {
        if (!complete)
        {
            currentCheckpoint = currentLine.getCurrentPoint();
            Vector3 pointPosition = currentCheckpoint.getPosition();
            Vector3 saw = new Vector3(0.0f, sawEdge.position.y, sawEdge.position.z);
            Vector3 point = new Vector3(0.0f, pointPosition.y, pointPosition.z);

            Vector3 delta = point - saw;
            float distance = Vector3.Distance(saw, point);
            Debug.Log(distance);
            if (delta.y >= 0.0f && distance <= 0.1f)
            {
                if (currentCheckpoint.type == LinePointType.Start || currentCheckpoint.type == LinePointType.Checkpoint)
                {
                    currentLine.updateToNextCheckpoint();
                }
                else
                {
                    handle.transform.position = maxHeight;
                    lineIndex++;
                    Destroy(woodMaterial.transform.GetChild(0).gameObject);
                    if (lineIndex < MAX_LINES)
                    {
                        currentLine = cutLines[lineIndex];
                    }
                    else
                    {
                        complete = true;
                        //COMPLETE
                    }
                }
            }
        }
    }
}

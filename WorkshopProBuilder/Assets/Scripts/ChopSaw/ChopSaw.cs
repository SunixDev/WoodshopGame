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

	void Start () 
    {
        lineIndex = 0;
        MAX_LINES = cutLines.Length;
        currentLine = cutLines[lineIndex];
        currentCheckpoint = currentLine.getCurrentPoint();
	}

    void Update()
    {
        Vector3 pointPosition = currentCheckpoint.getPosition();
        Vector3 saw = new Vector3(0.0f, sawEdge.position.y, sawEdge.position.z);
        Vector3 point = new Vector3(0.0f, pointPosition.y, pointPosition.z);

        Vector3 delta = point - saw;
        if (delta.y != 0.0f)
        {
            if (currentCheckpoint.type == LinePointType.Start || currentCheckpoint.type == LinePointType.Checkpoint)
            {
                currentLine.updateToNextCheckpoint();
            }
            else
            {
                lineIndex++;
                if (lineIndex < MAX_LINES)
                {
                    currentLine = cutLines[lineIndex];
                }
                else
                {
                    //COMPLETE
                }
            }
        }
    }
}

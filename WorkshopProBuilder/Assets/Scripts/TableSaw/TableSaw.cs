using UnityEngine;
using System.Collections;

public class TableSaw : MonoBehaviour 
{
    public GameObject woodMaterial;
    public Transform sawEdge;
    public GameObject cutLineStart;
    [Range(1.0f, 5.0f)]
    public float averageCutRate;

    private bool cuttingBoard;

	void Start () 
    {
        cuttingBoard = true;
	}
	
	void Update () 
    {
        if (cutLineStart != null && cuttingBoard)
        {
            if (cutLineStart.tag == "StartPoint")
            {
                Vector3 saw = new Vector3(0.0f, 0.0f, sawEdge.position.z);
                Vector3 point = new Vector3(0.0f, 0.0f, cutLineStart.transform.position.z);

                Vector3 delta = point - saw;
                if (delta.z > 0.0f)
                {
                    cutLineStart = cutLineStart.GetComponent<Checkpoint>().nextCheckpoint.gameObject;
                }
            }
            else if (cutLineStart.tag == "EndPoint" || cutLineStart.tag == "Checkpoint")
            {
                Vector3 saw = new Vector3(0.0f, 0.0f, sawEdge.position.z);
                Vector3 point = new Vector3(0.0f, 0.0f, cutLineStart.transform.position.z);

                Vector3 delta = point - saw;
                if (delta.z > 0.0f)
                {
                    if (cutLineStart.tag == "EndPoint")
                    {
                        GameObject leftover = woodMaterial.transform.FindChild("WoodLeftover").gameObject;
                        woodMaterial.transform.DetachChildren();
                        leftover.transform.position += new Vector3(1.0f, 0.0f, 0.0f);
                        cuttingBoard = false;
                        Destroy(woodMaterial);
                    }
                    else if (cutLineStart.tag == "Checkpoint")
                    {
                        cutLineStart = cutLineStart.GetComponent<Checkpoint>().nextCheckpoint.gameObject;
                    }
                }
            }
        }
	}
}

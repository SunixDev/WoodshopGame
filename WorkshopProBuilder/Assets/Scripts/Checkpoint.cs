using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour 
{
    public Checkpoint nextCheckpoint;
    public Checkpoint previousCheckpoint;
    public LinePointType type;

    private Transform objectTransform;

    void Awake()
    {
        objectTransform = transform;
        GetComponent<Renderer>().enabled = false;
    }

    public Vector3 getPosition()
    {
        return objectTransform.position;
    }

    public string getTag()
    {
        return tag;
    }
}

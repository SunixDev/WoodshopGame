using UnityEngine;
using System.Collections;

public class Positional : MonoBehaviour 
{
    public Vector3 GoalPosition;
    public float GoalOffset;

    private Transform objectTransform;

	void Start () 
    {
        objectTransform = transform;
	}
	
	void Update () 
    {
        float currentDistance = Vector3.Distance(objectTransform.position, GoalPosition);
        if (currentDistance <= GoalOffset)
        {
            objectTransform.position = GoalPosition;
        }
	}
}

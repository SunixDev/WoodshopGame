using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Checkpoint : MonoBehaviour 
{
    public float DebugRadius = 0.02f;

    private Transform objectTransform;

	void Start() 
    {
        objectTransform = transform;
	}

    public Vector3 GetPosition()
    {
        return objectTransform.position;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(transform.position, DebugRadius);
    }

    //void Update() {}
}

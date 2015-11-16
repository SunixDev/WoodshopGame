using UnityEngine;
using System.Collections;

public class RotateTowardsTest : MonoBehaviour 
{
    public Transform point;
	
	void Update () 
    {
        transform.rotation = Quaternion.LookRotation(point.forward, point.up);
	}
}

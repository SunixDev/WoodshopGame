using UnityEngine;
using System.Collections;

public class Rotate : MonoBehaviour 
{
    public float xAngleRotation;
    public float yAngleRotation;
    public float zAngleRotation;

    public float rotationSpeed = 10;

	void Start () 
    {

	}

    void Update()
    {
        transform.Rotate((new Vector3(xAngleRotation, yAngleRotation, zAngleRotation) * Time.deltaTime) * rotationSpeed);
    }
}

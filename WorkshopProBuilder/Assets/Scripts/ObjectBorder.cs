using UnityEngine;
using System.Collections;

public class ObjectBorder : MonoBehaviour 
{
    public bool RestrictX;
    public float MinX;
    public float MaxX;

    public bool RestrictY;
    public float MinY;
    public float MaxY;

    public bool RestrictZ;
    public float MinZ;
    public float MaxZ;
	
	void LateUpdate () 
    {
        float x = transform.position.x;
        float y = transform.position.y;
        float z = transform.position.z;

        x = (RestrictX) ? Mathf.Clamp(x, MinX, MaxX) : x;
        y = (RestrictY) ? Mathf.Clamp(y, MinY, MaxY) : y;
        z = (RestrictZ) ? Mathf.Clamp(z, MinZ, MaxZ) : z;

        transform.position = new Vector3(x, y, z);
	}
}

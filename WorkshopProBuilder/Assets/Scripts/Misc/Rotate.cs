using UnityEngine;
using System.Collections;

public class Rotate : MonoBehaviour 
{
    public float RotationSpeed = 10;

    [Header("Rotation Axes")]
    public bool RotationX = true;
    public bool RotationY = false;
    public bool RotationZ = false;

    private bool rotateEnabled;

	void Start () 
    {
        StartCoroutine(RotationCoroutine());
        rotateEnabled = false;
	}

    IEnumerator RotationCoroutine()
    {
        while (true)
        {
            if (rotateEnabled)
            {
                float x = (RotationX) ? 1.0f : 0.0f;
                float y = (RotationY) ? 1.0f : 0.0f;
                float z = (RotationZ) ? 1.0f : 0.0f;
                transform.Rotate(new Vector3(x, y, z), RotationSpeed);
            }
            yield return null;
        }
    }

    public void EnableRotation(bool enable)
    {
        rotateEnabled = enable;
    }
}

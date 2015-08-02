using UnityEngine;
using System.Collections;

public class DadoCutPractice : MonoBehaviour 
{
    public float NumberOfCuts;

    private float ScalingAmount;
    private Vector3 StartingScale;
    private Vector3 StartingPosition;
    private bool Scalable;

	void Start () 
    {
        ScalingAmount = 1.0f / NumberOfCuts;
        StartingScale = transform.localScale;
        StartingPosition = transform.position;
        Scalable = true;
	}
	
	void Update () 
    {
        if (Scalable)
        {
            if (Input.GetMouseButtonDown(0))
            {
                float previousScale = transform.localScale.x;
                float resized = StartingScale.x * ScalingAmount;
                transform.localScale = new Vector3(transform.localScale.x - resized, StartingScale.y, StartingScale.z);

                //float x_Position = transform.position.x * ScalingAmount;
                //float newX = (transform.position.x - x_Position) / ScalingAmount;
                float newX = transform.position.x + ((transform.localScale.x - previousScale) / ((ScalingAmount * NumberOfCuts) * 2));
                Vector3 movement = new Vector3(newX, transform.position.y, transform.position.z) - transform.position;
                Vector3 newPosition = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z) * movement;
                transform.position += newPosition;
            }

            if (transform.localScale.x <= 0.0f)
            {
                Scalable = false;
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            transform.localScale = StartingScale;
            transform.position = StartingPosition;
            Scalable = true;
        }
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GlueBox : MonoBehaviour 
{
    public List<GluePlane> GluingPlanes;
    public float GlueIncreaseRate = 20.0f;

    [Header("Valid Glue Range")]
    [Range(50.0f, 90.0f)]
    public float ValidMinGlueAmount = 75.0f;
    public float GlueOffset = 5.0f;

    private Collider objCollider;
    private float currentGlueAmount;
    private Vector3 previousTouchPosition;
    private float ValidMaxGlueAmount = 100.0f;
    private bool minimumGlueAmountReached;

	void Start () 
    {
        objCollider = GetComponent<Collider>();
        foreach (GluePlane p in GluingPlanes)
        {
            p.MaxGlueAmount = ValidMaxGlueAmount;
        }
        previousTouchPosition = Vector3.zero;
        currentGlueAmount = 0.0f;
        minimumGlueAmountReached = false;
	}

    void Update()
    {
        if (Input.GetMouseButton(0) || Input.touchCount == 1)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f));
            if (Physics.Raycast(ray, out hit) && hit.collider == objCollider)
            {
                if (currentGlueAmount <= ValidMaxGlueAmount + GlueOffset)
                {
                    currentGlueAmount += GlueIncreaseRate * Time.deltaTime;
                    if (currentGlueAmount >= ValidMinGlueAmount && !minimumGlueAmountReached)
                    {
                        minimumGlueAmountReached = true;
                    }
                }

                if (currentGlueAmount <= ValidMaxGlueAmount)
                {
                    foreach (GluePlane p in GluingPlanes)
                    {
                        p.UpdatePlane(currentGlueAmount);
                    }
                }
            }
        }
    }

    public bool IsMinimumGlueAmountReached()
    {
        return minimumGlueAmountReached;
    }

    public float GetValidMaxGlueAmount()
    {
        return ValidMaxGlueAmount;
    }

    private void DebugLogs()
    {
        if (currentGlueAmount >= ValidMaxGlueAmount)
        {
            Debug.Log("Reached Maximum glue amount: " + currentGlueAmount);
        }
        else if (currentGlueAmount >= ValidMinGlueAmount)
        {
            Debug.Log("Reached minimum for perfect glue score: " + currentGlueAmount);
        }
        else if (currentGlueAmount >= ValidMinGlueAmount - GlueOffset)
        {
            Debug.Log("Just enough to connect, but not sticky enough: " + currentGlueAmount);
        }
        else if (currentGlueAmount < ValidMinGlueAmount - GlueOffset)
        {
            Debug.Log("Not enough to connect: " + currentGlueAmount);
        }
    }
}
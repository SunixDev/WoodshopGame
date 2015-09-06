using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GlueBox : MonoBehaviour 
{
    public List<GluePlane> GluingPlanes;
    public float GlueApplicationRate = 20.0f;
    public SnapPoint PointToActivate;

    [Header("Valid Glue Range")]
    [Range(75.0f, 95.0f)]
    public float MinPercentForPerfectScore = 75.0f;

    private Collider objCollider;
    private float currentGlueAmount;
    private Vector3 previousHitPoint;
    private float ValidMaxGlueAmount = 100.0f;
    private bool minimumGlueAmountReached;

	void Start () 
    {
        objCollider = GetComponent<Collider>();
        foreach (GluePlane p in GluingPlanes)
        {
            p.MaxGlueAmount = ValidMaxGlueAmount;
        }
        previousHitPoint = Vector3.zero;
        currentGlueAmount = 0.0f;
        minimumGlueAmountReached = false;
	}

    void Update()
    {
        if (Input.GetMouseButton(0) || Input.touchCount == 1)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f));
            if (Physics.Raycast(ray, out hit) && hit.collider == objCollider && previousHitPoint != hit.point)
            {
                currentGlueAmount += GlueApplicationRate * Time.deltaTime;
                if (currentGlueAmount >= MinPercentForPerfectScore && !minimumGlueAmountReached)
                {
                    minimumGlueAmountReached = true;
                    Debug.Log("Minimum Reached");
                }
                foreach (GluePlane p in GluingPlanes)
                {
                    p.UpdatePlane(currentGlueAmount);
                }
                previousHitPoint = hit.point;
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
}
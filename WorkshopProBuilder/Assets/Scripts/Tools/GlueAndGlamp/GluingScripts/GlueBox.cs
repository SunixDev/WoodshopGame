using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GlueBox : MonoBehaviour 
{
    public List<GluePlane> GluingPlanes;
    public float GlueApplicationRate = 20.0f;
    public SnapPoint PointToActivate;
    public bool ReadyToConnect { get; set; }

    [Header("Valid Glue Range")]
    public float MinValueToActivatePointAndLowScore = 60.0f;
    public float MinValueForPerfectScore = 90.0f;
    public float MaxGlueAmountBeforeTooMuch = 100.0f;

    private Collider objCollider;
    private float currentGlueAmount;
    private Vector3 previousHitPoint;
    private bool minimumGlueAmountReached = false;
    private bool perfectGlueAmountReached = false;

	void Start () 
    {
        objCollider = GetComponent<Collider>();
        foreach (GluePlane p in GluingPlanes)
        {
            p.MaxGlueAmount = MaxGlueAmountBeforeTooMuch;
        }
        previousHitPoint = Vector3.zero;
        currentGlueAmount = 0.0f;
        PointToActivate.CanConnect = false;
        PointToActivate.HidePoint();
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
                if (currentGlueAmount >= MinValueToActivatePointAndLowScore && !minimumGlueAmountReached)
                {
                    ReadyToConnect = true;
                    minimumGlueAmountReached = true;
                    PointToActivate.CanConnect = true;
                    Debug.Log("Minimum Reached");
                }
                if (currentGlueAmount >= MinValueForPerfectScore && currentGlueAmount < MaxGlueAmountBeforeTooMuch && !perfectGlueAmountReached)
                {
                    perfectGlueAmountReached = true;
                    Debug.Log("Perfect Score Reached");
                }
                foreach (GluePlane p in GluingPlanes)
                {
                    p.UpdatePlane(currentGlueAmount);
                }
                previousHitPoint = hit.point;
            }
        }
    }

    public float GetTotalGlueApplied()
    {
        return currentGlueAmount;
    }

    public bool IsMinimumGlueAmountReached()
    {
        return minimumGlueAmountReached;
    }

    public float GetValidMaxGlueAmount()
    {
        return MaxGlueAmountBeforeTooMuch;
    }
}
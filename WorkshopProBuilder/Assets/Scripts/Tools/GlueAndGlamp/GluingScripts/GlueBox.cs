using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GlueBox : MonoBehaviour 
{
    public List<GluePlane> GluingPlanes;
    public SnapPoint PointToActivate;
    public bool ReadyToConnect { get; set; }

    //[Header("Valid Glue Range")]
    //public float MinValueToActivatePointAndLowScore = 60.0f;
    //public float MinValueForPerfectScore = 90.0f;
    //public float MaxGlueAmountBeforeTooMuch = 100.0f;

    private float currentGlueAmount;
    //private bool minimumGlueAmountReached = false;
    //private bool perfectGlueAmountReached = false;

	void Start () 
    {
        currentGlueAmount = 0.0f;
        //PointToActivate.CanConnect = false;
        //PointToActivate.HidePoint();
	}

    void Update()
    {

    }

    public void ApplyGlue(float amount, float maxAmount)
    {
        Debug.Log(gameObject.name + " is being increased by " + amount + " units of glue.");
        //currentGlueAmount += amount * Time.deltaTime;
        //foreach (GluePlane p in GluingPlanes)
        //{
        //    p.UpdatePlane(currentGlueAmount, maxAmount);
        //}
    }

    //public float GetTotalGlueApplied()
    //{
    //    if (currentGlueAmount > MaxGlueAmountBeforeTooMuch)
    //    {
    //        float amountPassedMax = currentGlueAmount - MaxGlueAmountBeforeTooMuch;
    //        return MaxGlueAmountBeforeTooMuch - amountPassedMax;
    //    }
    //    return currentGlueAmount;
    //}

    //public bool IsMinimumGlueAmountReached()
    //{
    //    return minimumGlueAmountReached;
    //}

    //public float GetValidMaxGlueAmount()
    //{
    //    return MaxGlueAmountBeforeTooMuch;
    //}
}

/*In Update*/
//if (Input.GetMouseButton(0) || Input.touchCount == 1)
//{
//    RaycastHit hit;
//    Ray ray = Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f));
//    if (Physics.Raycast(ray, out hit) && hit.collider == objCollider && previousHitPoint != hit.point)
//    {
//        if(currentGlueAmount <= (MaxGlueAmountBeforeTooMuch * 2.0f))
//        {
//            currentGlueAmount += GlueApplicationRate * Time.deltaTime;
//        }
//        if (currentGlueAmount >= MinValueToActivatePointAndLowScore && !minimumGlueAmountReached)
//        {
//            ReadyToConnect = true;
//            minimumGlueAmountReached = true;
//            PointToActivate.CanConnect = true;
//            PointToActivate.DisplayPoint();
//        }
//        if (currentGlueAmount >= MinValueForPerfectScore && !perfectGlueAmountReached)
//        {
//            perfectGlueAmountReached = true;
//        }
//        foreach (GluePlane p in GluingPlanes)
//        {
//            p.UpdatePlane(currentGlueAmount, MaxGlueAmountBeforeTooMuch);
//        }
//        previousHitPoint = hit.point;
//    }
//}
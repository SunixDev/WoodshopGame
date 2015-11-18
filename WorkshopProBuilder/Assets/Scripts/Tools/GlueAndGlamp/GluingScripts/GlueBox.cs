using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GlueBox : MonoBehaviour 
{
    public List<GluePlane> GluingPlanes;
    public SnapPoint PointToActivate;
    public bool MinimumReached { get; set; }

    private float currentGlueAmount;

    void Start()
    {
        currentGlueAmount = 0.0f;
        PointToActivate.DeactivatePoint();
    }

    public void ApplyGlue(PlayerGlue playerGlue)
    {
        if (currentGlueAmount <= playerGlue.MaxAmount * 2f)
        {
            currentGlueAmount += playerGlue.ApplicationRate * Time.deltaTime;
            if (currentGlueAmount >= playerGlue.AmountToActivateSnapPoints && !MinimumReached)
            {
                MinimumReached = true;
                PointToActivate.ActivatePoint();
            }
            foreach (GluePlane p in GluingPlanes)
            {
                p.UpdatePlane(currentGlueAmount, playerGlue.MaxAmount);
            }
        }
    }

    public float GetTotalGlueApplied()
    {
        return currentGlueAmount;
    }
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
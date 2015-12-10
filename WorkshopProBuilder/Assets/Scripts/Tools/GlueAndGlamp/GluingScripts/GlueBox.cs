using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GlueBox : MonoBehaviour 
{
    public List<GluePlane> GluingPlanes;
    public SnapPoint PointToActivate;
    public bool ActiveInStep = false;
    public bool MinimumReached { get; private set; }

    private float currentGlueAmount;

    public void Initialize()
    {
        currentGlueAmount = 0.0f;
        PointToActivate.DeactivatePoint();
        foreach (GluePlane plane in GluingPlanes)
        {
            plane.Initialize();
        }
    }

    public void ApplyGlue(PlayerGlue playerGlue)
    {
        if (currentGlueAmount <= playerGlue.MaxAmount * 2f)
        {
            currentGlueAmount += playerGlue.ApplicationRate * Time.deltaTime;
            CheckSnapPointActivation(playerGlue.AmountToActivateSnapPoints);
            UpdateGluePlanes(playerGlue.MaxAmount);
        }
    }

    private void CheckSnapPointActivation(float requiredAmountToActivate)
    {
        if (currentGlueAmount >= requiredAmountToActivate && !MinimumReached)
        {
            MinimumReached = true;
            PointToActivate.ActivatePoint();
        }
    }

    private void UpdateGluePlanes(float playerGlueMaxAmount)
    {
        foreach (GluePlane p in GluingPlanes)
        {
            p.UpdatePlane(currentGlueAmount, playerGlueMaxAmount);
        }
    }

    public float CalculatePercentage(float maxAmount)
    {
        float finalAmount = currentGlueAmount;
        if(currentGlueAmount > maxAmount)
        {
            float excessiveAmount = currentGlueAmount - maxAmount;
            finalAmount = maxAmount - excessiveAmount;
        }
        return finalAmount;
    }

    public float GetTotalGlueApplied()
    {
        return currentGlueAmount;
    }
}
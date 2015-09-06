using UnityEngine;
using System.Collections;

public class StepID : MonoBehaviour 
{
    public int StepNumber;

    public bool UsedInStep(int stepNumber)
    {
        return (StepNumber == stepNumber);
    }
}

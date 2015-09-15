using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ProjectState
{
    NotStarted,
    InProgress,
    Drying,
    Completed,
    Failed
}

public class Project : MonoBehaviour 
{
    public string Name;
    public float TotalValue;
    public List<Step> ProjectSteps;
    public bool Completed;
    public ProjectState state;
    public int CurrentStep;

    void Awake()
    {

    }

    public int GetCurrentStepNumber()
    {
        return CurrentStep + 1;
    }

    public int GetCurrentStepIndex()
    {
        return CurrentStep;
    }

    public Step GetCurrentStepObject()
    {
        return ProjectSteps[CurrentStep];
    }

    public void GoToNextStep()
    {
        CurrentStep++;
    }
}

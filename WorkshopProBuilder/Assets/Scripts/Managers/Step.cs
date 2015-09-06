using UnityEngine;
using System.Collections;
using System;

public enum ProjectStepState
{
    ReviewPlan,
    GetMaterials,
    UsingTool,
    DisplayResults
}

public class Step : MonoBehaviour 
{
    public ToolType ToolToUse;
    public Sprite PlanDrawing;
    public Requirements StepRequirements;
    public bool Completed { get; set; }
    public ProjectStepState state;

    public void StateChange(ProjectStepState state)
    {
        this.state = state;
    }
}

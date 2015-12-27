using UnityEngine;
using System.Collections;

public class ScoreManager : MonoBehaviour 
{
    [Header("Project Value")]
    public float ProjectCashValue;
    public int ProjectPointsValue;

    private float totalScore = 0f;
    private float scorePercentage = 100f;
    private int numberOfSteps = 0;

    public void ApplyScore(float points)
    {
        totalScore += points;
        numberOfSteps++;
        CalculatePercentage();
        Debug.Log("Applied Score: " + points);
        Debug.Log("Number of Completed Steps: " + numberOfSteps);
        Debug.Log("Current Total Score: " + totalScore);
        Debug.Log("Current Score Percentage: " + scorePercentage);
        Debug.Log("Current Project Value: " + CalculateCurrentCashValue());
    }

    public void ResetScore()
    {
        totalScore = 0f;
        scorePercentage = 100f;
        numberOfSteps = 0;
    }

    private void CalculatePercentage()
    {
        if (numberOfSteps == 0 )
        {
            scorePercentage = 100;
        }
        else
        {
            scorePercentage = totalScore / numberOfSteps;
            if (scorePercentage > 100f)
            {
                scorePercentage = 100f;
            }
        }
        
    }

    public float CalculateCurrentCashValue()
    {
        if (numberOfSteps == 0)
        {
            return ProjectCashValue;
        }
        else
        {
            float valueFromPercentage = ProjectCashValue * (scorePercentage / 100.0f);
            return valueFromPercentage;
        }
    }

    public float CalculateCurrentPointValue()
    {
        if (numberOfSteps == 0)
        {
            return ProjectPointsValue;
        }
        else
        {
            float valueFromPercentage = ProjectPointsValue * (scorePercentage / 100.0f);
            return valueFromPercentage;
        }
    }
}

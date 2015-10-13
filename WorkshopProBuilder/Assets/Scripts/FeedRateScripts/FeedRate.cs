using UnityEngine;
using System.Collections;

public class FeedRate : MonoBehaviour 
{
    public float MinPerfectRate;
    public float MaxPerfectRate;
    public float MaxFeedRate;
    public float ValueDecreasePerUpdate;
    public FeedRateBar RateDisplay;
    public bool RateTooSlow { get; set; }
    public bool RateTooFast { get; set; }

    private float LineScorePercentage = 100.0f;

    void Start()
    {
        RateTooSlow = false;
        RateTooFast = false;
    }

    public void UpdateScoreWithRate(float rate)
    {
        if (rate < MinPerfectRate || rate > MaxPerfectRate)
        {
            LineScorePercentage -= ValueDecreasePerUpdate;
        }
        RateTooSlow = (rate < MinPerfectRate);
        RateTooFast = (rate > MaxPerfectRate);
    }

    public void ReduceScoreDirectly(float amount)
    {
        LineScorePercentage -= amount;
    }

    public void UpdateDataDisplay(float rate)
    {
        RateDisplay.UpdateBar(rate, MaxFeedRate);
        RateDisplay.UpdateColor(rate, MaxFeedRate, MinPerfectRate, MaxPerfectRate);
    }

    public float GetLineScore()
    {
        return LineScorePercentage;
    }

    public void ResetFeedRate()
    {
        LineScorePercentage = 100.0f;
        RateDisplay.ResetDisplay();
        RateTooSlow = false;
        RateTooFast = false;
    }
}

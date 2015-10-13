using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FeedRateBar : MonoBehaviour 
{
    public RectTransform Indicator;
    public Image IndicatorImage;
    public Color BelowPerfectColor;
    public Color AtPerfectColor;
    public Color PassedPerfectColor;

    public float MaxWidth { get; set; }
    public float MinWidth { get; set; }

    void Start()
    {
        MinWidth = 0.0f;
        MaxWidth = Indicator.rect.width;
        Indicator.sizeDelta = new Vector2(MinWidth, Indicator.rect.height);
    }

    public void ResetDisplay()
    {
        Indicator.sizeDelta = new Vector2(MinWidth, Indicator.rect.height);
    }

    public void UpdateBar(float amount, float maxAmount)
    {
        if (amount <= maxAmount)
        {
            float percentage = (amount * 100) / maxAmount;
            float amountToApply = MaxWidth * (percentage / 100);
            Indicator.sizeDelta = new Vector2(amountToApply, Indicator.rect.height);
        }
    }

    public void UpdateColor(float amount, float maxAmount, float minRate, float maxRate)
    {
        float percentage = (amount * 100) / maxAmount;
        float amountToApply = MaxWidth * (percentage / 100);

        float minPercentage = (minRate * 100) / maxAmount;
        float min = MaxWidth * (minPercentage / 100);

        float maxPercentage = (maxRate * 100) / maxAmount;
        float max = MaxWidth * (maxPercentage / 100);

        if (Indicator.sizeDelta.x < min)
        {
            IndicatorImage.color = BelowPerfectColor;
        }
        else if (Indicator.sizeDelta.x >= min && Indicator.sizeDelta.x <= max)
        {
            IndicatorImage.color = (AtPerfectColor);
        }
        else if (Indicator.sizeDelta.x > max)
        {
            IndicatorImage.color = (PassedPerfectColor);
        }
    }

    public void ResetBar()
    {
        Indicator.sizeDelta = new Vector2(MinWidth, Indicator.rect.height);
    }
}

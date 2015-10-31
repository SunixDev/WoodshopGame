using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FinalScript : MonoBehaviour 
{
    public Text PercentageText;
    public Text OriginalValueText;
    public Text FinalScoreText;

    void Start() 
    {
        float percent = GameManager.instance.CalculatePercentage();
        PercentageText.text = percent.ToString("F1") + "/100";

        OriginalValueText.text = "$" + GameManager.instance.ProjectValue.ToString("F1");

        float finalValue = GameManager.instance.CalculateCurrentValue();
        FinalScoreText.text = "$" + finalValue.ToString("F1");
    }
}

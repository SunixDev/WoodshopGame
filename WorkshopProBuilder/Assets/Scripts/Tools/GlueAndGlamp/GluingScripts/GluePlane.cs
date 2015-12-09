using UnityEngine;
using System.Collections;

public enum IndicationType
{
    Highlight,
    Application
}

public class GluePlane : MonoBehaviour 
{
    private Color glueColor = Color.white;
    private Color highlightColor = new Color(0.98f, 1f, 0f, 1f);
    private Renderer objRenderer;
    private Vector3 fullScale;
    private IndicationType type = IndicationType.Highlight;
    private float excessiveGluePercentage = 1.1f;

    public void Initialize()
    {
        glueColor.a = 0.0f;
        objRenderer = GetComponent<Renderer>();
        fullScale = transform.localScale;
        objRenderer.sharedMaterial.color = highlightColor;
    }

    public void UpdatePlane(float currentGlueAmount, float maxGlueAmount)
    {
        if (type == IndicationType.Highlight)
        {
            type = IndicationType.Application;
            objRenderer.material.color = glueColor;
        }
        float percentageFilled = currentGlueAmount / maxGlueAmount; //Percentage between 1.0 and 0.0
        if (percentageFilled > excessiveGluePercentage)
            percentageFilled = excessiveGluePercentage;
        glueColor.a = percentageFilled;
        objRenderer.material.color = glueColor;
        transform.localScale = fullScale * percentageFilled;
    }
}

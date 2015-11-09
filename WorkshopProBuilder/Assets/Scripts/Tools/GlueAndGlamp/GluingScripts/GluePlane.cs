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

    void OnEnable()
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
        float percentageFilled = currentGlueAmount / maxGlueAmount;
        if (percentageFilled > 1.2f)
            percentageFilled = 1.2f;
        glueColor.a = percentageFilled;
        objRenderer.material.color = glueColor;
        transform.localScale = fullScale * percentageFilled;
    }
}

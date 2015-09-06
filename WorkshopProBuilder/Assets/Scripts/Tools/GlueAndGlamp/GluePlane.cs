using UnityEngine;
using System.Collections;

public class GluePlane : MonoBehaviour 
{
    public float MaxGlueAmount { get; set; }

    private Color GlueColor = Color.white;
    private Renderer objRenderer;
    private Vector3 expectedScale;

    void OnEnable()
    {
        GlueColor.a = 0.0f;
        objRenderer = GetComponent<Renderer>();
        objRenderer.material.color = GlueColor;

        expectedScale = transform.localScale;
        transform.localScale = Vector3.zero;
    }

    public void UpdatePlane(float currentGlueAmount)
    {
        float percentageFilled = currentGlueAmount / MaxGlueAmount;
        if (percentageFilled > 1.0f)
            percentageFilled = 1.0f;
        GlueColor.a = percentageFilled;
        objRenderer.material.color = GlueColor;
        transform.localScale = expectedScale * percentageFilled;
    }
}

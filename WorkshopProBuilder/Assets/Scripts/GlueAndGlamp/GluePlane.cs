using UnityEngine;
using System.Collections;

public class GluePlane : MonoBehaviour 
{
    public float MaxGlueAmount { get; set; }
    //public Vector3 MaxScale;

    private Color GlueColor = Color.white;
    private Renderer objRenderer;
    //private Vector3 currentScale;

	void Start () 
    {
        GlueColor.a = 0.0f;
        objRenderer = GetComponent<Renderer>();
        objRenderer.material.color = GlueColor;
        //currentScale = Vector3.zero;
        //transform.localScale = currentScale;
	}

    public void UpdatePlane(float currentGlueAmount)
    {
        float percentageFilled = currentGlueAmount / MaxGlueAmount;
        if (percentageFilled > 1.0f)
            percentageFilled = 1.0f;
        GlueColor.a = percentageFilled;
        objRenderer.material.color = GlueColor;

        //float scaleX = MaxScale.x * percentageFilled;
        //float scaleY = MaxScale.y * percentageFilled;
        //float scaleZ = MaxScale.z * percentageFilled;
    }
}

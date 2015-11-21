using UnityEngine;
using System.Collections;

public class ClampPoint : MonoBehaviour 
{
    public Vector3 LocalConnectionRotation;
    public bool Clamped { get; set; }
    public Vector3 Position
    {
        get
        {
            return transform.position;
        }
    }

    private Color NeutralColor = Color.yellow;
    private Color ActiveColor = Color.green;
    private Renderer objRenderer;

    void Awake()
    {
        Clamped = false;
        HidePoint();
        objRenderer = GetComponent<Renderer>();
        ChangeToNeutralColor();
    }

    public void DisplayPoint()
    {
        GetComponent<MeshRenderer>().enabled = true;
    }

    public void HidePoint()
    {
        GetComponent<MeshRenderer>().enabled = false;
    }

    public void ChangeToNeutralColor()
    {
        objRenderer.material.color = NeutralColor;
    }

    public void ChangeToActiveColor()
    {
        objRenderer.material.color = ActiveColor;
    }
}

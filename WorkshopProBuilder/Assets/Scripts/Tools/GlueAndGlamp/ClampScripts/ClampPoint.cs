using UnityEngine;
using System.Collections;

public class ClampPoint : MonoBehaviour 
{
    public Transform parentPiece;
    public Vector3 LocalConnectionRotation;
    public bool Clamped { get; set; }
    public Vector3 Position
    {
        get
        {
            return transform.position;
        }
    }

    //private Color neutralColor = Color.yellow;
    //private Color activeColor = Color.green;
    private Renderer objRenderer;

    void Awake()
    {
        Clamped = false;
        parentPiece = transform.parent;
        if (parentPiece == null)
        {
            Debug.LogError(gameObject + " needs to be the child of a Piece object");
        }
        //HidePoint();
        objRenderer = GetComponent<Renderer>();
        //ChangeToNeutralColor();
    }

    public void DisplayPoint()
    {
        GetComponent<MeshRenderer>().enabled = true;
    }

    public void HidePoint()
    {
        GetComponent<MeshRenderer>().enabled = false;
    }

    public Transform GetParentTransform()
    {
        return parentPiece;
    }

    //public void ChangeToNeutralColor()
    //{
    //    objRenderer.material.color = neutralColor;
    //}

    //public void ChangeToActiveColor()
    //{
    //    objRenderer.material.color = activeColor;
    //}
}

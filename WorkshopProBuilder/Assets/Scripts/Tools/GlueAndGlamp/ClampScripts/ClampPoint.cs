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

    private Transform parentPiece;
    private Renderer objRenderer;

    void Awake()
    {
        Clamped = false;
        parentPiece = transform.parent;
        if (parentPiece == null)
        {
            Debug.LogError(gameObject + " needs to be the child of a Piece object");
        }
        HidePoint();
        objRenderer = GetComponent<Renderer>();
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
}

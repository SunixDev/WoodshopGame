using UnityEngine;
using System.Collections;

public class ClampPoint : MonoBehaviour 
{
    //public GameObject ParentPiece;
    public Vector3 LocalConnectionRotation;
    public bool Clamped { get; set; }
    public Vector3 Position
    {
        get
        {
            return transform.position;
        }
    }

    void Awake()
    {
        Clamped = false;
        HidePoint();
    }

    public void DisplayPoint()
    {
        GetComponent<MeshRenderer>().enabled = true;
    }

    public void HidePoint()
    {
        GetComponent<MeshRenderer>().enabled = false;
    }
}

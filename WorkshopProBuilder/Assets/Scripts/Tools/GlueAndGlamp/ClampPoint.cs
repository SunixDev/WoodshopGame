using UnityEngine;
using System.Collections;

public class ClampPoint : MonoBehaviour 
{
    public GameObject ParentPiece;
    public Vector3 ConnectionRotation;
    public Vector3 Position
    {
        get
        {
            return transform.position;
        }
    }

    void Start()
    {
        HidePoint();
    }

    //void OnEnable()
    //{
    //    //GetComponent<MeshRenderer>().enabled = false;
    //}

    public void DisplayPoint()
    {
        GetComponent<MeshRenderer>().enabled = true;
    }

    public void HidePoint()
    {
        GetComponent<MeshRenderer>().enabled = false;
    }
}

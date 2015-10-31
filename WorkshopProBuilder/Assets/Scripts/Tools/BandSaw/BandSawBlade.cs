using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BandSawBlade : MonoBehaviour 
{
    public bool SawActive = false;
    public Collider WhileOffBladeCollider;
    public Color ActivatedColor;
    public Color DeactivatedColor;

    public bool CuttingWoodBoard { get; private set; }
    public bool NoInteractionWithBoard { get; private set; }
    public Vector3 BladePoint
    {
        get
        {
            return transform.position;
        }
        set
        {
            transform.position = value;
        }
    }

    private Vector3 originalBladePosition;
    //private List<GameObject> HitObjects;

    void Awake()
    {
        //HitObjects = new List<GameObject>();
        originalBladePosition = transform.position;
        NoInteractionWithBoard = true;
        CuttingWoodBoard = false;
        if (SawActive)
            TurnOn();
        else
            TurnOff();
    }

    void Update()
    {
        if (SawActive)
        {
            Ray ray = new Ray(BladePoint + new Vector3(0.0f, 0.1f, 0.0f), Vector3.down);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit) && (hit.collider.tag == "Piece" || hit.collider.tag == "Leftover" || hit.collider.tag == "DadoBlock"))
            {
                transform.position = hit.point;
                CuttingWoodBoard = true;
                NoInteractionWithBoard = false;
            }
            else
            {
                ResetEdgePosition();
                CuttingWoodBoard = false;
                NoInteractionWithBoard = true;
            }
        }
    }

    public void TurnOn()
    {
        SawActive = true;
        WhileOffBladeCollider.enabled = false;
        GetComponent<Renderer>().material.color = ActivatedColor;
    }

    public void TurnOff()
    {
        SawActive = false;
        WhileOffBladeCollider.enabled = true;
        GetComponent<Renderer>().material.color = DeactivatedColor;
    }

    public void ResetEdgePosition()
    {
        transform.position = originalBladePosition;
    }
}

//void OnTriggerEnter(Collider other)
//{
//    if ((other.tag == "Piece" || other.tag == "Leftover" || other.tag == "DadoBlock") && SawActive)
//    {
//        Ray ray = new Ray(BladePoint + new Vector3(0.0f, 1.0f, 0.0f), Vector3.down);
//        RaycastHit hit;
//        if (Physics.Raycast(ray, out hit) && (hit.collider.tag == "Piece" || hit.collider.tag == "Leftover" || hit.collider.tag == "DadoBlock"))
//        {
//            transform.position = hit.point;
//            CuttingWoodBoard = true;
//            NoInteractionWithBoard = false;
//            if(!HitObjects.Contains(other.gameObject))
//            {
//                HitObjects.Add(other.gameObject);
//            }
//        }
//    }
//}

//void OnTriggerExit(Collider other)
//{
//    if ((other.tag == "Piece" || other.tag == "Leftover" || other.tag == "DadoBlock") && SawActive)
//    {
//        Ray ray = new Ray(BladePoint + new Vector3(0.0f, 1.0f, 0.0f), Vector3.down);
//        RaycastHit hit;
//        bool otherColliderExited = true;
//        if (Physics.Raycast(ray, out hit))
//        {
//            if (hit.collider.tag == "Piece" || hit.collider.tag == "Leftover" || hit.collider.tag == "DadoBlock")
//            {
//                otherColliderExited = false;
//            }
//        }
//        if (otherColliderExited)
//        {
//            if (HitObjects.Contains(other.gameObject))
//            {
//                HitObjects.Remove(other.gameObject);
//            }
//            if (HitObjects.Count == 0)
//            {
//                CuttingWoodBoard = false;
//                NoInteractionWithBoard = true;
//                ResetEdgePosition();
//            }

//        }
//    }
//}

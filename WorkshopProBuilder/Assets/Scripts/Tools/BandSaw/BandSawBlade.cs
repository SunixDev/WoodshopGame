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

    void Awake()
    {
        originalBladePosition = transform.position;
        NoInteractionWithBoard = true;
        CuttingWoodBoard = false;
        if (SawActive)
            TurnOn();
        else
            TurnOff();
    }

    void OnTriggerEnter(Collider other)
    {
        if ((other.tag == "Piece" || other.tag == "Leftover" || other.tag == "DadoBlock") && SawActive)
        {
            CuttingWoodBoard = true;
            NoInteractionWithBoard = false;
            Ray ray = new Ray(BladePoint + new Vector3(0.0f, 1.0f, 0.0f), Vector3.down);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                transform.position = hit.point;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if ((other.tag == "Piece" || other.tag == "Leftover" || other.tag == "DadoBlock") && SawActive)
        {
            CuttingWoodBoard = false;
            NoInteractionWithBoard = true;
            ResetEdgePosition();
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

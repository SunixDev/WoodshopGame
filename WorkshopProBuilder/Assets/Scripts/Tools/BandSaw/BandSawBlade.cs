using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BandSawBlade : MonoBehaviour 
{
    public bool Active = false;
    public Collider WhileOffBladeCollider;
    public Color ActivatedColor;
    public Color DeactivatedColor;
    public List<GameObject> HitObjects;

    public bool MadeContactWithBoard { get; private set; }
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
        MadeContactWithBoard = false;
        CuttingWoodBoard = false;
        HitObjects = new List<GameObject>();
        if (Active)
            TurnOn();
        else
            TurnOff();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Piece" || other.tag == "Leftover" || other.tag == "DadoBlock")
        {
            MadeContactWithBoard = true;
            NoInteractionWithBoard = false;
            HitObjects.Add(other.gameObject);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Piece" || other.tag == "Leftover" || other.tag == "DadoBlock")
        {
            CuttingWoodBoard = true;
            NoInteractionWithBoard = false;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Piece" || other.tag == "Leftover" || other.tag == "DadoBlock")
        {
            NoInteractionWithBoard = true;
            MadeContactWithBoard = false;
            CuttingWoodBoard = false;
            HitObjects.Remove(other.gameObject);
        }
    }

    public int GetHitCount()
    {
        return HitObjects.Count;
    }

    public GameObject GetHitObjectAt(int index)
    {
        if (HitObjects.Count <= 0)
        {
            return null;
        }
        if (index >= HitObjects.Count)
        {
            return null;
        }
        return HitObjects[index];
    }

    public GameObject GetHitObjectWithTag(string otherTag)
    {
        if (HitObjects.Count <= 0)
        {
            return null;
        }
        GameObject objToReturn = null;
        bool found = false;
        for (int i = 0; i < HitObjects.Count && !found; i++)
        {
            if (HitObjects[i].tag == otherTag)
            {
                objToReturn = HitObjects[i];
                found = true;
            }
        }
        return objToReturn;
    }

    public void TurnOn()
    {
        Active = true;
        WhileOffBladeCollider.enabled = false;
        GetComponent<Renderer>().material.color = ActivatedColor;
    }

    public void TurnOff()
    {
        Active = false;
        WhileOffBladeCollider.enabled = true;
        GetComponent<Renderer>().material.color = DeactivatedColor;
    }

    public void ResetEdgePosition()
    {
        transform.position = originalBladePosition;
    }
}

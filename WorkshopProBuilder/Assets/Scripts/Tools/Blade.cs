using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Blade : MonoBehaviour 
{
    public bool MadeContactWithBoard { get; set; }
    public bool CuttingWoodBoard { get; set; }
    public bool NoInteractionWithBoard { get; set; }
    public List<GameObject> HitObjects;

    void Awake()
    {
        NoInteractionWithBoard = true;
        MadeContactWithBoard = false;
        CuttingWoodBoard = false;
        HitObjects = new List<GameObject>();
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
        if(HitObjects.Count <= 0)
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
}

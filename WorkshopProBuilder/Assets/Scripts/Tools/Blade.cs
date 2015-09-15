using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Blade : MonoBehaviour 
{
    public bool MadeContactWithBoard { get; private set; }
    public bool CuttingWoodBoard { get; private set; }
    public bool NoInteractionWithBoard { get; private set; }

    public Transform BladeEdge;
    public List<GameObject> HitObjects;
    public BoxCollider Collider;
    public Rotate Rotation;
    public bool Active;

    private Vector3 originalBladeEdgePosition;    

    void Awake()
    {
        NoInteractionWithBoard = true;
        MadeContactWithBoard = false;
        CuttingWoodBoard = false;
        HitObjects = new List<GameObject>();
        originalBladeEdgePosition = BladeEdge.position;
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

    public void TurnOn()
    {
        Active = true;
        Rotation.EnableRotation(true);
        if (Collider != null)
        {
            Collider.enabled = false;
        }
    }

    public void TurnOff()
    {
        Active = false;
        Rotation.EnableRotation(false);
        if (Collider != null)
        {
            Collider.enabled = true;
        }
        transform.localRotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, 0.0f));
    }

    public Vector3 EdgePosition()
    {
        return BladeEdge.position;
    }

    public void SetEdgePosition(Vector3 position)
    {
        BladeEdge.position = position;
    }

    public void ResetEdgePosition()
    {
        BladeEdge.position = originalBladeEdgePosition;
    }
}

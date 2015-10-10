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
    public BoxCollider BarrierCollider;
    public MeshCollider BladeCollider;
    public Rotate Rotation;
    public bool Active;

    private Vector3 originalBladeEdgePosition = Vector3.zero;    

    void Awake()
    {
        NoInteractionWithBoard = true;
        MadeContactWithBoard = false;
        CuttingWoodBoard = false;
        HitObjects = new List<GameObject>();
        if (BladeEdge != null)
        {
            originalBladeEdgePosition = BladeEdge.position;
        }
        if (Active)
            TurnOn();
        else
            TurnOff();
    }

    void OnTriggerEnter(Collider other)
    {
        if ((other.tag == "Piece" || other.tag == "Leftover" || other.tag == "DadoBlock") && Active)
        {
            MadeContactWithBoard = true;
            NoInteractionWithBoard = false;
            HitObjects.Add(other.gameObject);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if ((other.tag == "Piece" || other.tag == "Leftover" || other.tag == "DadoBlock") && Active)
        {
            CuttingWoodBoard = true;
            NoInteractionWithBoard = false;
            for (int i = 0; i < HitObjects.Count; i++)
            {
                Debug.Log(HitObjects[i]);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if ((other.tag == "Piece" || other.tag == "Leftover" || other.tag == "DadoBlock") && Active)
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

    public GameObject GetHitObjectByTag(string otherTag)
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

    public List<GameObject> GetAllObjectsByTag(string otherTag)
    {
        if (HitObjects.Count <= 0)
        {
            return null;
        }
        List<GameObject> objsToReturn = new List<GameObject>();
        for (int i = 0; i < HitObjects.Count; i++)
        {
            if (HitObjects[i].tag == otherTag)
            {
                objsToReturn.Add(HitObjects[i]);
            }
        }
        return objsToReturn;
    }

    public void TurnOn()
    {
        Active = true;
        Rotation.EnableRotation(true);
        if (BarrierCollider != null)
        {
            BarrierCollider.enabled = false;
        }
        BladeCollider.isTrigger = true;
    }

    public void TurnOff()
    {
        Active = false;
        Rotation.EnableRotation(false);
        if (BarrierCollider != null)
        {
            BarrierCollider.enabled = true;
        }
        transform.localRotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, 0.0f));
        BladeCollider.isTrigger = false;
    }

    public Vector3 EdgePosition()
    {
        if (BladeEdge == null)
        {
            return Vector3.zero;
        }
        return BladeEdge.position;
    }

    public void SetEdgePosition(Vector3 position)
    {
        if (BladeEdge != null)
        {
            BladeEdge.position = position;
        }
        else
        {
            Debug.Log("There is no edge point on this blade");
        }
    }

    public void ResetEdgePosition()
    {
        if (BladeEdge != null)
        {
            BladeEdge.position = originalBladeEdgePosition;
        }
        else
        {
            Debug.Log("There is no edge point on this blade");
        }
    }
}

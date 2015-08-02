using UnityEngine;
using System.Collections;

public class TableSawBlade : MonoBehaviour 
{
    public WoodMaterialManager BoardManager;
    public bool MadeContactWithBoard { get; set; }
    public bool CuttingWoodBoard { get; set; }
    public bool NoInteractionWithBoard { get; set; }

    void Awake()
    {
        NoInteractionWithBoard = true;
        MadeContactWithBoard = false;
        CuttingWoodBoard = false;
    }

    //void Update()
    //{
    //    Debug.Log("Contact Wood: " + MadeContactWithBoard);
    //    Debug.Log("Cutting Wood: " + CuttingWoodBoard);
    //    Debug.Log("No Interaction Wood: " + NoInteractionWithBoard);
    //}

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Piece" || other.tag == "Leftover")
        {
            MadeContactWithBoard = true;
            NoInteractionWithBoard = false;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Piece" || other.tag == "Leftover")
        {
            CuttingWoodBoard = true;
            NoInteractionWithBoard = false;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Piece" || other.tag == "Leftover")
        {
            NoInteractionWithBoard = true;
            MadeContactWithBoard = false;
            CuttingWoodBoard = false;
        }
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DadoCutManager : MonoBehaviour 
{
    //public WoodMaterialManager WoodManager;
    //public GameObject WoodMaterial;
    //public List<DadoBlock> DadoBlocks;
    //public Blade SawBlade;
    //public Transform BladeEdge;
    //public bool CuttingInProgress { get; set; }

    //private CutState state;
    //private DadoBlock CurrentDado;
    //private bool WithinDadoCut;
    //private Vector3 previousEndPosition;
    //private Vector3 originalBoardPosition; 

    //void Start () 
    //{
    //    state = CutState.ReadyToCut;
    //    CuttingInProgress = false;
    //    //originalBoardPosition = WoodMaterial.transform.position;
    //}
	
    //void Update () 
    //{
    //    if (state == CutState.ReadyToCut)
    //    {
    //        if (SawBlade.MadeContactWithBoard)
    //        {
    //            GameObject hit = SawBlade.GetHitObjectWithTag("DadoBlock");
    //            CurrentDado = hit.GetComponent<DadoBlock>();
    //            WithinDadoCut = CurrentDado.BlockArea.WithinDadoCut(BladeEdge.position);
    //            //Take off points if needed
    //            CurrentDado.ScaleDown();
    //            previousEndPosition = CurrentDado.CutLine.GetLastCheckpoint().GetPosition();
    //            state = CutState.Cutting;
    //            CuttingInProgress = true;
    //        }
    //    }
    //    else if (state == CutState.Cutting)
    //    {
    //        if (!PassedEndOfBoard())
    //        {
    //            if (WithinDadoCut)
    //            {
    //                float currentDistance = Vector3.Distance(BladeEdge.position, CurrentDado.CutLine.GetLastCheckpoint().GetPosition());
    //                float previousDistance = Vector3.Distance(BladeEdge.position, previousEndPosition);
    //                float deltaDistance = previousDistance - currentDistance;
    //                //Use to measure push consistency
    //            }
    //            else
    //            {
    //                //Reduce until restart is necessary
    //            }
    //        }
    //        else
    //        {
    //            state = CutState.EndOfCut;
    //        }
    //    }
    //    else if (state == CutState.EndOfCut)
    //    {
    //        if (!SawBlade.CuttingWoodBoard && SawBlade.NoInteractionWithBoard)
    //        {
    //            WoodMaterial.transform.position = originalBoardPosition;
    //            state = CutState.ReadyToCut;
    //            WithinDadoCut = false;
    //            CuttingInProgress = false;
    //            if (!CurrentDado.AnyCutsLeft())
    //            {
    //                Destroy(CurrentDado.gameObject);
    //            }
    //            CurrentDado = null;
    //        }
    //    }
    //}

    //private bool PassedEndOfBoard()
    //{
    //    bool passed = false;
    //    Vector3 difference = CurrentDado.CutLine.GetLastCheckpoint().GetPosition() - BladeEdge.position;
    //    if (difference.z >= 0)
    //    {
    //        passed = true;
    //    }
    //    return passed;
    //}
}

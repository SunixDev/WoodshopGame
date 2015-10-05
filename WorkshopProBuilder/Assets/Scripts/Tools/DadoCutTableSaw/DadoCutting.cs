using UnityEngine;
using System.Collections;

public class DadoCutting : MonoBehaviour 
{
    public DadoCutManager manager;
    public Blade SawBlade;
    public CutState CurrentState { get; set; }

    private DadoBlock CurrentDado;
    private Vector3 previousDadoPosition;

    void Start()
    {
        CurrentState = CutState.ReadyToCut;
    }

    void Update()
    {
        if (CurrentState == CutState.ReadyToCut && SawBlade.MadeContactWithBoard)
        {
            if (BladeHitPiece() && BladeHitDadoBlock())
            {
                //GameObject hit = SawBlade.GetHitObjectByTag("DadoBlock");
                //CurrentDado = hit.GetComponent<DadoBlock>();
                //previousDadoPosition = CurrentDado.Position;
                //CurrentState = CutState.Cutting;
                //Reduce score by small amount
                Debug.Log("Hit piece and dado block");
            }
            else if (BladeHitDadoBlock())
            {
                //GameObject hit = SawBlade.GetHitObjectByTag("DadoBlock");
                //CurrentDado = hit.GetComponent<DadoBlock>();
                //previousDadoPosition = CurrentDado.Position;
                //CurrentState = CutState.Cutting;
                Debug.Log("Only hit dado block");
            }
            else if (BladeHitPiece())
            {
                Debug.Log("Only hit piece and losing points");
            }
        }
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
    }

    private bool BladeHitDadoBlock()
    {
        GameObject dadoHit = SawBlade.GetHitObjectByTag("DadoBlock");
        bool dadoWasHit = (dadoHit != null);
        return dadoWasHit;
    }

    private bool BladeHitPiece()
    {
        GameObject hitPiece = SawBlade.GetHitObjectByTag("Piece");
        bool pieceWashit = (hitPiece != null);
        return pieceWashit;
    }

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

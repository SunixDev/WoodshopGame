﻿using UnityEngine;
using System.Collections;

public class DadoCutting : MonoBehaviour 
{
    public DadoCutManager manager;
    public Blade SawBlade;
    public float MaxStallTime = 3.0f;
    //public float PushRateOffset = 0.001;
    public CutState CurrentState { get; set; }

    private DadoBlock currentDado;
    private bool cuttingOutDado;
    private Vector3 previousPiecePosition;
    private float timeStalling;

    void Start()
    {
        currentDado = null;
        cuttingOutDado = false;
        timeStalling = 0.0f;
        CurrentState = CutState.Waiting;
    }

    private void SwitchDado()
    {
        DadoBlock nearestDado = manager.GetNearestDadoBlock(SawBlade.transform.position);
        currentDado = nearestDado;
    }

    private void StartWoodCutting()
    {
        if (BladeHitDadoBlock())
        {
            previousPiecePosition = currentDado.Position;
            cuttingOutDado = true;
            if (BladeHitPiece())
            {
                //Reduce score by small amount
                Debug.Log("Hit piece and dado block: lost some points");
            }
            else
            {
                Debug.Log("Only hit dado block");
            }
        }
        else if (BladeHitPiece())
        {
            Debug.Log("Only hit piece and losing points gradually");
            cuttingOutDado = false;
        }
    }

    private float TrackPushRate()
    {
        Vector3 currentPosition = manager.GetCurrentBoardPosition();
        Vector3 deltaVector = currentPosition - previousPiecePosition;
        previousPiecePosition = currentPosition;
        return deltaVector.magnitude;
    }

    void Update()
    {
        #region CuttingCode
        if (manager.DadosToCut.Count > 0)
        {
            if (CurrentState == CutState.Waiting)
            {
                SwitchDado();

                if (SawBlade.MadeContactWithBoard && SawBlade.Active)
                {
                    CurrentState = CutState.ReadyToCut;
                    manager.EnableBoardPhysics(false);
                }
            }
            else if (CurrentState == CutState.ReadyToCut)
            {
                StartWoodCutting();
                CurrentState = CutState.Cutting;
                manager.RestrictCurrentBoardMovement(false, true);
            }
            else if (CurrentState == CutState.Cutting && SawBlade.Active)
            {

            }
            else if (CurrentState == CutState.EndOfCut)
            {

            }
            //    else if (CurrentState == CutState.Cutting && SawBlade.Active)
            //    {
            //        if (cuttingOutDado)
            //        {
            //            if (SawBlade.CuttingWoodBoard)
            //            {
            //                float pushRate = TrackPushRate();
            //                Debug.Log("Push Rate: " + pushRate);
            //                if (pushRate == 0)
            //                {
            //                    timeStalling += Time.deltaTime;
            //                    if (timeStalling >= MaxStallTime)
            //                    {
            //                        //Wood burnt, Start over
            //                    }
            //                }
            //                else
            //                {
            //                    timeStalling = 0.0f;
            //                    //Calculate push rate is within consistent rate
            //                    //Lose points if too slow or too fast
            //                }
            //            }
            //            else if (SawBlade.NoInteractionWithBoard)
            //            {
            //                CurrentState = CutState.EndOfCut;
            //                manager.EnableBoardPhysics(true);
            //            }
            //        }
            //        else
            //        {
            //            //Decrease value by certain amount until zero and need to start over
            //            if (SawBlade.NoInteractionWithBoard)
            //            {
            //                CurrentState = CutState.Waiting;
            //                currentDado = null;
            //                manager.RestrictCurrentBoardMovement(false, false);
            //                manager.EnableBoardPhysics(true);
            //            }
            //        }
            //    }
            //}
            //else if (CurrentState == CutState.EndOfCut)
            //{
            //    if (!SawBlade.CuttingWoodBoard && SawBlade.NoInteractionWithBoard)
            //    {
            //        currentDado.ScaleDown();
            //        if (!currentDado.AnyCutsLeft())
            //        {
            //            //method in manager that handles removing dado game object
            //            Destroy(currentDado.gameObject);
            //        }
            //        currentDado = null;
            //        cuttingOutDado = false;
            //        timeStalling = 0.0f;
            //        CurrentState = CutState.Waiting;
            //    }
            //}
        #endregion
        }
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
    //    Vector3 difference = currentDado.CutLine.GetLastCheckpoint().GetPosition() - BladeEdge.position;
    //    if (difference.z >= 0)
    //    {
    //        passed = true;
    //    }
    //    return passed;
    //}
}

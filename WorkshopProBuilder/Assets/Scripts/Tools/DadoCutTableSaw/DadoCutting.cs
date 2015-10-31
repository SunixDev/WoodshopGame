using UnityEngine;
using System.Collections;

public class DadoCutting : MonoBehaviour 
{
    public DadoCutManager manager;
    public Blade SawBlade;
    public float MaxStallTime = 3.0f;
    public FeedRate FeedRateTracker;
    public CutState CurrentState { get; set; }
    public Transform BladeStart;
    public Transform BladeEnd;

    private DadoBlock currentDado = null;
    private bool cuttingOutDado = false;
    private Vector3 currentPiecePosition;
    private Vector3 previousPiecePosition;

    private float playerFeedRate = 0.0f;
    private float playerSmoothingVelocity = 0.0f;

    private float totalTimePassed = 0.0f;
    private float timeUpdateFrequency = 0.1f;

    private float totalTimeStalling = 0.0f;

    private float timeNotCuttingLine = 0.0f;

    void Start()
    {
        CurrentState = CutState.ReadyToCut;
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
                FeedRateTracker.ReduceScoreDirectly(0.5f);
            }
        }
        else if (BladeHitPiece())
        {
            FeedRateTracker.ReduceScoreDirectly(0.8f);
            cuttingOutDado = false;
        }
    }

    private float TrackPushRate()
    {
        Vector3 deltaVector = currentPiecePosition - previousPiecePosition;
        deltaVector = new Vector3(deltaVector.x, 0.0f, deltaVector.z);
        float unitsPerSecond = deltaVector.magnitude / Time.deltaTime;
        return Mathf.SmoothDamp(playerFeedRate, unitsPerSecond, ref playerSmoothingVelocity, 0.3f);
    }

    private void UpdateFeedRateData()
    {
        if (SawBlade.SawBladeActive)
        {
            playerFeedRate = TrackPushRate();
        }
        else
        {
            playerFeedRate = 0.0f;
        }
        FeedRateTracker.UpdateDataDisplay(playerFeedRate);
    }

    void Update()
    {
        #region CuttingCode
        if (manager.StillCutting)
        {
            currentPiecePosition = manager.GetCurrentBoardPosition();
            totalTimePassed += Time.deltaTime;
            UpdateFeedRateData();

            if (CurrentState == CutState.ReadyToCut)
            {
                SwitchDado();

                if (SawBlade.CuttingWoodBoard && SawBlade.SawBladeActive)
                {
                    StartWoodCutting();
                    CurrentState = CutState.Cutting;
                    manager.RestrictCurrentBoardMovement(false, true);
                }
            }
            else if (CurrentState == CutState.Cutting && SawBlade.SawBladeActive)
            {
                if (cuttingOutDado)
                {
                    if (SawBlade.NoInteractionWithBoard && !PieceWithinBlade())
                    {
                        CurrentState = CutState.EndOfCut;
                    }
                    else
                    {
                        if (totalTimePassed >= timeUpdateFrequency)
                        {
                            totalTimePassed = 0.0f;
                            FeedRateTracker.UpdateScoreWithRate(playerFeedRate);
                        }
                        if (FeedRateTracker.RateTooSlow || FeedRateTracker.RateTooFast)
                        {
                            totalTimeStalling += Time.deltaTime;
                            FeedRateTracker.ReduceScoreDirectly(0.1f);
                            if (totalTimeStalling >= MaxStallTime && FeedRateTracker.RateTooSlow)
                            {
                                manager.StopGameDueToLowScore("You were cutting too slow, now the wood is burnt.");
                            }
                            else if (totalTimeStalling >= 1.0f && FeedRateTracker.RateTooFast)
                            {
                                manager.StopGameDueToLowScore("You were cutting too fast and caused the saw to bind.");
                            }
                        }
                    }
                }
                else
                {
                    if (SawBlade.NoInteractionWithBoard && !PieceWithinBlade())
                    {
                        timeNotCuttingLine = 0.0f;
                        CurrentState = CutState.ReadyToCut;
                        SawBlade.ResetEdgePosition();
                        currentDado = null;
                        manager.RestrictCurrentBoardMovement(false, false);
                    }
                    else
                    {
                        timeNotCuttingLine += Time.deltaTime;
                        if (totalTimePassed >= timeUpdateFrequency)
                        {
                            totalTimePassed = 0.0f;
                            FeedRateTracker.ReduceScoreDirectly(0.6f);
                        }
                        if (timeNotCuttingLine >= MaxStallTime)
                        {
                            manager.StopGameDueToLowScore("You were not cutting into the marked dado cut, and now the board is ruined.");
                        }
                    }
                }
            }
            else if (CurrentState == CutState.EndOfCut)
            {
                if (!SawBlade.CuttingWoodBoard && SawBlade.NoInteractionWithBoard && !PieceWithinBlade())
                {
                    currentDado.ScaleDown();
                    if (!currentDado.AnyCutsLeft())
                    {
                        manager.DisplayScore(FeedRateTracker);
                        FeedRateTracker.ResetFeedRate();
                    }
                    manager.SplitMaterial(currentDado);
                    cuttingOutDado = false;
                    currentDado = null;
                    SawBlade.ResetEdgePosition();
                    CurrentState = CutState.ReadyToCut;
                    manager.RestrictCurrentBoardMovement(false, false);
                }
            }
        #endregion
            previousPiecePosition = currentPiecePosition;
            if (FeedRateTracker.GetLineScore() <= 0.0f)
            {
                manager.StopGameDueToLowScore("This cut is too messed up to keep going.");
            }
            if (totalTimePassed >= timeUpdateFrequency)
            {
                totalTimePassed = 0.0f;
            }
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

    private bool PieceWithinBlade()
    {
        RaycastHit hit;
        if (Physics.Raycast(BladeStart.position, -Vector3.up, out hit) && (hit.collider.tag == "Piece" || hit.collider.tag == "Leftover" || hit.collider.tag == "Dado"))
        {
            return true;
        }
        else if (Physics.Raycast(BladeEnd.position, -Vector3.up, out hit) && (hit.collider.tag == "Piece" || hit.collider.tag == "Leftover" || hit.collider.tag == "Dado"))
        {
            return true;
        }
        else
        {
            Vector3 fromStartToPiece = currentPiecePosition - BladeStart.position;
            Vector3 fromEndToPiece = currentPiecePosition - BladeEnd.position;
            if (fromStartToPiece.z > 0.0f && fromEndToPiece.z < 0.0f)
            {
                return true;
            }
        }
        return false;
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

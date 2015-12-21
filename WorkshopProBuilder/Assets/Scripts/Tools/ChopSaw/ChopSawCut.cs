using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChopSawCut : MonoBehaviour 
{
    public ChopSawManager manager;
    public Blade SawBlade;
    public float ValidCutOffset = 0.005f;
    public float MaxStallTime = 3.0f;
    public FeedRate FeedRateTracker;
    public CutState CurrentState { get; set; }
    public LayerMask mask;

    private CutLine currentLine;
    private bool cuttingAlongLine;
    private Vector3 currentBladePosition;
    private Vector3 previousBladePosition;

    private float playerFeedRate = 0.0f;
    private float playerSmoothingVelocity = 0.0f;

    private float totalTimePassed = 0.0f;
    private float timeUpdateFrequency = 0.1f;

    private float totalTimeStalling = 0.0f;

    private float timeNotCuttingLine = 0.0f;

    void Start()
    {
        currentLine = null;
        cuttingAlongLine = false;
        CurrentState = CutState.ReadyToCut;
    }

    private void SwitchLine()
    {
        CutLine nearestLine = manager.GetNearestLine(SawBlade.transform.position);
        if (nearestLine.IsMarked && nearestLine.LineMark != null)
        {
            if (nearestLine.LineMark.GetComponent<LineMark>().GoodLineMark)
            {
                nearestLine.DisplayLine(true, true);
            }
        }
        if (currentLine != null && currentLine != nearestLine)
        {
            currentLine.DisplayLine(false, true);
        }
        currentLine = nearestLine;
    }

    private void StartWoodCutting(Vector3 cutStartPoint)
    {
        SawBlade.SetEdgePosition(cutStartPoint);
        currentLine.DetermineCutDirection(SawBlade.EdgePosition());

        float distanceFromBlade = currentLine.CalculateDistance(SawBlade.EdgePosition());
        cuttingAlongLine = (distanceFromBlade <= ValidCutOffset);
        if (cuttingAlongLine && distanceFromBlade >= 0.003f)
        {
            FeedRateTracker.ReduceScoreDirectly(0.5f);
        }
        else if (!cuttingAlongLine)
        {
            FeedRateTracker.ReduceScoreDirectly(1.0f);
        }

        manager.RestrictCurrentBoardMovement(true, true);
        previousBladePosition = SawBlade.EdgePosition();
        CurrentState = CutState.Cutting;
        totalTimePassed = 0.0f;
    }

    private float TrackPushRate()
    {
        Vector3 deltaVector = currentBladePosition - previousBladePosition;
        deltaVector = new Vector3(0.0f, deltaVector.y, 0.0f);
        float unitsPerSecond = deltaVector.magnitude / Time.deltaTime;
        return Mathf.SmoothDamp(playerFeedRate, unitsPerSecond, ref playerSmoothingVelocity, 0.3f);
    }

    private void UpdateFeedRateDate()
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
            currentBladePosition = SawBlade.EdgePosition();
            totalTimePassed += Time.deltaTime;
            UpdateFeedRateDate();

            if (CurrentState == CutState.ReadyToCut)
            {
                SwitchLine();

                if (SawBlade.CuttingWoodBoard && SawBlade.SawBladeActive)
                {
                    Vector3 origin = SawBlade.EdgePosition() + new Vector3(0.0f, 0.5f, 0.0f);
                    Ray ray = new Ray(origin, Vector3.down);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask) && (hit.collider.tag == "Piece" || hit.collider.tag == "Leftover"))
                    {
                        Vector3 bladeEdgePosition = new Vector3(currentLine.GetCurrentCheckpointPosition().x, hit.point.y, hit.point.z);
                        StartWoodCutting(bladeEdgePosition);
                    }
                }
            }
            else if (CurrentState == CutState.Cutting && SawBlade.SawBladeActive)
            {
                if (cuttingAlongLine)
                {
                    currentLine.UpdateLine(SawBlade.EdgePosition());
                    if (currentLine.LineIsCut())
                    {
                        CurrentState = CutState.EndOfCut;
                    }
                    else
                    {
                        if (totalTimePassed >= timeUpdateFrequency)
                        {
                            totalTimePassed = 0.0f;
                            totalTimeStalling = 0.0f;
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
                    if (SawBlade.NoInteractionWithBoard)
                    {
                        timeNotCuttingLine = 0.0f;
                        CurrentState = CutState.ReadyToCut;
                        SawBlade.ResetEdgePosition();
                        currentLine = null;
                        manager.RestrictCurrentBoardMovement(false, false);
                    }
                    else
                    {
                        timeNotCuttingLine += Time.deltaTime;
                        if (totalTimePassed >= timeUpdateFrequency)
                        {
                            totalTimePassed = 0.0f;
                            FeedRateTracker.ReduceScoreDirectly(0.8f);
                        }
                        if (timeNotCuttingLine >= MaxStallTime)
                        {
                            manager.StopGameDueToLowScore("You were not cutting along the line, and now the board is ruined.");
                        }
                    }
                }
            }
            else if (CurrentState == CutState.EndOfCut)
            {
                if (!SawBlade.CuttingWoodBoard && SawBlade.NoInteractionWithBoard)
                {
                    manager.DisplayScore(FeedRateTracker);
                    manager.SplitMaterial(currentLine);
                    cuttingAlongLine = false;
                    currentLine = null;
                    SawBlade.ResetEdgePosition();
                    CurrentState = CutState.ReadyToCut;
                    FeedRateTracker.ResetFeedRate();
                }
            }
        }
        previousBladePosition = currentBladePosition;
        if (FeedRateTracker.GetLineScore() <= 0.0f)
        {
            manager.StopGameDueToLowScore("This cut is too messed up to keep going.");
        }
        #endregion
    }

    void OnDisable()
    {
        if (currentLine != null)
        {
            currentLine.DisplayLine(false, true);
            currentLine = null;
        }
        CurrentState = CutState.ReadyToCut;
        SawBlade.ResetEdgePosition();
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChopSawCut : MonoBehaviour 
{
    public ChopSawManager manager;
    public Blade SawBlade;
    public float ValidCutOffset = 0.005f;
    public float MaxStallTime = 3.0f;
    //public float PushRateOffset = 0.001;
    public CutState CurrentState { get; set; }
    public LayerMask mask;

    private CutLine currentLine;
    private bool cuttingAlongLine;
    private Vector3 previousBladePosition;
    private float timeStalling;

    void Start()
    {
        currentLine = null;
        cuttingAlongLine = false;
        timeStalling = 0.0f;
        CurrentState = CutState.ReadyToCut;
    }

    private void SwitchLine()
    {
        CutLine nearestLine = manager.GetNearestLine(SawBlade.transform.position);
        if (nearestLine.IsMarked)
        {
            nearestLine.DisplayLine(true, true);
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

        manager.RestrictCurrentBoardMovement(true, true);
        previousBladePosition = SawBlade.EdgePosition();
        CurrentState = CutState.Cutting;
    }

    private float TrackPushRate()
    {
        Vector3 currentPosition = SawBlade.EdgePosition();
        Vector3 deltaVector = currentPosition - previousBladePosition;
        previousBladePosition = currentPosition;
        return deltaVector.magnitude;
    }

    void Update()
    {
        #region CuttingCode
        if (manager.LinesToCut.Count > 0)
        {
            if (CurrentState == CutState.ReadyToCut)
            {
                SwitchLine();

                if (SawBlade.MadeContactWithBoard && SawBlade.Active)
                {
                    Vector3 origin = SawBlade.EdgePosition() + new Vector3(0.0f, 0.5f, 0.0f);
                    Ray ray = new Ray(origin, Vector3.down);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit) && (hit.collider.tag == "Piece" || hit.collider.tag == "Leftover"))
                    {
                        Vector3 bladeEdgePosition = new Vector3(currentLine.GetCurrentCheckpointPosition().x, hit.point.y, hit.point.z);
                        StartWoodCutting(hit.point);
                    }
                }
            }
            else if (CurrentState == CutState.Cutting && SawBlade.Active)
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
                        float pushRate = TrackPushRate();
                        Debug.Log("Push Rate: " + pushRate);
                        if (pushRate == 0)
                        {
                            timeStalling += Time.deltaTime;
                            if (timeStalling >= MaxStallTime)
                            {
                                //Wood burnt, Start over
                                Debug.Log("Wood is burnt, Start over");
                            }
                        }
                        else
                        {
                            timeStalling = 0.0f;
                            //Calculate push rate is within consistent rate
                            //Lose points if too slow or too fast
                        }
                    }
                }
                else
                {
                    //Decrease value by certain amount until zero and need to start over
                    if (SawBlade.NoInteractionWithBoard)
                    {
                        CurrentState = CutState.ReadyToCut;
                        SawBlade.ResetEdgePosition();
                        currentLine = null;
                        manager.RestrictCurrentBoardMovement(false, false);
                    }
                }
            }
            else if (CurrentState == CutState.EndOfCut)
            {
                if (!SawBlade.CuttingWoodBoard && SawBlade.NoInteractionWithBoard)
                {
                    manager.SplitMaterial(currentLine);
                    cuttingAlongLine = false;
                    currentLine = null;
                    SawBlade.ResetEdgePosition();
                    CurrentState = CutState.ReadyToCut;
                }
            }
        }
        #endregion
    }

}

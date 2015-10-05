using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TableSawCut : MonoBehaviour 
{
    public TableSawManager manager;
    public Blade SawBlade;
    public float ValidCutOffset = 0.005f;
    public CutState CurrentState { get; set; }

    private CutLine currentLine;
    private bool cuttingAlongLine;
    private Vector3 previousCheckpointPosition;
    private float timeWithoutPushing;

	void Start () 
    {
        currentLine = null;
        cuttingAlongLine = false;
        timeWithoutPushing = 0.0f;
        CurrentState = CutState.ReadyToCut;
	}

    void Update() 
    {
        #region CuttingCode
        if (SawBlade.Active)
        {
            if (CurrentState == CutState.ReadyToCut && SawBlade.MadeContactWithBoard && SawBlade.Active)
            {
                Vector3 origin = SawBlade.EdgePosition() + new Vector3(0.0f, 0.5f, 0.0f);
                Ray ray = new Ray(origin, Vector3.down);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit) && (hit.collider.tag == "Piece" || hit.collider.tag == "Leftover"))
                {
                    currentLine = manager.GetNearestLine(hit.point);
                    SawBlade.SetEdgePosition(hit.point);
                    cuttingAlongLine = BladeWithinValidCutOffset();
                    manager.SetCurrentBoardRestrictions(false, true);
                    CurrentState = CutState.Cutting;
                }
            }
            else if (CurrentState == CutState.Cutting)
            {
                if (currentLine.OnFirstCheckpoint() && SawBlade.CuttingWoodBoard)
                {
                    if (cuttingAlongLine)
                    {
                        if (PassedCurrentCheckpoint())
                        {
                            currentLine.UpdateToNextCheckpoint();
                            previousCheckpointPosition = currentLine.GetCurrentCheckpoint().GetPosition();
                        }
                    }
                    else
                    {
                        //Decrease value by certain amount
                        //Increase time until enough damage is made to start over
                        if (SawBlade.NoInteractionWithBoard)
                        {
                            CurrentState = CutState.ReadyToCut;
                            SawBlade.ResetEdgePosition();
                            currentLine = null;
                            manager.SetCurrentBoardRestrictions(true, true);
                        }
                    }
                }
                else if ((currentLine.OnConnectedCheckpoint() || currentLine.OnLastCheckpoint()) && SawBlade.CuttingWoodBoard)
                {
                    Vector3 currentPointPosition = currentLine.GetCurrentCheckpoint().GetPosition();
                    float deltaDistance = Vector3.Distance(previousCheckpointPosition, currentPointPosition);
                    Debug.Log(deltaDistance);
                    //Distance determines push rate, which can lower score if too fast to too slow
                    //if (distance == 0)
                    //{
                    //    timeWithoutPushing += Time.deltaTime;
                    //    if (timeWithoutPushing >= 1.0f)
                    //    {
                    //        //Burnt wood; Start over;
                    //    }
                    //}
                    //else
                    //{
                    //    timeWithoutPushing = 0.0f;
                    //}

                    if (PassedCurrentCheckpoint())
                    {
                        currentLine.UpdateToNextCheckpoint();
                        if (currentLine.GetCurrentCheckpoint() == null)
                        {
                            CurrentState = CutState.EndOfCut;
                        }
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

    public void ActivateSaw(bool active)
    {
        
    }

    private bool BladeWithinValidCutOffset()
    {
        Vector3 edge = new Vector3(SawBlade.EdgePosition().x, 0.0f, 0.0f);
        Vector3 checkpoint = new Vector3(currentLine.GetCurrentCheckpoint().GetPosition().x, 0.0f, 0.0f);

        float distance = Vector3.Distance(edge, checkpoint);
        return (distance <= ValidCutOffset);
    }

    private float DistanceBetweenBladeAndLine(Vector3 bladeEdge, Vector3 currentCheckpoint, Vector3 origin)
    {
        Vector3 toEdge = bladeEdge - origin;
        Vector3 toCheckpoint = currentCheckpoint - origin;
        toCheckpoint.Normalize();

        float projection = Vector3.Dot(toEdge, toCheckpoint);
        toCheckpoint = toCheckpoint * projection;
        Vector3 rejectionVector = toEdge - toCheckpoint;

        return rejectionVector.magnitude;
    }

    private bool PassedCurrentCheckpoint()
    {
        bool passed = false;
        Vector3 difference = currentLine.GetCurrentCheckpoint().GetPosition() - SawBlade.EdgePosition();
        if (difference.z >= 0)
        {
            passed = true;
        }
        return passed;
    }
}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChopSawCut : MonoBehaviour 
{
    public ChopSawManager manager;
    public Blade SawBlade;
    public float ValidCutOffset = 0.005f;
    public CutState CurrentState { get; set; }
    public LayerMask mask;

    private CutLine currentLine;
    private bool cuttingAlongLine;
    private Vector3 previousBladeEdgePosition;
    private float timeWithoutPushing;

    void Start()
    {
        currentLine = null;
        cuttingAlongLine = false;
        timeWithoutPushing = 0.0f;
        CurrentState = CutState.ReadyToCut;
    }

    void Update()
    {
        #region CuttingCode
        if (CurrentState == CutState.ReadyToCut && SawBlade.MadeContactWithBoard && SawBlade.Active)
        {
            Ray ray = new Ray(SawBlade.EdgePosition(), Vector3.down);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, mask) && (hit.collider.tag == "Piece" || hit.collider.tag == "Leftover"))
            {
                currentLine = manager.GetNearestLine(hit.point);
                Vector3 bladeEdgePosition = new Vector3(hit.point.x, hit.point.y, currentLine.GetCurrentCheckpoint().GetPosition().z);
                SawBlade.SetEdgePosition(bladeEdgePosition);
                previousBladeEdgePosition = SawBlade.EdgePosition();
                cuttingAlongLine = BladeWithinValidCutOffset();
                manager.EnableCurrentBoardMovement(false);
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
                        float distance = DistanceBetweenBladeAndLine(SawBlade.EdgePosition(), currentLine.GetCurrentCheckpoint().GetPosition(), currentLine.GetPreviousCheckpoint().GetPosition());
                        //Use distance to determine score
                        previousBladeEdgePosition = SawBlade.EdgePosition();
                    }
                }
                else
                {
                    //Decrease value by certain amount
                    //Increase time cutting until enough damage is made to start over
                    if (SawBlade.NoInteractionWithBoard)
                    {
                        CurrentState = CutState.ReadyToCut;
                        SawBlade.ResetEdgePosition();
                        currentLine = null;
                        manager.EnableCurrentBoardMovement(true);
                    }
                }
            }
            else if ((currentLine.OnConnectedCheckpoint() || currentLine.OnLastCheckpoint()) && SawBlade.CuttingWoodBoard)
            {
                float deltaDistance = Vector3.Distance(previousBladeEdgePosition, SawBlade.EdgePosition());
                Debug.Log(deltaDistance);
                //Distance determines push rate, which can lower score if too fast or too slow
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
            else if (SawBlade.NoInteractionWithBoard)
            {
                CurrentState = CutState.ReadyToCut;
                currentLine = null;
                cuttingAlongLine = false;
                timeWithoutPushing = 0.0f;
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
        #endregion
    }

    private bool BladeWithinValidCutOffset()
    {
        Vector3 edge = new Vector3(SawBlade.EdgePosition().x, SawBlade.EdgePosition().y, 0.0f);
        Vector3 checkpoint = new Vector3(currentLine.GetCurrentCheckpoint().GetPosition().x, currentLine.GetCurrentCheckpoint().GetPosition().y, 0.0f);

        float distance = Vector3.Distance(edge, checkpoint);
        return (distance <= ValidCutOffset);
    }

    private float DistanceBetweenBladeAndLine(Vector3 bladeEdge, Vector3 currentCheckpoint, Vector3 origin)
    {
        Vector3 toBladeEdge = bladeEdge - origin;
        Vector3 toCheckpoint = currentCheckpoint - origin;
        toCheckpoint.Normalize();

        float projection = Vector3.Dot(toBladeEdge, toCheckpoint);
        toCheckpoint = toCheckpoint * projection;
        Vector3 rejectionVector = toBladeEdge - toCheckpoint;

        return rejectionVector.magnitude;
    }

    private bool PassedCurrentCheckpoint()
    {
        bool passed = false;
        Vector3 difference = currentLine.GetCurrentCheckpoint().GetPosition() - SawBlade.EdgePosition();
        if (difference.y >= 0)
        {
            passed = true;
        }
        return passed;
    }

}

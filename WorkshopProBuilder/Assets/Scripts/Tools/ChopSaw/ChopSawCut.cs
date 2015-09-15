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
    private Vector3 previousBladePosition;
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
                CurrentState = CutState.Cutting;
                currentLine = manager.GetNearestLine(hit.point);
                Vector3 bladeEdgePosition = new Vector3(hit.point.x, hit.point.y, currentLine.GetCurrentCheckpoint().GetPosition().z);
                SawBlade.SetEdgePosition(bladeEdgePosition);
                cuttingAlongLine = BladeWithinValidCutOffset();
            }
        }
        //else if (state == CutState.Cutting)
        //{
        //    if (CuttingFirstCheckpoint() && SawBlade.CuttingWoodBoard)
        //    {
        //        if (cuttingAlongLine)
        //        {
        //            if (PassedCurrentCheckpoint())
        //            {
        //                currentLine.UpdateToNextCheckpoint();
        //                float distance = DistanceBetweenBladeAndLine(currentLine.GetCurrentCheckpoint().GetPosition(), BladeEdge.position, currentLine.GetPreviousCheckpoint().GetPosition());
        //                //Use distance to determine score
        //                previousBladePosition = BladeEdge.position;
        //            }
        //        }
        //        else
        //        {
        //            //Decrease value until player is forced to start over
        //        }
        //    }
        //    else if ((CuttingConnectedCheckpoint() || CuttingLastCheckpoint()) && SawBlade.CuttingWoodBoard)
        //    {
        //        float distance = Vector3.Distance(previousBladePosition, BladeEdge.position);
        //        //Distance determines push rate, which can lower score if too fast or too slow
        //        if (distance == 0)
        //        {
        //            timeWithoutPushing += Time.deltaTime;
        //            if (timeWithoutPushing >= 1.0f)
        //            {
        //                //Burnt wood; Start over;
        //            }
        //        }
        //        else
        //        {
        //            timeWithoutPushing = 0.0f;
        //        }

        //        if (PassedCurrentCheckpoint())
        //        {
        //            currentLine.UpdateToNextCheckpoint();
        //            if (currentLine.GetCurrentCheckpoint() == null)
        //            {
        //                state = CutState.EndOfCut;
        //            }
        //        }
        //    }
        //    else if (SawBlade.NoInteractionWithBoard)
        //    {
        //        state = CutState.ReadyToCut;
        //        currentLine = null;
        //        boardBeingCut = null;
        //        cuttingAlongLine = false;
        //        timeWithoutPushing = 0.0f;
        //    }
        //}
        //else if (state == CutState.EndOfCut)
        //{
        //    if(!SawBlade.NoInteractionWithBoard)
        //    {
        //        cuttingAlongLine = false;
        //        state = CutState.ReadyToCut;
        //        LinesToCut.Remove(currentLine);
        //        WoodManager.SplitBoard(currentLine.GetFirstBaseNode(), currentLine.GetSecondBaseNode(), boardBeingCut, currentLine);
        //        currentLine = null;
        //        boardBeingCut = null;
        //        BladeEdge.position = originalBladeEdgePosition;
        //    }
        //}
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
        if (difference.y >= 0)
        {
            passed = true;
        }
        return passed;
    }

}

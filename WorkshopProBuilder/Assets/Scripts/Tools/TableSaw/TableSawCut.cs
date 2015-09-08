using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TableSawCut : MonoBehaviour 
{
    public TableSawManager manager;
    public Transform BladeEdge;
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
        if (CurrentState == CutState.ReadyToCut && SawBlade.MadeContactWithBoard && SawBlade.Active)
        {
            Vector3 origin = BladeEdge.position + new Vector3(0.0f, 0.5f, 0.0f);
            Ray ray = new Ray(origin, Vector3.down);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit) && (hit.collider.tag == "Piece" || hit.collider.tag == "Leftover"))
            {
                CurrentState = CutState.Cutting;
                currentLine = manager.GetNearestLine(hit.point);
                BladeEdge.position = hit.point;
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
        //                previousCheckpointPosition = currentLine.GetCurrentCheckpoint().GetPosition();
        //            }
        //        }
        //        else
        //        {
        //            //Decrease value until player is forced to start over
        //        }
        //    }
        //    else if (( CuttingConnectedCheckpoint() || CuttingLastCheckpoint() ) && SawBlade.CuttingWoodBoard)
        //    {
        //        Vector3 currentPointPosition = currentLine.GetCurrentCheckpoint().GetPosition();
        //        float distance = Vector3.Distance(previousCheckpointPosition, currentPointPosition);
        //        //Distance determines push rate, which can lower score if too fast ro too slow
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
        //                boardBeingCut.GetComponent<BoardController>().Moveable = false;
        //            }
        //        }
        //    }
        //}
        //else if (state == CutState.EndOfCut)
        //{
        //    if (!SawBlade.CuttingWoodBoard && SawBlade.NoInteractionWithBoard)
        //    {
        //        cuttingAlongLine = false;
        //        boardBeingCut.GetComponent<BoardController>().RotationPoint = null;
        //        boardBeingCut.transform.position += new Vector3(0.0f, 0.0f, 0.8f * Time.deltaTime);
        //        state = CutState.ReadyToCut;
        //        LinesToCut.Remove(currentLine);
        //        currentLine = null;
        //        boardBeingCut = null;
        //        BladeEdge.position = originalBladeEdgePosition;
        //    }
        //}
        #endregion
    }

    public void ActivateSaw(bool active)
    {
        
    }

    private bool BladeWithinValidCutOffset()
    {
        Vector3 edge = new Vector3(BladeEdge.position.x, 0.0f, 0.0f);
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
        Vector3 difference = currentLine.GetCurrentCheckpoint().GetPosition() - BladeEdge.position;
        if (difference.z >= 0)
        {
            passed = true;
        }
        return passed;
    }
}
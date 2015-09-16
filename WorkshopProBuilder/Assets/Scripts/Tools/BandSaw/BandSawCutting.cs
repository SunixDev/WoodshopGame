using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BandSawCutting : MonoBehaviour 
{
    public BandSawManager manager;
    public BandSawBlade Blade;
    public CutState CurrentState { get; set; }
    public float ValidCutOffset = 0.005f;

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
	
	void Update () 
    {
        #region CuttingCode
        if (CurrentState == CutState.ReadyToCut && Blade.Active && Blade.CuttingWoodBoard)
        {
            Vector3 origin = Blade.BladePoint + new Vector3(0.0f, 0.3f, 0.0f);
            Ray ray = new Ray(origin, Vector3.down);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit) && (hit.collider.tag == "Piece" || hit.collider.tag == "Leftover"))
            {
                CurrentState = CutState.Cutting;
                currentLine = manager.GetNearestLine(hit.point);
                Blade.BladePoint = hit.point;
            }
        }
        //else if (state == CutState.Cutting)
        //{
        //    if (CuttingFirstCheckpoint())
        //    {
        //        if (PassedCurrentCheckpoint())
        //        {
        //            currentLine.UpdateToNextCheckpoint();
        //            previousCheckpointPosition = currentLine.GetCurrentCheckpoint().GetPosition();
        //        }
        //    }
        //    else if ((CuttingConnectedCheckpoint() || CuttingLastCheckpoint()))
        //    {
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
        //    float distance = Vector3.Distance(currentLine.GetPreviousCheckpoint().GetPosition(), BladeEdge.position);
        //    if (distance <= 0.01f)
        //    {
        //        boardBeingCut.transform.position += new Vector3(0.0f, 0.0f, 0.08f * Time.deltaTime);
        //    }
        //    else
        //    {
        //        cuttingAlongLine = false;
        //        boardBeingCut.transform.position += new Vector3(0.0f, 0.0f, 0.08f * Time.deltaTime);
        //        boardBeingCut.GetComponent<BoardController>().RotationPoint = null;
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
        Vector3 edge = new Vector3(Blade.BladePoint.x, 0.0f, Blade.BladePoint.z);
        Vector3 checkpoint = new Vector3(currentLine.GetCurrentCheckpoint().GetPosition().x, 0.0f, currentLine.GetCurrentCheckpoint().GetPosition().z);

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
        bool withinRange = BladeWithinValidCutOffset();
        bool passed = withinRange;
        if (withinRange)
        {
            Vector3 difference = currentLine.GetCurrentCheckpoint().GetPosition() - Blade.BladePoint;
            passed = (difference.z >= 0);
        }
        return passed;
    }
}

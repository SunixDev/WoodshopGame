using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BandSawCutting : MonoBehaviour 
{
    public WoodMaterialManager WoodManager;
    public List<CutLine> LinesToCut;
    public Transform BladeEdge;
    public CutState state { get; set; }
    public float ValidCutOffset = 0.005f;

    private CutLine currentLine;
    private WoodMaterialObject boardBeingCut;
    private Vector3 originalBladeEdgePosition;
    private bool cuttingAlongLine;
    private Vector3 previousCheckpointPosition;
    private float timeWithoutPushing;

	void Start () 
    {
        state = CutState.ReadyToCut;
        currentLine = null;
        boardBeingCut = null;
        originalBladeEdgePosition = BladeEdge.position;
        cuttingAlongLine = false;
        timeWithoutPushing = 0.0f;
	}
	
	void Update () 
    {
        #region CuttingCode
        if (state == CutState.ReadyToCut)
        {
            Vector3 origin = BladeEdge.position + new Vector3(0.0f, 0.3f, 0.0f);
            Ray ray = new Ray(origin, Vector3.down);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit) && (hit.collider.tag == "Piece" || hit.collider.tag == "Leftover"))
            {
                state = CutState.Cutting;
                currentLine = GetNearestLine(hit.point);
                boardBeingCut = hit.transform.gameObject.GetComponent<WoodMaterialObject>();
                boardBeingCut.GetComponent<BoardController>().RotationPoint = BladeEdge;
                BladeEdge.position = hit.point;
            }
        }
        else if (state == CutState.Cutting)
        {
            if (CuttingFirstCheckpoint())
            {
                if (PassedCurrentCheckpoint())
                {
                    currentLine.UpdateToNextCheckpoint();
                    previousCheckpointPosition = currentLine.GetCurrentCheckpoint().GetPosition();
                }
            }
            else if ((CuttingConnectedCheckpoint() || CuttingLastCheckpoint()))
            {
                if (PassedCurrentCheckpoint())
                {
                    currentLine.UpdateToNextCheckpoint();
                    if (currentLine.GetCurrentCheckpoint() == null)
                    {
                        state = CutState.EndOfCut;
                        boardBeingCut.GetComponent<BoardController>().Moveable = false;
                    }
                }
            }
        }
        else if (state == CutState.EndOfCut)
        {
            float distance = Vector3.Distance(currentLine.GetPreviousCheckpoint().GetPosition(), BladeEdge.position);
            if (distance <= 0.01f)
            {
                boardBeingCut.transform.position += new Vector3(0.0f, 0.0f, 0.08f * Time.deltaTime);
            }
            else
            {
                cuttingAlongLine = false;
                boardBeingCut.transform.position += new Vector3(0.0f, 0.0f, 0.08f * Time.deltaTime);
                boardBeingCut.GetComponent<BoardController>().RotationPoint = null;
                state = CutState.ReadyToCut;
                LinesToCut.Remove(currentLine);
                WoodManager.SplitBoard(currentLine.GetFirstBaseNode(), currentLine.GetSecondBaseNode(), boardBeingCut, currentLine);
                currentLine = null;
                boardBeingCut = null;
                BladeEdge.position = originalBladeEdgePosition;
            }
        }
        #endregion
	}

    private CutLine GetNearestLine(Vector3 fromPosition)
    {
        bool lineFound = false;
        int nearestLineIndex = -1;
        float smallestDistance = 0.0f;
        for (int i = 0; i < LinesToCut.Count && !lineFound; i++)
        {
            if (nearestLineIndex == -1)
            {
                nearestLineIndex = i;
                float firstDistance = Vector3.Distance(fromPosition, LinesToCut[i].Checkpoints[0].GetPosition());
                float lastDistance = Vector3.Distance(fromPosition, LinesToCut[i].Checkpoints[LinesToCut[i].Checkpoints.Count - 1].GetPosition());
                if(firstDistance < lastDistance)
                {
                    smallestDistance = firstDistance;
                    LinesToCut[i].CutBackwards = false;
                }
                else
                {
                    smallestDistance = lastDistance;
                    LinesToCut[i].CutBackwards = true;
                }
                
            }
            else
            {
                float firstDistance = Vector3.Distance(fromPosition, LinesToCut[i].Checkpoints[0].GetPosition());
                float lastDistance = Vector3.Distance(fromPosition, LinesToCut[i].Checkpoints[LinesToCut[i].Checkpoints.Count - 1].GetPosition());
                if (firstDistance < lastDistance)
                {
                    if (firstDistance < smallestDistance)
                    {
                        nearestLineIndex = i;
                        smallestDistance = firstDistance;
                        LinesToCut[i].CutBackwards = false;
                    }
                }
                else
                {
                    if (lastDistance < smallestDistance)
                    {
                        nearestLineIndex = i;
                        smallestDistance = lastDistance;
                        LinesToCut[i].CutBackwards = true;
                    }
                }
            }
        }
        return LinesToCut[nearestLineIndex];
    }

    private bool CuttingFirstCheckpoint()
    {
        return currentLine.GetPreviousCheckpoint() == null && currentLine.GetNextCheckpoint() != null;
    }

    private bool CuttingConnectedCheckpoint()
    {
        return currentLine.GetPreviousCheckpoint() != null && currentLine.GetNextCheckpoint() != null;
    }

    private bool CuttingLastCheckpoint()
    {
        return currentLine.GetPreviousCheckpoint() != null && currentLine.GetNextCheckpoint() == null;
    }

    private bool BladeWithinValidCutOffset()
    {
        Vector3 edge = new Vector3(BladeEdge.position.x, 0.0f, BladeEdge.position.z);
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
            Vector3 difference = currentLine.GetCurrentCheckpoint().GetPosition() - BladeEdge.position;
            passed = (difference.z >= 0);
        }
        return passed;
    }
}

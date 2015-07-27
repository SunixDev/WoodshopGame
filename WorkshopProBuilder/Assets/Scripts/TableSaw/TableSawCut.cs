using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TableSawCut : MonoBehaviour 
{
    public WoodMaterialManager WoodManager;
    public List<CutLine> LinesToCut;
    public Transform BladeEdge;
    public CutState state { get; set; }
    public float ValidCutOffset = 0.005f;

    private CutLine CurrentLine;
    private WoodMaterialObject BoardBeingCut;
    private Vector3 OriginalBladeEdgePosition;

	void Start () 
    {
        state = CutState.ReadyToCut;
        CurrentLine = null;
        BoardBeingCut = null;
        OriginalBladeEdgePosition = BladeEdge.position;
	}

    void Update() 
    {
        #region CuttingCode
        if (state == CutState.ReadyToCut)
        {
            Vector3 origin = BladeEdge.position + new Vector3(0.0f, 0.5f, 0.0f);
            Ray ray = new Ray(origin, Vector3.down);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit) && (hit.collider.tag == "Piece" || hit.collider.tag == "Leftover"))
            {
                state = CutState.Cutting;
                CurrentLine = GetNearestLine(hit.point);
                BoardBeingCut = hit.transform.gameObject.GetComponent<WoodMaterialObject>();
                BladeEdge.position = hit.point;
            }
        }
        else if (state == CutState.Cutting)
        {
            if (CuttingFirstCheckpoint())
            {
                Vector3 edge = new Vector3(BladeEdge.position.x, 0.0f, 0.0f);
                Vector3 checkpoint = new Vector3(CurrentLine.GetCurrentCheckpoint().GetPosition().x, 0.0f, 0.0f);

                float distance = Vector3.Distance(edge, checkpoint);
                if (distance <= ValidCutOffset)
                {
                    if (PassedCurrentCheckpoint())
                    {
                        CurrentLine.UpdateToNextCheckpoint();
                    }
                }
                else
                {
                    //Decease value until player is forced to start over
                }
            }
            else if (CuttingConnectedCheckpoint() || CuttingLastCheckpoint())
            {
                float distance = DistanceBetweenFromOrigin(CurrentLine.GetCurrentCheckpoint().GetPosition(), BladeEdge.position, CurrentLine.GetPreviousCheckpoint().GetPosition());
                //Add code that determine score decrease, if any
                if (PassedCurrentCheckpoint())
                {
                    CurrentLine.UpdateToNextCheckpoint();
                    if (CurrentLine.GetCurrentCheckpoint() == null)
                    {
                        state = CutState.EndOfCut;
                        LinesToCut.Remove(CurrentLine);
                        WoodManager.SplitBoard(CurrentLine.GetFirstBaseNode(), CurrentLine.GetSecondBaseNode(), BoardBeingCut, CurrentLine);
                        CurrentLine = null;
                        BoardBeingCut = null;
                        BladeEdge.position = OriginalBladeEdgePosition;
                    }
                }
            }
        }
        else if (state == CutState.EndOfCut)
        {
            Vector3 origin = BladeEdge.position + new Vector3(0.0f, 0.5f, 0.0f);
            Ray ray = new Ray(origin, Vector3.down);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit) && hit.collider.tag != "Piece" && hit.collider.tag != "Leftover")
            {
                state = CutState.ReadyToCut;
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
                smallestDistance = Vector3.Distance(fromPosition, LinesToCut[i].GetCurrentCheckpoint().GetPosition());
            }
            else
            {
                float distance = Vector3.Distance(fromPosition, LinesToCut[i].GetCurrentCheckpoint().GetPosition());
                if (distance < smallestDistance)
                {
                    nearestLineIndex = i;
                    smallestDistance = distance;
                }
            }
        }
        return LinesToCut[nearestLineIndex];
    }

    private bool CuttingFirstCheckpoint()
    {
        return CurrentLine.GetPreviousCheckpoint() == null && CurrentLine.GetNextCheckpoint() != null;
    }

    private bool CuttingConnectedCheckpoint()
    {
        return CurrentLine.GetPreviousCheckpoint() != null && CurrentLine.GetNextCheckpoint() != null;
    }

    private bool CuttingLastCheckpoint()
    {
        return CurrentLine.GetPreviousCheckpoint() != null && CurrentLine.GetNextCheckpoint() == null;
    }

    private float DistanceBetweenFromOrigin(Vector3 currentCheckpoint, Vector3 bladeEdge, Vector3 previousCheckpoint)
    {
        Vector3 edge = bladeEdge - previousCheckpoint;
        Vector3 checkpoint = currentCheckpoint - previousCheckpoint;
        checkpoint.Normalize();

        float projection = Vector3.Dot(edge, checkpoint);
        checkpoint = checkpoint * projection;
        Vector3 rejectionVector = edge - checkpoint;

        return rejectionVector.magnitude;
    }

    private bool PassedCurrentCheckpoint()
    {
        bool passed = false;
        Vector3 difference = CurrentLine.GetCurrentCheckpoint().GetPosition() - BladeEdge.position;
        if (difference.z >= 0)
        {
            passed = true;
        }
        return passed;
    }
}

/*
 * Test Code
 * 
 * if (Input.GetMouseButtonDown(0) && LinesToCut.Count > 0)
        {
            RaycastHit hit;
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(mouseRay, out hit) && (hit.collider.tag == "Piece" || hit.collider.tag == "Leftover"))
            {
                Node baseNode = CurrentLine.Connections[0].FirstPiece;
                Node baseNode2 = CurrentLine.Connections[0].SecondPiece;
                WoodMaterialObject board = hit.transform.GetComponent<WoodMaterialObject>();

                LinesToCut.Remove(CurrentLine);
                WoodManager.SplitBoard(baseNode, baseNode2, board, CurrentLine);
                Destroy(board.gameObject);
                CurrentLine = (LinesToCut.Count > 0) ? LinesToCut[0] : null;
            }
        }
*/
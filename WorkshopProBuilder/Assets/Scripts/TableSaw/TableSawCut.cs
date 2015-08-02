using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TableSawCut : MonoBehaviour 
{
    public WoodMaterialManager WoodManager;
    public List<CutLine> LinesToCut;
    public Transform BladeEdge;
    public TableSawBlade Blade;
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

    void Update() 
    {
        #region CuttingCode
        if (state == CutState.ReadyToCut)
        {
            if (Blade.MadeContactWithBoard)
            {
                Vector3 origin = BladeEdge.position + new Vector3(0.0f, 0.5f, 0.0f);
                Ray ray = new Ray(origin, Vector3.down);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit) && (hit.collider.tag == "Piece" || hit.collider.tag == "Leftover"))
                {
                    state = CutState.Cutting;
                    currentLine = GetNearestLine(hit.point);
                    boardBeingCut = hit.transform.gameObject.GetComponent<WoodMaterialObject>();
                    boardBeingCut.GetComponent<BoardController>().RotationPoint = BladeEdge;
                    BladeEdge.position = hit.point;
                    cuttingAlongLine = BladeWithinValidCutOffset();
                }
            }
        }
        else if (state == CutState.Cutting)
        {
            if (CuttingFirstCheckpoint() && Blade.CuttingWoodBoard)
            {
                if (cuttingAlongLine)
                {
                    if (PassedCurrentCheckpoint())
                    {
                        currentLine.UpdateToNextCheckpoint();
                        float distance = DistanceBetweenBladeAndLine(currentLine.GetCurrentCheckpoint().GetPosition(), BladeEdge.position, currentLine.GetPreviousCheckpoint().GetPosition());
                        //Use distance to determine score
                        previousCheckpointPosition = currentLine.GetCurrentCheckpoint().GetPosition();
                    }
                }
                else
                {
                    //Decrease value until player is forced to start over
                }
            }
            else if (( CuttingConnectedCheckpoint() || CuttingLastCheckpoint() ) && Blade.CuttingWoodBoard)
            {
                Vector3 currentPointPosition = currentLine.GetCurrentCheckpoint().GetPosition();
                float distance = Vector3.Distance(previousCheckpointPosition, currentPointPosition);
                //Distance determines push rate, which can lower score if too fast ro too slow
                if (distance == 0)
                {
                    timeWithoutPushing += Time.deltaTime;
                    if (timeWithoutPushing >= 1.0f)
                    {
                        //Burnt wood; Start over;
                    }
                }
                else
                {
                    timeWithoutPushing = 0.0f;
                }

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
            if (Blade.CuttingWoodBoard && !Blade.NoInteractionWithBoard)
            {
                boardBeingCut.transform.position += new Vector3(0.0f, 0.0f, 0.8f * Time.deltaTime);
            }
            else
            {
                cuttingAlongLine = false;
                boardBeingCut.GetComponent<BoardController>().RotationPoint = null;
                boardBeingCut.transform.position += new Vector3(0.0f, 0.0f, 0.8f * Time.deltaTime);
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
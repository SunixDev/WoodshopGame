using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TableSawCut : MonoBehaviour 
{
    public WoodMaterialManager WoodManager;
    public List<CutLine> LinesToCut;
    public Transform BladeEdge;
    public CutState state { get; set; }

    private CutLine CurrentLine;

	void Start () 
    {
        state = CutState.ReadyToCut;
        CurrentLine = LinesToCut[0];
	}

    void Update() 
    {
        if (Input.GetMouseButtonDown(0) && LinesToCut.Count > 0)
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

        #region CuttingCode
        if (state == CutState.ReadyToCut)
        {
            //CutLine line = GetNearestLine();
            //Vector3 bladeEdge = new Vector3(BladeEdge.position.x, 0.0f, BladeEdge.position.z);
            //Vector3 checkpoint = new Vector3(line.GetCurrentCheckpoint().GetPosition().x, 0.0f, line.GetCurrentCheckpoint().GetPosition().z);
            //Vector3 difference = checkpoint - bladeEdge;
            //if (difference.z > 0.0f)
            //{
            //    state = CutState.Cutting;
            //    CurrentLine = line;
            //    CurrentLine.UpdateToNextCheckpoint();
            //}
        }
        else if (state == CutState.Cutting)
        {
            //Vector3 bladeEdge = new Vector3(BladeEdge.position.x, 0.0f, BladeEdge.position.z);
            //Vector3 checkpoint = new Vector3(CurrentLine.GetCurrentCheckpoint().GetPosition().x, 0.0f, CurrentLine.GetCurrentCheckpoint().GetPosition().z);
            //Vector3 difference = checkpoint - bladeEdge;
            //if (difference.z > 0.0f)
            //{
            //    CurrentLine.UpdateToNextCheckpoint();
            //    if (CurrentLine.GetCurrentCheckpoint() == null)
            //    {
            //        SeparatePieces();
            //        LinesToCut.RemoveAt(NearestLineIndex);
            //        Destroy(CurrentLine);
            //        NearestLineIndex = -1;
            //        state = CutState.ReadyToCut;
            //    }
            //}
        }
        else
        {
            //Debug.LogError("Issue with line cutting");
        }
        #endregion

    }

    private CutLine GetNearestLine()
    {
        return null;
    }
}
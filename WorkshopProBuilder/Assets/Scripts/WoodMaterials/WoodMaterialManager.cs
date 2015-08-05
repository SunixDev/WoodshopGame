using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WoodMaterialManager : MonoBehaviour 
{
    public List<GameObject> WoodMaterials;

    public void SplitBoard(Node baseNode, Node baseNode2, WoodMaterialObject boardToSplit, CutLine detachedLine)
    {
        WoodMaterials.Remove(boardToSplit.gameObject);
        WoodManagerHelper.RemoveLine(boardToSplit, detachedLine);
        DeterminePiece(baseNode, ref boardToSplit.LinesToCut);
        DeterminePiece(baseNode2, ref boardToSplit.LinesToCut);
        Destroy(boardToSplit.gameObject);
    }

    private void DeterminePiece(Node node, ref List<CutLine> AvailableLines)
    {
        if (node.ConnectedPieces.Count <= 0)
        {
            GameObject obj = node.gameObject;
            obj.transform.parent = null;
            if (obj.tag == "Piece")
            {
                PieceController controller = obj.AddComponent<PieceController>();
                controller.Initialize();

                PositionSnap snapping = obj.AddComponent<PositionSnap>();
                snapping.Initialize(controller);

                WoodMaterials.Add(obj);
            }
            else if (obj.tag == "Leftover")
            {
                Destroy(obj);
            }
            else
            {
                Debug.LogError(obj.name + " is not tag as Piece or Leftover");
            }
        }
        else
        {
            GameObject obj = WoodManagerHelper.CreateSeparateBoard(node, ref AvailableLines);
            WoodMaterials.Add(obj);
        }
    }
}

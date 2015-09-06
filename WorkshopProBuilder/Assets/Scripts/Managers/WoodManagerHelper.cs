using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WoodManagerHelper : MonoBehaviour 
{
    public static void RemoveLine(WoodMaterialObject boardToSplit, CutLine detachedLine)
    {
        detachedLine.SeverConnections();
        boardToSplit.RemoveLine(detachedLine);
        Destroy(detachedLine.gameObject);
    }

    public static GameObject CreateSeparateBoard(Node baseNode, ref WoodMaterialObject boardToSplit)
    {
        List<Node> nodes = new List<Node>();
        RetrieveNodes(ref nodes, baseNode);

        GameObject board = new GameObject("WoodStrip");
        board.tag = "WoodStrip";
        BoardController controller = board.AddComponent<BoardController>();
        controller.objRigidbody = board.GetComponent<Rigidbody>();
        WoodMaterialObject wood = board.AddComponent<WoodMaterialObject>();
        foreach (Node n in nodes)
        {
            n.gameObject.transform.parent = board.transform;
            wood.WoodPieces.Add(n.gameObject);
            if (n.gameObject.tag == "DadoBlock")
            {
                wood.AddDado(n.gameObject.GetComponent<DadoBlock>());
            }
            for (int i = 0; i < boardToSplit.LinesToCut.Count; i++)
            {
                if (boardToSplit.LinesToCut[i].ContainsPiece(n))
                {
                    boardToSplit.LinesToCut[i].gameObject.transform.parent = board.transform;
                    wood.AddLine(boardToSplit.LinesToCut[i]);
                    boardToSplit.RemoveLine(i--);
                }
            }
        }
        controller.WoodObject = wood;
        return board;
    }

    //Recursive call to get all connected nodes
    private static void RetrieveNodes(ref List<Node> nodes, Node baseNode)
    {
        nodes.Add(baseNode);
        foreach (Node c in baseNode.ConnectedPieces)
        {
            if (!nodes.Contains(c))
            {
                RetrieveNodes(ref nodes, c);
            }
        }
    }
}

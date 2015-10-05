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


    public static GameObject CreateSeparateBoard(Node baseNode, ref WoodMaterialObject boardToSplit)
    {
        List<Node> nodes = new List<Node>();
        RetrieveNodes(ref nodes, baseNode);

        GameObject board = new GameObject("WoodStrip");
        board.tag = "WoodStrip";
        WoodMaterialObject woodBoard = board.AddComponent<WoodMaterialObject>();
        List<Node> boardNodes = new List<Node>();
        List<CutLine> boardLines = new List<CutLine>();
        Bounds boardBounds = new Bounds();
        for (int index = 0; index < nodes.Count; index++)
        {
            Node n = nodes[index];
            boardNodes.Add(n);//n.gameObject.transform.parent = board.transform;
            woodBoard.WoodPieces.Add(n.gameObject);
            if (index == 0)
            {
                boardBounds = new Bounds(n.gameObject.transform.position, Vector3.zero);
            }
            else
            {
                boardBounds.Encapsulate(n.gameObject.transform.position);
            }
            if (n.gameObject.tag == "DadoBlock")
            {
                woodBoard.AddDado(n.gameObject.GetComponent<DadoBlock>());
            }
            for (int i = 0; i < boardToSplit.LinesToCut.Count; i++)
            {
                if (boardToSplit.LinesToCut[i].ContainsPiece(n))
                {
                    boardLines.Add(boardToSplit.LinesToCut[i]);//boardToSplit.LinesToCut[i].gameObject.transform.parent = board.transform;
                    boardBounds.Encapsulate(boardToSplit.LinesToCut[i].gameObject.transform.position);
                    woodBoard.AddLine(boardToSplit.LinesToCut[i]);
                    boardToSplit.RemoveLine(i--);
                }
            }
        }

        board.transform.position = boardBounds.center;

        foreach (Node n in boardNodes)
        {
            n.gameObject.transform.parent = board.transform;
        }

        foreach (CutLine line in boardLines)
        {
            line.gameObject.transform.parent = board.transform;
        }

        return board;
    }
}

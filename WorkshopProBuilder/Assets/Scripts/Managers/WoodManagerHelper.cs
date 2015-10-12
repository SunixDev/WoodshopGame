using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WoodManagerHelper : MonoBehaviour 
{
    public static void RemoveCutLine(WoodMaterialObject boardToSplit, CutLine detachedLine)
    {
        detachedLine.SeverConnections();
        boardToSplit.RemoveLine(detachedLine);
        Destroy(detachedLine.gameObject);
    }

    private static GameObject DeterminePiece(Node node, ref WoodMaterialObject boardToSplit)
    {
        GameObject objToReturn = null;
        if (node.ConnectedPieces.Count <= 0)
        {
            GameObject obj = node.gameObject;
            obj.transform.parent = null;
            if (obj.tag == "Piece")
            {
                PieceController controller = obj.GetComponent<PieceController>();
                if (controller == null)
                {
                    controller = obj.AddComponent<PieceController>();
                }
                controller.Initialize();
            }
            else if (obj.tag == "Leftover")
            {
                WoodLeftover leftoverScript = obj.GetComponent<WoodLeftover>();
                leftoverScript.BeginDisappearing();
            }
            else
            {
                Debug.LogError(obj.name + " is not tag as Piece or Leftover");
            }
            objToReturn = obj;
        }
        else
        {
            GameObject obj = WoodManagerHelper.CreateSeparateBoard(node, ref boardToSplit);
            objToReturn = obj;
        }
        return objToReturn;
    }

    public static List<GameObject> SplitBoard(Node baseNode, Node baseNode2, WoodMaterialObject boardToSplit, CutLine detachedLine)
    {
        WoodManagerHelper.RemoveCutLine(boardToSplit, detachedLine);

        List<GameObject> splitPieces = new List<GameObject>();
        splitPieces.Add(WoodManagerHelper.DeterminePiece(baseNode, ref boardToSplit));
        splitPieces.Add(WoodManagerHelper.DeterminePiece(baseNode2, ref boardToSplit));
        Destroy(boardToSplit.gameObject);

        return splitPieces;
    }

    //Recursive call to get all connected nodes
    private static void RetrieveNodes(ref List<Node> nodes, Node baseNode)
    {
        nodes.Add(baseNode);
        foreach (Node c in baseNode.ConnectedPieces)
        {
            if (!nodes.Contains(c))
            {
                WoodManagerHelper.RetrieveNodes(ref nodes, c);
            }
        }
    }


    public static GameObject CreateSeparateBoard(Node baseNode, ref WoodMaterialObject boardToSplit)
    {
        List<Node> nodes = new List<Node>();
        WoodManagerHelper.RetrieveNodes(ref nodes, baseNode);

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

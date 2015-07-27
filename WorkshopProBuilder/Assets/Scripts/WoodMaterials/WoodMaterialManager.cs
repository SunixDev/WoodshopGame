using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WoodMaterialManager : MonoBehaviour 
{
    public List<GameObject> WoodMaterials;

    public void SplitBoard(Node baseNode, Node baseNode2, WoodMaterialObject boardToSplit, CutLine detachedLine)
    {
        WoodMaterials.Remove(boardToSplit.gameObject);
        RemoveLine(boardToSplit, detachedLine);
        CreateSeparateBoard(baseNode, ref boardToSplit.LinesToCut);
        CreateSeparateBoard(baseNode2, ref boardToSplit.LinesToCut);
        Destroy(boardToSplit.gameObject);
    }

    private void CreateSeparateBoard(Node baseNode, ref List<CutLine> AvailableLines)
    {
        List<Node> nodes = new List<Node>();
        RetrieveNodes(ref nodes, baseNode);

        GameObject go = new GameObject();
        go.name = "WoodMaterial";
        go.AddComponent<Rigidbody>();
        WoodMaterialObject wood = go.AddComponent<WoodMaterialObject>();

        foreach (Node n in nodes)
        {
            n.gameObject.transform.parent = go.transform;
            wood.WoodPieces.Add(n.gameObject);
            for (int i = 0; i < AvailableLines.Count; i++)
            {
                if (AvailableLines[i].ContainsPiece(n))
                {
                    AvailableLines[i].gameObject.transform.parent = go.transform;
                    wood.LinesToCut.Add(AvailableLines[i]);
                    AvailableLines.RemoveAt(i--);
                }
            }
        }

        WoodMaterials.Add(go);
    }

    //Recursive call to get all connected nodes
    private void RetrieveNodes(ref List<Node> nodes, Node baseNode)
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

    private void RemoveLine(WoodMaterialObject boardToSplit, CutLine detachedLine)
    {
        detachedLine.SeverConnections();
        boardToSplit.LinesToCut.Remove(detachedLine);
        Destroy(detachedLine.gameObject);
    }
}

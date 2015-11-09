using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class WoodMaterialManager : MonoBehaviour 
{
    public List<GameObject> WoodMaterials;

    public List<GameObject> SplitBoard(Node baseNode, Node baseNode2, WoodMaterialObject boardToSplit, CutLine detachedLine)
    {
        WoodMaterials.Remove(boardToSplit.gameObject);
        WoodManagerHelper.RemoveCutLine(boardToSplit, detachedLine);

        List<GameObject> splitPieces = new List<GameObject>();
        splitPieces.Add(DeterminePiece(baseNode, ref boardToSplit));
        splitPieces.Add(DeterminePiece(baseNode2, ref boardToSplit));
        Destroy(boardToSplit.gameObject);

        return splitPieces;
    }

    public List<GameObject> GetPiecesByLine(CutLineType line)
    {
        List<GameObject> materials = new List<GameObject>();
        foreach (GameObject go in WoodMaterials)
        {
            WoodMaterialObject wood = go.GetComponent<WoodMaterialObject>();
            if (wood != null)
            {
                if (wood.ContainsLinesOfType(line))
                {
                    materials.Add(go);
                }
            }
        }
        return materials;
    }

    public List<GameObject> GetRevealedPieces()
    {
        List<GameObject> materials = new List<GameObject>();
        foreach (GameObject go in WoodMaterials)
        {
            if (go.tag == "Piece")
            {
                materials.Add(go);
            }
        }
        return materials;
    }

    public List<GameObject> GetAllWoodMaterials()
    {
        return WoodMaterials;
    }

    public GameObject CombinePieces(WoodPiece pieceOne, WoodPiece pieceTwo)
    {
        return null;
    }

    private GameObject DeterminePiece(Node node, ref WoodMaterialObject boardToSplit)
    {
        GameObject objToReturn = null;
        if (node.ConnectedPieces.Count <= 0)
        {
            GameObject obj = node.gameObject;
            obj.transform.parent = null;
            if (obj.tag == "Piece")
            {
                PieceController controller = obj.GetComponent<PieceController>();
                //controller.Initialize();
                WoodMaterials.Add(obj);
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
            WoodMaterials.Add(obj);
            objToReturn = obj;
        }
        return objToReturn;
    }

    public void HideAllPieces()
    {
        foreach (GameObject go in WoodMaterials)
        {
            go.transform.position = Vector3.zero;
            go.transform.rotation = Quaternion.identity;
            go.SetActive(false);
        }
    }

    public void HidePiece(GameObject woodMaterial)
    {
        woodMaterial.transform.position = Vector3.zero;
        woodMaterial.transform.rotation = Quaternion.identity;
        woodMaterial.SetActive(false);
    }
}
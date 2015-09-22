using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class WoodMaterialManager : MonoBehaviour 
{
    public List<GameObject> WoodMaterials;

    public void SplitBoard(Node baseNode, Node baseNode2, WoodMaterialObject boardToSplit, CutLine detachedLine)
    {
        WoodMaterials.Remove(boardToSplit.gameObject);
        WoodManagerHelper.RemoveLine(boardToSplit, detachedLine);
        DeterminePiece(baseNode, ref boardToSplit);
        DeterminePiece(baseNode2, ref boardToSplit);
        Destroy(boardToSplit.gameObject);
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

    private void DeterminePiece(Node node, ref WoodMaterialObject boardToSplit)
    {
        if (node.ConnectedPieces.Count <= 0)
        {
            GameObject obj = node.gameObject;
            obj.transform.parent = null;
            if (obj.tag == "Piece")
            {
                PieceController controller = obj.GetComponent<PieceController>();
                controller.Initialize();
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
        }
        else
        {
            GameObject obj = WoodManagerHelper.CreateSeparateBoard(node, ref boardToSplit);
            WoodMaterials.Add(obj);
        }
    }
}


//public List<GameObject> RetrieveAvailablePieces()
//{
//    List<GameObject> pieces = new List<GameObject>();
//    foreach (GameObject piece in WoodMaterials)
//    {
//        if (piece.tag == "Piece")
//        {
//            pieces.Add(piece);
//        }
//    }
//    return pieces;
//}


//private static WoodMaterialManager _instance;
//public static WoodMaterialManager instance
//{
//    get
//    {
//        return _instance;
//    }
//}

//void Awake()
//{
//    if (_instance == null)
//    {
//        _instance = this;
//        DontDestroyOnLoad(gameObject);
//    }
//    else
//    {
//        if (this != _instance)
//        {
//            Destroy(this.gameObject);
//        }
//    }
//}




//List<AnchorPoint> allPoints = pieceOne.AnchorPoints;
//allPoints.AddRange(pieceTwo.AnchorPoints);
//pieceOne.gameObject.GetComponent<PieceController>();

//GameObject obj = new GameObject();
//Bounds bounds = new Bounds(pieceOne.gameObject.transform.position, Vector3.zero);
//bounds.Encapsulate(pieceTwo.gameObject.transform.position);
//obj.transform.position = bounds.center;

//PieceController controller = obj.AddComponent<PieceController>();
//controller.Initialize();

//PositionSnap snapping = obj.AddComponent<PositionSnap>();
//snapping.Initialize(controller);
//controller.enabled = true;
//snapping.enabled = true;

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BandSawManager : MonoBehaviour 
{
    public List<GameObject> WoodToCut;
    public Transform PlacementFromBlade;
    public BandSawUI UI_Manager;

    private int currentPieceIndex = 0;

	void Start () 
    {
        UI_Manager.ChangeSawButtons(false);
        foreach (GameObject woodMat in WoodToCut)
        {
            WoodMaterialObject wood = woodMat.GetComponent<WoodMaterialObject>();
            foreach (CutLine line in wood.LinesToCut)
            {
                if (line.CutType == CutLineType.CurvatureCut)
                {
                    line.lineRenderer.enabled = true;
                }
            }
            woodMat.SetActive(false);
        }
        WoodToCut[currentPieceIndex].SetActive(true);
        PlacePiece();
    }

    public void SplitMaterial(CutLine lineToRemove)
    {
        if (WoodToCut.Count > 0)
        {
            WoodMaterialObject woodBoard = WoodToCut[currentPieceIndex].GetComponent<WoodMaterialObject>();
            List<GameObject> pieces = WoodManagerHelper.SplitBoard(lineToRemove.GetFirstBaseNode(),
                                                        lineToRemove.GetSecondBaseNode(),
                                                        woodBoard, lineToRemove);
            foreach (GameObject piece in pieces)
            {
                if (piece.tag == "Piece")
                {
                    WoodToCut.RemoveAt(currentPieceIndex);
                    Destroy(piece);
                    NextPiece();
                }
                else
                {
                    Rigidbody physics = piece.AddComponent<Rigidbody>();
                    physics.useGravity = true;
                    BandSawPieceController controller = piece.AddComponent<BandSawPieceController>();
                    controller.Moveable = true;
                    WoodToCut[currentPieceIndex] = piece;
                    PlacePiece();
                }
            }
        }
        else
        {
            Debug.Log("All lines cut");
        }
    }

    private void NextPiece()
    {
        if (WoodToCut.Count > 0)
        {
            WoodToCut[currentPieceIndex].SetActive(true);
            PlacePiece();
        }
    }

    public void PlacePiece()
    {
        WoodToCut[currentPieceIndex].transform.rotation = Quaternion.identity;
        WoodToCut[currentPieceIndex].transform.position = PlacementFromBlade.position;// +new Vector3(0.0f, 0.0f, -2.0f);
        //Ray ray = new Ray(PlacementFromBlade.position, -Vector3.forward);
        //RaycastHit hit;
        //if (Physics.Raycast(ray, out hit))
        //{
        //    float distance = (hit.point - PlacementFromBlade.position).magnitude;
        //    WoodToCut[currentPieceIndex].transform.position += (distance * Vector3.forward);
        //}
    }

    public Vector3 GetCurrentBoardPosition()
    {
        return WoodToCut[currentPieceIndex].transform.position;
    }

    public bool AllLinesOnBoardAreCut()
    {
        WoodMaterialObject wood = WoodToCut[currentPieceIndex].GetComponent<WoodMaterialObject>();
        int linesCut = 0;
        foreach (CutLine line in wood.LinesToCut)
        {
            if (line.CutType == CutLineType.CurvatureCut)
            {
                linesCut++;
            }
        }
        return linesCut == 0;
    }

    public bool AllPiecesAreCut()
    {
        return currentPieceIndex == WoodToCut.Count;
    }

    public CutLine GetNearestLine(Vector3 fromPosition)
    {
        bool lineFound = false;
        int nearestLineIndex = -1;
        float smallestDistance = 0.0f;
        List<CutLine> lines = WoodToCut[currentPieceIndex].GetComponent<WoodMaterialObject>().LinesToCut;
        for (int i = 0; i < lines.Count && !lineFound; i++)
        {
            CutLine currentLine = lines[i];
            if (currentLine.CutType == CutLineType.CurvatureCut)
            {
                float firstDistance = Vector3.Distance(fromPosition, currentLine.Checkpoints[0].GetPosition());
                float lastDistance = Vector3.Distance(fromPosition, currentLine.Checkpoints[currentLine.Checkpoints.Count - 1].GetPosition());

                

                if (nearestLineIndex == -1 || firstDistance < smallestDistance || lastDistance < smallestDistance)
                {
                    if (nearestLineIndex == -1)
                    {
                        smallestDistance = (firstDistance < lastDistance) ? firstDistance : lastDistance;
                    }
                    else
                    {
                        smallestDistance = (firstDistance < smallestDistance) ? firstDistance : smallestDistance;
                        smallestDistance = (lastDistance < smallestDistance) ? lastDistance : smallestDistance;
                    }
                    nearestLineIndex = i;
                }
            }
        }
        return lines[nearestLineIndex];
    }
}




//AvailableWoodMaterial = GameManager.instance.GetNecessaryMaterials(CutLineType.CurvatureCut);
//LinesToCut = new List<CutLine>();
//foreach (GameObject go in AvailableWoodMaterial)
//{
//    WoodMaterialObject wood = go.GetComponent<WoodMaterialObject>();
//    LinesToCut.AddRange(wood.RetrieveLines(CutLineType.CurvatureCut, GameManager.instance.GetStep()));
//    go.SetActive(false);
//}
//if (AvailableWoodMaterial.Count > 0)
//{
//    AvailableWoodMaterial[currentPieceIndex].SetActive(true);
//    PlacePiece();
//    UI_Manager.UpdateSelectionButtons(currentPieceIndex, AvailableWoodMaterial.Count);
//}
//else
//{
//    Debug.Log("No pieces are available");
//}


//foreach (GameObject woodMat in WoodToCut)
//{
//    WoodMaterialObject wood = woodMat.GetComponent<WoodMaterialObject>();
//    foreach (CutLine line in wood.LinesToCut)
//    {
//        if (line.CutType == CutLineType.CurvatureCut)
//        {
//            line.DisplayLine(true, false);
//        }
//    }
//    BoardController controller = woodMat.AddComponent<BoardController>();
//    controller.Moveable = true;
//    controller.WoodObject = wood;
//    woodMat.SetActive(false);
//}
//WoodToCut[currentPieceIndex].SetActive(true);
//currentBoardController = WoodToCut[currentPieceIndex].GetComponent<BoardController>();
//PlacePiece();
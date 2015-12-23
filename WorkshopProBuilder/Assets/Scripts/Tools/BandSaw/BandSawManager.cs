using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BandSawManager : MonoBehaviour 
{
    public List<GameObject> WoodToCut;
    public List<CutLine> LinesToCut;
    public Transform PlacementFromBlade;
    public BandSawUI UI_Manager;
    public bool StillCutting { get; set; }

    private int currentPieceIndex = 0;
    private float cumulativeLineScore = 0.0f;
    private float numberOfCuts;

    void Start()
    {
        numberOfCuts = LinesToCut.Count;
        StillCutting = true;
        UI_Manager.DisplayPlans(true);
        UI_Manager.ChangeSawButtons(false);
        foreach (GameObject woodMat in WoodToCut)
        {
            woodMat.SetActive(false);
        }
        foreach (CutLine line in LinesToCut)
        {
            if (line.CutType == CutLineType.CurvatureCut)
            {
                line.lineRenderer.enabled = true;
            }
        }
        WoodToCut[currentPieceIndex].SetActive(true);
        PlacePiece();
    }

    public void StopGameDueToLowScore(string message)
    {
        StillCutting = false;
        UI_Manager.InfoPanel.SetActive(true);
        UI_Manager.InfoText.text = message + "\nStart the project all over again with new materials.";
        UI_Manager.HideButton.gameObject.SetActive(false);
        UI_Manager.StartOverButton.gameObject.SetActive(true);
        UI_Manager.NextSceneButton.gameObject.SetActive(false);
    }

    public void DisplayScore(float lineScore)
    {
        UI_Manager.InfoPanel.SetActive(true);
        cumulativeLineScore += lineScore;
        Debug.Log("Band Saw Cut Score: " + lineScore);
        string result = "";
        if (lineScore >= 90.0f)
        {
            result += "Excellent! That was a perfect cut.";
        }
        else if (lineScore < 90.0f && lineScore >= 75.0f)
        {
            result += "Well done! It's a bit rough, but a clean cut regardless.";
        }
        else
        {
            result += "Not bad, but you can do a much better job. Remember to cut at a consistent rate and near the line.";
        }
        UI_Manager.InfoText.text = result;
        UI_Manager.HideButton.gameObject.SetActive(true);
        UI_Manager.StartOverButton.gameObject.SetActive(false);
        UI_Manager.NextSceneButton.gameObject.SetActive(false);
    }

    public void SplitMaterial(CutLine lineToRemove)
    {
        WoodToCut[currentPieceIndex].transform.rotation = Quaternion.identity;
        WoodMaterialObject woodBoard = WoodToCut[currentPieceIndex].GetComponent<WoodMaterialObject>();
        LinesToCut.Remove(lineToRemove);
        List<GameObject> pieces = WoodManagerHelper.SplitBoard(lineToRemove.GetFirstBaseNode(),
                                                    lineToRemove.GetSecondBaseNode(),
                                                    woodBoard, lineToRemove);
        if (LinesToCut.Count > 0)
        {
            foreach (GameObject piece in pieces)
            {
                if (piece.tag == "Piece")
                {
                    WoodToCut.RemoveAt(currentPieceIndex);
                    Destroy(piece);
                    NextPiece();
                }
                else if (piece.tag == "Leftover")
                {

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
            UI_Manager.InfoPanel.SetActive(true);
            UI_Manager.InfoText.text = "All of the lines are cut. \nOn to the next step.";
            UI_Manager.HideButton.gameObject.SetActive(false);
            UI_Manager.StartOverButton.gameObject.SetActive(false);
            UI_Manager.NextSceneButton.gameObject.SetActive(true);
            StillCutting = false;
            float percentage = cumulativeLineScore / numberOfCuts;
            Debug.Log(percentage);
            if (GameManager.instance != null)
            {
                GameManager.instance.ApplyScore(percentage);
            }
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

    public void SetUpBoardForCutting(bool beingCut)
    {
        WoodToCut[currentPieceIndex].GetComponent<BandSawPieceController>().BeingCut = beingCut;
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

    public void SwitchScene(string level)
    {
        Application.LoadLevel(level);
    }

    public CutLine GetNearestLine(Vector3 fromPosition)
    {
        bool lineFound = false;
        int nearestLineIndex = -1;
        float smallestDistance = 0.0f;
        if (WoodToCut == null)
        {
            Debug.Log(WoodToCut);
        }
        if (WoodToCut[currentPieceIndex] == null)
        {
            Debug.Log(WoodToCut[currentPieceIndex]);
        }
        if (WoodToCut[currentPieceIndex].GetComponent<WoodMaterialObject>() == null)
        {
            Debug.Log(WoodToCut[currentPieceIndex].GetComponent<WoodMaterialObject>());
        }
        if (WoodToCut[currentPieceIndex].GetComponent<WoodMaterialObject>() == null)
        {
            Debug.Log(WoodToCut[currentPieceIndex].GetComponent<WoodMaterialObject>().LinesToCut);
        }
        if (WoodToCut[currentPieceIndex].GetComponent<WoodMaterialObject>().LinesToCut == null)
        {
            Debug.Log(WoodToCut[currentPieceIndex].GetComponent<WoodMaterialObject>().LinesToCut);
        }
        List<CutLine> lines = WoodToCut[currentPieceIndex].GetComponent<WoodMaterialObject>().LinesToCut;
        for (int i = 0; i < lines.Count && !lineFound; i++)
        {
            CutLine currentLine = lines[i];
            if (currentLine != null)
            {
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
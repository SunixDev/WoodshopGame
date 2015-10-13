using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum GlueResult
{
    TooLittle,
    TooMuch,
    Perfect
}

public class SnapPieceGameManager : MonoBehaviour
{
    public List<SnapPiece> PiecesToConnect;
    public Transform Center;
    public List<GlueBox> Glues;
    public List<SnapPoint> SnapPoints;
    public SnapPieceUI UI_Manager;
    public CameraControl GameCamera;
    public Transform SpawnPoint;
    public float ValidConnectionDistance = 0.05f;

    private WoodPiece CurrentPiece;
    private int currentPieceIndex = 0;
    private List<SnapPiece> ConnectedPieces = new List<SnapPiece>();
    private bool inProgress = true;
    private int Total_PerfectGlues = 0;
    private int Total_MinimumGlues = 0;
    private int Total_TooMuchGlues = 0;
    private float TotalPercentage = 0.0f;
    private int TotalPiecesConnected = 0;

    void Start()
    {
        UI_Manager.DisplayPlans(true);
        SnapPiece initialPiece = PiecesToConnect[0];
        initialPiece.gameObject.SetActive(true);
        Destroy(initialPiece.gameObject.GetComponent<PieceController>());
        PiecesToConnect.RemoveAt(0);
        initialPiece.gameObject.transform.parent = Center;
        initialPiece.RotateToLocalRotation();
        initialPiece.MoveToLocalLocation();

        foreach (SnapPiece piece in PiecesToConnect)
        {
            UI_Manager.ListPanel.AddWoodMaterialButton(piece.gameObject.GetComponent<WoodPiece>().Name, this);
            piece.gameObject.SetActive(false);
        }

        foreach (GlueBox glue in Glues)
        {
            glue.gameObject.GetComponent<BoxCollider>().enabled = false;
        }

        CurrentPiece = PiecesToConnect[currentPieceIndex].GetComponent<WoodPiece>();
        PiecesToConnect[currentPieceIndex].gameObject.SetActive(true);
        PiecesToConnect[currentPieceIndex].transform.position = SpawnPoint.position;
        PiecesToConnect[currentPieceIndex].transform.rotation = Quaternion.identity;
    }

    void Update()
    {
        //Debug.Log("TotalPiecesConnected: " + TotalPiecesConnected);
        //Debug.Log("PiecesToConnect: " + PiecesToConnect.Count);
        //Debug.Log("TotalPiecesConnected < PiecesToConnect.Count: " + (TotalPiecesConnected < PiecesToConnect.Count));
        if (TotalPiecesConnected < PiecesToConnect.Count)
        {
            bool pieceConnected = false;
            foreach (SnapPoint otherPoint in SnapPoints)
            {
                foreach (SnapPoint currentPoint in CurrentPiece.SnapPoints)
                {
                    if (currentPoint != otherPoint && currentPoint.CanConnectTo(otherPoint) && 
                        currentPoint.DistanceFromPoint(otherPoint) <= ValidConnectionDistance && CurrentPiece.CanConnect &&
                        otherPoint.ParentSnapPiece.GetComponent<WoodPiece>().CanConnect)
                    {
                        TotalPiecesConnected++;
                        CurrentPiece.gameObject.transform.parent = Center;
                        EvaluateAllConnectionsInPiece();
                        pieceConnected = true;
                        break;
                    }
                }
                if (pieceConnected) { break; }
            }


            if (pieceConnected)
            {
                CurrentPiece = null;
                ConnectedPieces.Add(PiecesToConnect[currentPieceIndex]);
                Destroy(PiecesToConnect[currentPieceIndex].gameObject.GetComponent<PieceController>());
                currentPieceIndex++;
                if (TotalPiecesConnected < PiecesToConnect.Count)
                {
                    UI_Manager.ListPanel.RemoveButton(currentPieceIndex - 1);
                    if (currentPieceIndex == PiecesToConnect.Count || PiecesToConnect[currentPieceIndex].IsAttached)
                    {
                        bool found = false;
                        for (int i = 0; i < PiecesToConnect.Count && !found; i++)
                        {
                            if (!PiecesToConnect[i].IsAttached)
                            {
                                found = true;
                                currentPieceIndex = i;
                            }
                        }
                    }
                    CurrentPiece = PiecesToConnect[currentPieceIndex].GetComponent<WoodPiece>();
                    PiecesToConnect[currentPieceIndex].gameObject.SetActive(true);
                    PiecesToConnect[currentPieceIndex].transform.position = SpawnPoint.position;
                    PiecesToConnect[currentPieceIndex].transform.rotation = Quaternion.identity;
                }
                else
                {
                    DisplayResults();
                }
            }
        }
    }

    private void EvaluateAllConnectionsInPiece()
    {
        foreach (SnapPoint currentPoint in CurrentPiece.SnapPoints)
        {
            if (!currentPoint.IsConnected)
            {
                foreach (SnapPoint otherPoint in SnapPoints)
                {
                    if (currentPoint != otherPoint && currentPoint.CanConnectTo(otherPoint) && 
                        currentPoint.DistanceFromPoint(otherPoint) <= ValidConnectionDistance && CurrentPiece.CanConnect &&
                        otherPoint.ParentSnapPiece.GetComponent<WoodPiece>().CanConnect)
                    {
                        currentPoint.ConnectToPoint(otherPoint, Center);
                        break;
                    }
                }
            }
        }
    }

    public void DisplayResults()
    {
        if (TotalPiecesConnected >= PiecesToConnect.Count)
        {
            foreach (GlueBox glue in Glues)
            {
                EvaluateGluing(glue);
            }
            UI_Manager.InfoPanel.SetActive(true);
            UI_Manager.SceneButton.gameObject.SetActive(true);
            float overall = (TotalPercentage / Glues.Count);
            GameManager.instance.ApplyScore(overall);
            if (Total_MinimumGlues == 0 && Total_TooMuchGlues == 0)
            {
                UI_Manager.InfoText.text = "Results:\nExcellent gluing skills. Every piece was put together with the perfect amount of glue.\nOn to the next step.";
            }
            else
            {
                string praiseText = "";
                if(Total_PerfectGlues > (Total_MinimumGlues + Total_TooMuchGlues))
                {
                    praiseText = "Nice job putting the pieces together. A little bit of clean up and this will look nice.";
                }
                else if (Total_PerfectGlues <= (Total_MinimumGlues + Total_TooMuchGlues))
                {
                    praiseText = "Not bad, but you need more practice in how much glue you add.";
                }
                UI_Manager.InfoText.text = praiseText + "Here are the results:\n" + 
                                           "Areas with the perfect amount of glue: " + Total_PerfectGlues +
                                           "\nAreas that needed a bit more glue: " + Total_MinimumGlues +
                                           "\nAreas with too much glue that bubbled out the edges: " + Total_TooMuchGlues +
                                           "\nOn to the next step";
            }
        }
    }

    private void EvaluateGluing(GlueBox glue)
    {
        if (glue != null)
        {
            float glueApplied = glue.GetTotalGlueApplied();
            TotalPercentage += glueApplied;
            if (glueApplied >= glue.MinValueToActivatePointAndLowScore && glueApplied < glue.MinValueForPerfectScore)
            {
                Total_MinimumGlues++;
            }
            else if (glueApplied >= glue.MinValueForPerfectScore && glueApplied < glue.MaxGlueAmountBeforeTooMuch)
            {
                Total_PerfectGlues++;
            }
            else if (glueApplied >= glue.MaxGlueAmountBeforeTooMuch)
            {
                Total_TooMuchGlues++;
            }
        }
    }

    public void SwitchPiece(int index)
    {
        if (index >= 0 && index < PiecesToConnect.Count && index != currentPieceIndex)
        {
            PiecesToConnect[currentPieceIndex].gameObject.transform.position = Vector3.zero;
            PiecesToConnect[currentPieceIndex].gameObject.SetActive(false);
            PiecesToConnect[index].gameObject.SetActive(true);
            PiecesToConnect[index].transform.position = SpawnPoint.position;
            PiecesToConnect[index].transform.rotation = Quaternion.identity;
            CurrentPiece = PiecesToConnect[index].GetComponent<WoodPiece>();
            currentPieceIndex = index;
        }
        else
        {
            Debug.Log("Index #" + index + " is invalid.");
        }
    }

    public void SetupForMovingPieces()
    {
        if (CurrentPiece != null)
        {
            CurrentPiece.GetComponent<PieceController>().Moveable = true;
        }
        GameCamera.EnableMovement(false);
        foreach (GlueBox glue in Glues)
        {
            glue.gameObject.GetComponent<BoxCollider>().enabled = false;
        }
    }

    public void SetupForGluing()
    {
        if (CurrentPiece != null)
        {
            CurrentPiece.GetComponent<PieceController>().Moveable = false;
        }
        GameCamera.EnableMovement(false);
        foreach (GlueBox glue in Glues)
        {
            glue.gameObject.GetComponent<BoxCollider>().enabled = true;
        }
    }

    public void SetupForCameraMovement()
    {
        if (CurrentPiece != null)
        {
            CurrentPiece.GetComponent<PieceController>().Moveable = false;
        }
        GameCamera.EnableMovement(true);
        foreach (GlueBox glue in Glues)
        {
            glue.gameObject.GetComponent<BoxCollider>().enabled = false;
        }
    }

    public void ResetCameraPosition()
    {
        GameCamera.SnapToRotation(0.0f, 30.0f);
        GameCamera.ResetLookAtOffset();
        GameCamera.DistanceFromPoint = 1.2f;
    }

    public void GoToNextScene(string scene)
    {
        Application.LoadLevel(scene);
    }
}

//PiecesToConnect = GameManager.instance.GetNecessaryPieces();
//foreach (GameObject go in PiecesToConnect)
//{
//    WoodPiece wood = go.GetComponent<WoodPiece>();
//    Glues = wood.ActivateGlueBoxes(GameManager.instance.GetStep());
//    SnapPoints = wood.ActivateSnapPoints(GameManager.instance.GetStep());
//}


//bool piecesConnected = true;
//for (int i = 0; i < SnapPoints.Count && piecesConnected; i++)
//{
//    piecesConnected = SnapPoints[i].IsConnected;
//}

//if (!piecesConnected)
//{

//}
//else
//{
//    Debug.Log("Step Done");
//}

//private void DisplayGlueResult(GlueResult result)
//{
//    UI_Manager.InfoPanel.SetActive(true);
//    UI_Manager.SceneButton.gameObject.SetActive(false);
//    UI_Manager.ContinueButton.gameObject.SetActive(true);
//    if (result == GlueResult.TooLittle)
//    {
//        UI_Manager.InfoText.text = "Not enough glue was added.\nRemember to add glue until you are close to the edge";
//    }
//    else if (result == GlueResult.Perfect)
//    {
//        UI_Manager.InfoText.text = "Excellent, you applied the right amount of glue to keep the pieces together.";
//    }
//    else if (result == GlueResult.TooMuch)
//    {
//        UI_Manager.InfoText.text = "Too much glue was added and some of it spilled out.\nRemember to only add enough to almost reach the edges.";
//    }
//}
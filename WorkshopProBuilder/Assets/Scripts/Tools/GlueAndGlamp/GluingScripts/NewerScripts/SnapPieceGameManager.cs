using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public enum GlueResult
{
    TooLittle,
    TooMuch,
    Perfect
}

public class SnapPieceGameManager : MonoBehaviour
{
    public List<GameObject> PiecesToConnect;
    public List<GlueBox> GlueAreas;
    public List<SnapPoint> SnapPoints;
    public Transform ProjectCenter;
    public MainUI UI_Manager;
    public DragButtonContainer snapPieceListPanel;
    public GlueButtonContainer gluePieceListPanel;

    void Start()
    {
        foreach (GameObject piece in PiecesToConnect)
        {
            piece.transform.position = new Vector3(0f, -20f, 20f);
            Sprite icon = piece.GetComponent<WoodPiece>().ButtonIcon;
            snapPieceListPanel.CreateButton(icon, piece.name);
            gluePieceListPanel.CreateButton(icon, piece.name, this);
        }

        foreach (SnapPoint snapPoint in SnapPoints)
        {
            snapPoint.gameObject.SetActive(true);
            snapPoint.ActiveInStep = true;
        }

        foreach (GlueBox glueArea in GlueAreas)
        {
            glueArea.gameObject.SetActive(true);
            glueArea.ActiveInStep = true;
        }

        UI_Manager.Initialize();
        bool projectFound = false;
        for (int i = 0; i < PiecesToConnect.Count && !projectFound; i++)
        {
            if (PiecesToConnect[i].tag == "WoodProject")
            {
                projectFound = true;
                PiecesToConnect[i].transform.position = ProjectCenter.position;
                PiecesToConnect[i].GetComponent<PieceController>().enabled = false;
            }
        }
        if (!projectFound)
        {
            SnapPiece initialPiece = PiecesToConnect[0].GetComponent<SnapPiece>();
            initialPiece.gameObject.SetActive(true);
            initialPiece.gameObject.GetComponent<PieceController>().enabled = false;
            initialPiece.SnapToProject(ProjectCenter);
        }
    }

    public void TestingGlueButton(Button buttonPressed)
    {
        Debug.Log(buttonPressed + " was pressed");
    }

    private void EvaluateAllConnectionsInPiece()
    {
        //foreach (SnapPoint currentPoint in CurrentPiece.SnapPoints)
        //{
        //    if (!currentPoint.IsConnected)
        //    {
        //        foreach (SnapPoint otherPoint in SnapPoints)
        //        {
        //            if (currentPoint != otherPoint && currentPoint.CanConnectTo(otherPoint) && 
        //                currentPoint.DistanceFromPoint(otherPoint) <= ValidConnectionDistance && CurrentPiece.CanConnect &&
        //                otherPoint.ParentSnapPiece.GetComponent<WoodPiece>().CanConnect)
        //            {
        //                currentPoint.ConnectPieceToPoint(otherPoint, Center);
        //                break;
        //            }
        //        }
        //    }
        //}
    }

    //public void DisplayResults()
    //{
    //    if (TotalPiecesConnected >= PiecesToConnect.Count)
    //    {
    //        foreach (GlueBox glue in Glues)
    //        {
    //            EvaluateGluing(glue);
    //        }
    //        UI_Manager.InfoPanel.SetActive(true);
    //        UI_Manager.SceneButton.gameObject.SetActive(true);
    //        float overall = (TotalPercentage / Glues.Count);
    //        if (GameManager.instance != null)
    //        {
    //            GameManager.instance.ApplyScore(overall);
    //        }
    //        else
    //        {
    //            Debug.Log("No Game manager");
    //        }
    //        if (Total_MinimumGlues == 0 && Total_TooMuchGlues == 0)
    //        {
    //            UI_Manager.InfoText.text = "Results:\nExcellent gluing skills. Every piece was put together with the perfect amount of glue.\nOn to the next step.";
    //        }
    //        else
    //        {
    //            string praiseText = "";
    //            if(Total_PerfectGlues > (Total_MinimumGlues + Total_TooMuchGlues))
    //            {
    //                praiseText = "Nice job putting the pieces together. A little bit of clean up and this will look nice.";
    //            }
    //            else if (Total_PerfectGlues <= (Total_MinimumGlues + Total_TooMuchGlues))
    //            {
    //                praiseText = "Not bad, but you need more practice in how much glue you add.";
    //            }
    //            UI_Manager.InfoText.text = praiseText + "Here are the results:\n" + 
    //                                       "Areas with the perfect amount of glue: " + Total_PerfectGlues +
    //                                       "\nAreas that needed a bit more glue: " + Total_MinimumGlues +
    //                                       "\nAreas with too much glue that bubbled out the edges: " + Total_TooMuchGlues +
    //                                       "\nOn to the next step";
    //        }
    //    }
    //}

    //private void EvaluateGluing(GlueBox glue)
    //{
    //    if (glue != null)
    //    {
    //        float glueApplied = glue.GetTotalGlueApplied();
    //        TotalPercentage += glueApplied;
    //        if (glueApplied >= glue.MinValueToActivatePointAndLowScore && glueApplied < glue.MinValueForPerfectScore)
    //        {
    //            Total_MinimumGlues++;
    //        }
    //        else if (glueApplied >= glue.MinValueForPerfectScore && glueApplied < glue.MaxGlueAmountBeforeTooMuch)
    //        {
    //            Total_PerfectGlues++;
    //        }
    //        else if (glueApplied >= glue.MaxGlueAmountBeforeTooMuch)
    //        {
    //            Total_TooMuchGlues++;
    //        }
    //    }
    //}

    //public void SwitchPiece(int index)
    //{
    //    if (index >= 0 && index < PiecesToConnect.Count && index != currentPieceIndex)
    //    {
    //        PiecesToConnect[currentPieceIndex].gameObject.transform.position = Vector3.zero;
    //        PiecesToConnect[currentPieceIndex].gameObject.SetActive(false);
    //        PiecesToConnect[index].gameObject.SetActive(true);
    //        PiecesToConnect[index].transform.position = SpawnPoint.position;
    //        PiecesToConnect[index].transform.rotation = Quaternion.identity;
    //        CurrentPiece = PiecesToConnect[index].GetComponent<WoodPiece>();
    //        currentPieceIndex = index;
    //    }
    //    else
    //    {
    //        Debug.Log("Index #" + index + " is invalid.");
    //    }
    //}

    public void GoToNextScene(string scene)
    {
        Application.LoadLevel(scene);
    }

    #region UI Event Test
    public void TestUI(Gesture gesture)
    {
        Debug.Log("UI Picked: " + gesture.pickedUIElement);
    }

    private void EnableTouchEvents()
    {
        EasyTouch.On_TouchStart += TestUI;
    }

    private void DisableTouchEvents()
    {
        EasyTouch.On_TouchStart -= TestUI;
    }

    //void OnEnable()
    //{
    //    EnableTouchEvents();
    //}
    //void OnDisable()
    //{
    //    DisableTouchEvents();
    //}
    //void OnDestroy()
    //{
    //    DisableTouchEvents();
    //}
    #endregion
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
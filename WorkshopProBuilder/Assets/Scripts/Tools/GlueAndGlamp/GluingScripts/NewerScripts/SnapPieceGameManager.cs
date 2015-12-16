using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Text;

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
    public SnapPieceUI UI_Manager;
    public GlueManager glueManager;
    public SnapManager snapManager;
    public GameObject connectedProject { get; private set; }

    private int numberOfActiveAnchorPoints = 100;
    private Vector3 HiddenPieceLocation = new Vector3(0f, -20f, 20f);
    private float totalGluePercentage;
    private int totalMinimumGlues;
    private int totalPerfectGlues;
    private int totalTooMuchGlues;
    private bool gameInProgress;

    void Start()
    {
        UI_Manager.Initialize();
        SetUpGameplayObjects();
        bool projectFound = SetUpPieces();
        GenerateButtons(projectFound);
        UI_Manager.DisplayGlueButtonPanel(false);
        UI_Manager.DisplaySnapPieceButtonsPanel(true);
        snapManager.ConnectedProject = connectedProject;
        gameInProgress = true;
    }

    void LateUpdate()
    {
        if (numberOfActiveAnchorPoints <= 0 && gameInProgress)
        {
            //Debug.Log("ALL POINTS CONNECTED");
            foreach (GlueBox glue in GlueAreas)
            {
                EvaluateGlueArea(glue);
            }
            //Debug.Log("Total Glue Applied: " + totalGluePercentage);
            //Debug.Log("Percentage of Overall Glue Applied: " + totalGluePercentage / GlueAreas.Count);
            //Debug.Log("Glue Areas with a Minimal Amount: " + totalMinimumGlues);
            //Debug.Log("Glue Areas with a Perfect Amount: " + totalPerfectGlues);
            //Debug.Log("Glue Areas with a Excessive Amount: " + totalTooMuchGlues);
            string resultsText = EvaluateResults();
            UI_Manager.DisplayResultsPanel(resultsText, displayNextSceneButton: true);
            gameInProgress = false;
        }
    }

    #region Set Up Scene Code
    private void SetUpGameplayObjects()
    {
        numberOfActiveAnchorPoints = SnapPoints.Count;
        totalGluePercentage = 0f;
        totalMinimumGlues = 0;
        totalPerfectGlues = 0;
        totalTooMuchGlues = 0;
        foreach (SnapPoint snapPoint in SnapPoints)
        {
            snapPoint.gameObject.SetActive(true);
            snapPoint.Initialize();
            snapPoint.ActiveInStep = true;
            snapPoint.CanConnect = true;
        }

        foreach (GlueBox glueArea in GlueAreas)
        {
            glueArea.gameObject.SetActive(true);
            glueArea.Initialize();
            glueArea.ActiveInStep = true;
        }
    }

    private void CreateWoodProject(GameObject initialPiece)
    {
        connectedProject = new GameObject("Wood Project");
        connectedProject.tag = "WoodProject";
        connectedProject.transform.position = ProjectCenter.position;

        initialPiece.SetActive(true);
        initialPiece.GetComponent<SnapPiece>().SnapToProject(connectedProject.transform);

        SnapPiece snapPiece = connectedProject.AddComponent<SnapPiece>();
        WoodProject woodProjectComp = connectedProject.AddComponent<WoodProject>();
        woodProjectComp.AddPieceToProject(initialPiece);

        GluedPieceController controller = connectedProject.AddComponent<GluedPieceController>();
    }

    private bool SetUpPieces()
    {
        bool projectFound = false;
        for (int i = 0; i < PiecesToConnect.Count; i++)
        {
            if (PiecesToConnect[i].tag == "WoodProject")
            {
                projectFound = true;
                PiecesToConnect[i].SetActive(true);
                PiecesToConnect[i].transform.position = ProjectCenter.position;
                connectedProject = PiecesToConnect[i];
            }
            else
            {
                PiecesToConnect[i].transform.position = HiddenPieceLocation;
                PiecesToConnect[i].SetActive(false);
            }
        }
        if (!projectFound)
        {
            CreateWoodProject(PiecesToConnect[0]);
            PiecesToConnect[0] = connectedProject;
        }
        return projectFound;
    }

    public void GenerateButtons(bool projectFound)
    {
        for (int i = 0; i < PiecesToConnect.Count; i++)
        {
            if (PiecesToConnect[i].tag == "Piece")
            {
                snapManager.AddDragPiece(PiecesToConnect[i]);
                UI_Manager.CreateDragButton(PiecesToConnect[i]);
                if (PiecesToConnect[i].GetComponent<WoodPiece>().RequiresGlue)
                {
                    glueManager.AddGluingPiece(PiecesToConnect[i]);
                    UI_Manager.CreateGluingButton(PiecesToConnect[i], glueManager);
                }
            }
            if (PiecesToConnect[i].tag == "WoodProject")
            {
                if (PiecesToConnect[i].GetComponent<WoodProject>().RequiresGlue)
                {
                    glueManager.AddGluingPiece(PiecesToConnect[i]);
                    UI_Manager.CreateGluingButton(PiecesToConnect[i].GetComponent<WoodProject>(), glueManager);
                }
            }
        }
    }
    #endregion

    public void UpdateConnections()
    {
        numberOfActiveAnchorPoints--;
    }

    public void UpdateGame(GameObject draggedPiece)
    {
        if (draggedPiece == null)
        {
            Debug.LogError("SnapPieceGameManager cannot update game with a null GameObject");
        }
        else
        {
            if (glueManager.ContainsPiece(draggedPiece))
            {
                int index = glueManager.IndexOfPiece(draggedPiece);
                glueManager.UpdateAvailablePieces(index);
            }
        }
    }

    public string EvaluateResults()
    {
        StringBuilder stringB = new StringBuilder();
        string infoPanelText = "";
        if (numberOfActiveAnchorPoints <= 0)
        {
            float percentage = (totalGluePercentage / GlueAreas.Count);
            if (GameManager.instance != null)
            {
                GameManager.instance.ApplyScore(percentage);
                Debug.Log("Gluing Score Applied");
            }
            if (totalMinimumGlues == 0 && totalTooMuchGlues == 0)
            {
                infoPanelText = "Results:\nExcellent gluing skills. Every piece was put together with the perfect amount of glue.\n\nOn to the next step.";
            }
            else
            {
                if (totalPerfectGlues > (totalMinimumGlues + totalTooMuchGlues))
                {
                    infoPanelText = "Results:\nNice job putting the pieces together. Some areas could've been glued better, but a little bit of clean up and this will look nice.";
                }
                else if (totalPerfectGlues <= (totalMinimumGlues + totalTooMuchGlues))
                {
                    infoPanelText = "Results:\nNot bad, but you need to practice how much glue you add.";
                }
                infoPanelText = infoPanelText + "\n\nHere's how well you did:\n" +
                                           "Areas with the perfect amount of glue: " + totalPerfectGlues +
                                           "\nAreas that needed more glue: " + totalMinimumGlues +
                                           "\nAreas with too much glue: " + totalTooMuchGlues +
                                           "\nOn to the next step";
            }
        }
        return infoPanelText;
    }

    private void EvaluateGlueArea(GlueBox glueArea)
    {
        if (glueArea != null)
        {
            float glueApplied = glueArea.GetTotalGlueApplied();
            PlayerGlue playerGluing = glueManager.glue;
            if (glueApplied >= playerGluing.AmountToActivateSnapPoints && glueApplied < playerGluing.MinAmountForPerfectScore)
            {
                totalMinimumGlues++;
                totalGluePercentage += glueApplied;
            }
            else if (glueApplied >= playerGluing.MinAmountForPerfectScore && glueApplied <= playerGluing.MaxAmount)
            {
                totalPerfectGlues++;
                totalGluePercentage += playerGluing.MaxAmount;
            }
            else if (glueApplied >= playerGluing.MaxAmount)
            {
                totalTooMuchGlues++;
                totalGluePercentage += glueArea.CalculatePercentage(playerGluing.MaxAmount);
            }
        }
    }

    public void GoToNextScene(string scene)
    {
        if (GameManager.instance != null)
        {
            Application.LoadLevel(scene);
        }
        else
        {
            Debug.Log("Next Scene: " + scene);
        }
    }
}
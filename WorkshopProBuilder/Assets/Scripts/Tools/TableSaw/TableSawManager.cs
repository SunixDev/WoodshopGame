using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ActionState
{
    OnSaw,
    UsingRuler,
    ChangingCamera,
    None
}

public class TableSawManager : MonoBehaviour, IToolManager
{
    public List<GameObject> AvailableWoodMaterial;
    public List<CutLine> LinesToCut;
    public Transform FromSawSpawnPoint;
    public Transform CameraSawLookAtPoint;
    public Transform FromRulerSpawnPoint;
    public Transform CameraRulerLookAtPoint;
    public GameObject GameCamera;
    public TableSawUI UI_Manager;
    public Blade SawBlade;
    public Ruler GameRuler;
    public TableSawCut CutGameplay;
    public bool StillCutting { get; set; }

    private int currentPieceIndex = 0;
    private Transform currentSpawnPoint;
    private ActionState currentAction = ActionState.None;
    private ActionState previousAction = ActionState.None;
    private BoardController currentBoardController;
    private float cumulativeLineScore = 0.0f;
    private float numberOfCuts;
    private OrbitCamera orbitCamera;
    private PanCamera panCamera;

	void Start ()
    {
        orbitCamera = GameCamera.GetComponent<OrbitCamera>();
        panCamera = GameCamera.GetComponent<PanCamera>();

        numberOfCuts = LinesToCut.Count;
        UI_Manager.DisplayPlans(true);
        StillCutting = true;
        GameRuler.AssignManager(this);
        foreach (GameObject wood in AvailableWoodMaterial)
        {
            wood.SetActive(false);
        }
        AvailableWoodMaterial[currentPieceIndex].SetActive(true);
        currentBoardController = AvailableWoodMaterial[currentPieceIndex].GetComponent<BoardController>();
        UI_Manager.UpdateSelectionButtons(currentPieceIndex, AvailableWoodMaterial.Count);
        SetupForCutting();
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

    public void DisplayScore(FeedRate rateTracker)
    {
        UI_Manager.InfoPanel.SetActive(true);
        float lineScore = rateTracker.GetLineScore();
        cumulativeLineScore += lineScore;
        Debug.Log("Table Saw Cut Score: " + lineScore);
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
        WoodMaterialObject board = AvailableWoodMaterial[currentPieceIndex].GetComponent<WoodMaterialObject>();
        BoardController previousBoardController = AvailableWoodMaterial[currentPieceIndex].GetComponent<BoardController>();
        LinesToCut.Remove(lineToRemove);
        AvailableWoodMaterial.RemoveAt(currentPieceIndex);
        List<GameObject> pieces = WoodManagerHelper.SplitBoard(lineToRemove.GetFirstBaseNode(),
                                                    lineToRemove.GetSecondBaseNode(),
                                                    board, lineToRemove);

        bool pieceAdded = false;
        foreach (GameObject piece in pieces)
        {
            WoodMaterialObject boardPiece = piece.GetComponent<WoodMaterialObject>();
            if (boardPiece != null)
            {
                bool lineFound = false;
                for (int i = 0; i < LinesToCut.Count && !lineFound; i++)
                {
                    lineFound = boardPiece.ContainsLine(LinesToCut[i]);
                }
                if (lineFound)
                {
                    BoardController controller = piece.AddComponent<BoardController>();
                    controller.Moveable = true;
                    controller.WoodObject = boardPiece;
                    controller.MaxLimit_X = previousBoardController.MaxLimit_X;
                    controller.MaxLimit_Z = previousBoardController.MaxLimit_Z;
                    controller.MinLimit_X = previousBoardController.MinLimit_X;
                    controller.MinLimit_Z = previousBoardController.MinLimit_Z;
                    AvailableWoodMaterial.Add(piece);
                    if (!pieceAdded)
                    {
                        pieceAdded = true;
                        int index = AvailableWoodMaterial.IndexOf(piece);
                        currentPieceIndex = index;
                        AvailableWoodMaterial[currentPieceIndex].SetActive(true);
                        currentBoardController = AvailableWoodMaterial[currentPieceIndex].GetComponent<BoardController>();
                    }
                    else
                    {
                        piece.SetActive(false);
                        piece.transform.position = Vector3.zero;
                        piece.transform.rotation = Quaternion.identity;
                    }
                }
                else
                {
                    Destroy(piece);
                }
            }
        }

        if (!pieceAdded && AvailableWoodMaterial.Count > 0)
        {
            currentPieceIndex = 0;
            AvailableWoodMaterial[currentPieceIndex].SetActive(true);
            currentBoardController = AvailableWoodMaterial[currentPieceIndex].GetComponent<BoardController>();
            SetupForCutting();
            PlacePiece();
        }
        SawBlade.TurnOff();
        UI_Manager.ChangeSawButtons(false);

        if (LinesToCut.Count > 0)
        {
            UI_Manager.UpdateSelectionButtons(currentPieceIndex, AvailableWoodMaterial.Count);
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
            if (GameManager.instance != null)
            {
                GameManager.instance.ApplyScore(percentage);
            }
            else
            {
                Debug.Log("No game manager");
            }
        }
    }

    public void RestrictCurrentBoardMovement(bool restrictZ, bool restrictX)
    {
        currentBoardController.RestrictZ = restrictZ;
        currentBoardController.RestrictX = restrictX;
    }

    public void ResetCurrentBoardRotation()
    {
        currentBoardController.ResetRotation();
    }

    public void RotateCurrentBoard(float angle)
    {
        currentBoardController.ApplyRotation(new Vector3(0.0f, 1.0f, 0.0f), angle);
    }

    public void EnableCurrentBoardMovement(bool enableMovement)
    {
        currentBoardController.Moveable = enableMovement;
    }

    public Vector3 GetCurrentBoardPosition()
    {
        return currentBoardController.gameObject.transform.position;
    }

    public void SwitchToNextPiece()
    {
        if (LinesToCut.Count > 0)
        {
            SwitchPiece(currentPieceIndex + 1);
            UI_Manager.UpdateSelectionButtons(currentPieceIndex, AvailableWoodMaterial.Count);
        }
    }

    public void SwitchToPreviousPiece()
    {
        if (LinesToCut.Count > 0)
        {
            SwitchPiece(currentPieceIndex - 1);
            UI_Manager.UpdateSelectionButtons(currentPieceIndex, AvailableWoodMaterial.Count);
        }
    }

    private void SwitchPiece(int indexToSwitchTo)
    {
        AvailableWoodMaterial[currentPieceIndex].transform.position = Vector3.zero;
        AvailableWoodMaterial[currentPieceIndex].transform.rotation = Quaternion.identity;
        AvailableWoodMaterial[currentPieceIndex].SetActive(false);
        currentPieceIndex = indexToSwitchTo;

        AvailableWoodMaterial[currentPieceIndex].SetActive(true);
        currentBoardController = AvailableWoodMaterial[currentPieceIndex].GetComponent<BoardController>();
        if (currentAction == ActionState.OnSaw || previousAction == ActionState.OnSaw)
        {
            EnableCurrentBoardMovement(true);
            RestrictCurrentBoardMovement(false, false);
        }
        else if (currentAction == ActionState.UsingRuler || previousAction == ActionState.UsingRuler)
        {
            EnableCurrentBoardMovement(false);
        }
        PlacePiece();
    }

    public void SwitchAction(ActionState actionState)
    {
        if (currentAction != actionState)
        {
            previousAction = currentAction;
            currentAction = actionState;
        }
    }

    public void PlacePiece()
    {
        AvailableWoodMaterial[currentPieceIndex].transform.position = currentSpawnPoint.position + new Vector3(0.0f, 0.0f, -3.0f);
        Ray ray = new Ray(currentSpawnPoint.position, -Vector3.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            float distance = (hit.point - currentSpawnPoint.position).magnitude;
            AvailableWoodMaterial[currentPieceIndex].transform.position += (distance * Vector3.forward);
        }
    }

    public bool AreAllLinesCut()
    {
        return (LinesToCut.Count <= 0);
    }

    public void SwitchScene(string level)
    {
        Application.LoadLevel(level);
    }

    public void SetupForCutting()
    {
        currentSpawnPoint = FromSawSpawnPoint;
        if (previousAction == ActionState.None || previousAction == ActionState.UsingRuler || currentAction == ActionState.UsingRuler)
        {
            AvailableWoodMaterial[currentPieceIndex].transform.rotation = Quaternion.identity;
            PlacePiece();
            orbitCamera.enabled = true;
            panCamera.enabled = false;
            orbitCamera.ChangeAngle(0f, 50f);
            orbitCamera.ChangeDistanceConstraints(1.5f, 0.8f, 3.0f);
        }
        SawBlade.TurnOff();
        EnableCurrentBoardMovement(true);
        RestrictCurrentBoardMovement(false, false);
        SwitchAction(ActionState.OnSaw);
        UI_Manager.ChangeSawButtons(false);
        UI_Manager.DisplaySawButtons();
        GameRuler.gameObject.SetActive(false);
        CutGameplay.enabled = true;
    }

    public void SetupForMeasuring()
    {
        currentSpawnPoint = FromRulerSpawnPoint;
        if (previousAction == ActionState.None || previousAction == ActionState.OnSaw || currentAction == ActionState.OnSaw)
        {
            AvailableWoodMaterial[currentPieceIndex].transform.rotation = Quaternion.identity;
            PlacePiece();
            orbitCamera.enabled = false;
            panCamera.enabled = true;
            panCamera.ChangeDistanceConstraints(1.0f, 0.1f, 2.0f);
            panCamera.ChangeAngle(0.0f, 89.5f);
        }
        EnableCurrentBoardMovement(false);
        SwitchAction(ActionState.UsingRuler);
        SawBlade.TurnOff();
        UI_Manager.ChangeSawButtons(false);
        UI_Manager.DisplayBoardRotationButtons();
        GameRuler.gameObject.SetActive(true);
        GameRuler.CanMeasure = true;
        CutGameplay.enabled = false;
    }

    public void EnableUI(bool enable)
    {
        if (enable)
        {
            UI_Manager.DisableAllButtons();
        }
        else
        {
            UI_Manager.EnableAllButtons();
        }
    }

    public CutLine GetNearestLine(Vector3 fromPosition)
    {
        bool lineFound = false;
        int nearestLineIndex = -1;
        float smallestDistance = 0.0f;
        for (int i = 0; i < LinesToCut.Count && !lineFound; i++)
        {
            if (LinesToCut[i].gameObject.transform.parent.gameObject.activeSelf)
            {
                float firstDistance = Vector3.Distance(fromPosition, LinesToCut[i].Checkpoints[0].GetPosition());
                float lastDistance = Vector3.Distance(fromPosition, LinesToCut[i].Checkpoints[LinesToCut[i].Checkpoints.Count - 1].GetPosition());

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
        return LinesToCut[nearestLineIndex];
    }
}


//AvailableWoodMaterial = GameManager.instance.GetNecessaryMaterials(CutLineType.TableSawCut);
//LinesToCut = new List<CutLine>();
//foreach (GameObject go in AvailableWoodMaterial)
//{
//    WoodMaterialObject wood = go.GetComponent<WoodMaterialObject>();
//    LinesToCut.AddRange(wood.RetrieveLines(CutLineType.TableSawCut, GameManager.instance.GetStep()));
//    BoardController controller = go.AddComponent<BoardController>();
//    controller.Moveable = true;
//    controller.WoodObject = wood;
//    go.SetActive(false);
//}
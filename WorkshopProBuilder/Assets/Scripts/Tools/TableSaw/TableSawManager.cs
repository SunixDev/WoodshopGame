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
    public CameraControl GameCamera;
    public TableSawUI UI_Manager;
    public Blade SawBlade;
    public Ruler GameRuler;
    public TableSawCut CutGameplay;

    private int currentPieceIndex = 0;
    private Transform currentSpawnPoint;
    private ActionState currentAction = ActionState.None;
    private ActionState previousAction = ActionState.None;
    private BoardController currentBoardController;

	void Start ()
    {
        GameRuler.AssignManager(this);
        AvailableWoodMaterial = GameManager.instance.GetNecessaryMaterials(CutLineType.TableSawCut);
        LinesToCut = new List<CutLine>();
        foreach (GameObject go in AvailableWoodMaterial)
        {
            WoodMaterialObject wood = go.GetComponent<WoodMaterialObject>();
            LinesToCut.AddRange(wood.RetrieveLines(CutLineType.TableSawCut, GameManager.instance.GetStep()));
            BoardController controller = go.AddComponent<BoardController>();
            controller.Moveable = true;
            controller.WoodObject = wood;
            go.SetActive(false);
        }
        AvailableWoodMaterial[currentPieceIndex].SetActive(true);
        currentBoardController = AvailableWoodMaterial[currentPieceIndex].GetComponent<BoardController>();
        UI_Manager.UpdateSelectionButtons(currentPieceIndex, AvailableWoodMaterial.Count);
        SetupForCutting();
	}

    public void SplitMaterial(CutLine lineToRemove)
    {
        WoodMaterialObject board = AvailableWoodMaterial[currentPieceIndex].GetComponent<WoodMaterialObject>();
        LinesToCut.Remove(lineToRemove);
        AvailableWoodMaterial.RemoveAt(currentPieceIndex);
        GameManager.instance.WoodManager.SplitBoard(lineToRemove.GetFirstBaseNode(),
                                                    lineToRemove.GetSecondBaseNode(),
                                                    board, lineToRemove);

        GameManager.instance.WoodManager.HideAllPieces();

        if (LinesToCut.Count != 0)
        {
            AvailableWoodMaterial = GameManager.instance.GetNecessaryMaterials(CutLineType.TableSawCut);
            foreach (GameObject go in AvailableWoodMaterial)
            {
                if (go.GetComponent<BoardController>() == null)
                {
                    WoodMaterialObject wood = go.GetComponent<WoodMaterialObject>();
                    BoardController controller = go.AddComponent<BoardController>();
                    controller.Moveable = true;
                    controller.WoodObject = wood;
                }
                go.SetActive(false);
            }
            currentPieceIndex = 0;
            AvailableWoodMaterial[currentPieceIndex].SetActive(true);
            currentBoardController = AvailableWoodMaterial[currentPieceIndex].GetComponent<BoardController>();
            SetupForCutting();
            PlacePiece();
            UI_Manager.UpdateSelectionButtons(currentPieceIndex, AvailableWoodMaterial.Count);
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
        SwitchPiece(currentPieceIndex + 1);
        UI_Manager.UpdateSelectionButtons(currentPieceIndex, AvailableWoodMaterial.Count);
    }

    public void SwitchToPreviousPiece()
    {
        SwitchPiece(currentPieceIndex - 1);
        UI_Manager.UpdateSelectionButtons(currentPieceIndex, AvailableWoodMaterial.Count);
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
        foreach (GameObject go in AvailableWoodMaterial)
        {
            if (go.GetComponent<BoardController>())
            {
                Destroy(go.GetComponent<BoardController>());
            }
        }
        Application.LoadLevel(level);
    }

    public void SetupForCutting()
    {
        currentSpawnPoint = FromSawSpawnPoint;
        if (previousAction == ActionState.None || (previousAction == ActionState.UsingRuler && currentAction == ActionState.ChangingCamera) || currentAction == ActionState.UsingRuler)
        {
            AvailableWoodMaterial[currentPieceIndex].transform.rotation = Quaternion.identity;
            PlacePiece();
            GameCamera.ChangeLookAtPoint(CameraSawLookAtPoint);
            GameCamera.ChangeDistanceVariables(1.5f, 0.5f, 5.0f);
            GameCamera.ChangeVerticalRotationLimit(10.0f, 80.0f);
            GameCamera.ChangeAngle(0.0f, 45.0f);
        }
        if (currentAction != ActionState.ChangingCamera)
        {
            SawBlade.TurnOff();
        }
        GameCamera.EnableRotation(true);
        GameCamera.EnableMovement(false);
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
        if (previousAction == ActionState.None || (previousAction == ActionState.OnSaw && currentAction == ActionState.ChangingCamera) || currentAction == ActionState.OnSaw)
        {
            AvailableWoodMaterial[currentPieceIndex].transform.rotation = Quaternion.identity;
            PlacePiece();
            GameCamera.ChangeLookAtPoint(CameraRulerLookAtPoint);
            GameCamera.ChangeDistanceVariables(1.0f, 0.1f, 2.0f);
            GameCamera.ChangeVerticalRotationLimit(0.0f, 180.0f);
            GameCamera.ChangeAngle(0.0f, 90.0f);
        }
        GameCamera.EnableRotation(false);
        GameCamera.EnableMovement(false);
        EnableCurrentBoardMovement(false);
        SwitchAction(ActionState.UsingRuler);
        SawBlade.TurnOff();
        UI_Manager.ChangeSawButtons(false);
        UI_Manager.DisplayBoardRotationButtons();
        GameRuler.gameObject.SetActive(true);
        GameRuler.CanMeasure = true;
        CutGameplay.enabled = false;
    }

    public void SetupForCameraControl()
    {
        SwitchAction(ActionState.ChangingCamera);
        GameCamera.EnableMovement(true);
        EnableCurrentBoardMovement(false);
        GameRuler.CanMeasure = false;
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
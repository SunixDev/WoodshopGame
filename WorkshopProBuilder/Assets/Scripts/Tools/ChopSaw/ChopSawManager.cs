using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChopSawManager : MonoBehaviour, IToolManager
{
    public List<GameObject> AvailableWoodMaterial;
    public List<CutLine> LinesToCut;
    public Transform FromSawSpawnPoint;
    public Transform CameraSawLookAtPoint;
    public Transform FromRulerSpawnPoint;
    public Transform CameraRulerLookAtPoint;
    public CameraControl GameCamera;
    public ChopSawUI UI_Manager;
    public Blade SawBlade;
    public Ruler GameRuler;
    public ChopSawCut CutGameplay;
    public ChopSawController SawController;

    private int currentPieceIndex = 0;
    private Transform currentSpawnPoint;
    private ActionState currentAction = ActionState.None;
    private ActionState previousAction = ActionState.None;
    private BoardController currentBoardController;

	void Start () 
    {
        GameRuler.AssignManager(this);
        SawController.EnableMovement(false);
        AvailableWoodMaterial = GameManager.instance.GetNecessaryMaterials(CutLineType.ChopSawCut);
        LinesToCut = new List<CutLine>();
        foreach (GameObject go in AvailableWoodMaterial)
        {
            WoodMaterialObject wood = go.GetComponent<WoodMaterialObject>();
            LinesToCut.AddRange(wood.RetrieveLines(CutLineType.ChopSawCut, GameManager.instance.GetStep()));
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
            AvailableWoodMaterial = GameManager.instance.GetNecessaryMaterials(CutLineType.ChopSawCut);
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
            if (currentAction == ActionState.OnSaw)
            {
                PlacePieceAtSpawnPoint(new Vector3(0.0f, 0.0f, -3.0f));
            }
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
        if (currentAction == ActionState.OnSaw)
        {
            PlacePieceAtSpawnPoint(new Vector3(0.0f, 0.0f, -3.0f));
        }
        else if (currentAction == ActionState.UsingRuler)
        {
            PlacePieceAtSpawnPoint(new Vector3(-3.0f, 0.0f, 0.0f));            
        }
    }

    public void RotateCurrentBoard(float angle)
    {
        currentBoardController.ApplyRotation(new Vector3(0.0f, 1.0f, 0.0f), angle);
        if (currentAction == ActionState.OnSaw)
        {
            PlacePieceAtSpawnPoint(new Vector3(0.0f, 0.0f, -3.0f));
        }
        else if (currentAction == ActionState.UsingRuler)
        {
            PlacePieceAtSpawnPoint(new Vector3(-3.0f, 0.0f, 0.0f));
        }
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
            PlacePieceAtSpawnPoint(new Vector3(0.0f, 0.0f, -3.0f));
        }
        else if (currentAction == ActionState.UsingRuler || previousAction == ActionState.UsingRuler)
        {
            EnableCurrentBoardMovement(false);
            PlacePieceAtSpawnPoint(new Vector3(-3.0f, 0.0f, 0.0f));
        }
    }

    public void SwitchAction(ActionState actionState)
    {
        if (currentAction != actionState)
        {
            previousAction = currentAction;
            currentAction = actionState;
        }
    }

    public void PlacePieceAtSpawnPoint(Vector3 distanceFromPoint)
    {
        AvailableWoodMaterial[currentPieceIndex].transform.position = currentSpawnPoint.position + distanceFromPoint;
        Vector3 directionToPiece = (AvailableWoodMaterial[currentPieceIndex].transform.position - currentSpawnPoint.position).normalized;
        Ray ray = new Ray(currentSpawnPoint.position, directionToPiece);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            float distance = (hit.point - currentSpawnPoint.position).magnitude;
            AvailableWoodMaterial[currentPieceIndex].transform.position += (distance * -directionToPiece);
        }
    }

    public bool AreAllLinesCut()
    {
        return (LinesToCut.Count <= 0);
    }

    public void SetupForCutting()
    {
        currentSpawnPoint = FromSawSpawnPoint;
        if (previousAction == ActionState.None || (previousAction == ActionState.UsingRuler && currentAction == ActionState.ChangingCamera) || currentAction == ActionState.UsingRuler)
        {
            AvailableWoodMaterial[currentPieceIndex].transform.rotation = Quaternion.identity;
            PlacePieceAtSpawnPoint(new Vector3(0.0f, 0.0f, -3.0f));
            GameCamera.ChangeLookAtPoint(CameraSawLookAtPoint);
            GameCamera.ChangeDistanceVariables(1.5f, 0.2f, 2.0f);
            GameCamera.ChangeVerticalRotationLimit(30.0f, 80.0f);
            GameCamera.ChangeAngle(90.0f, 45.0f);
        }
        if (currentAction != ActionState.ChangingCamera)
        {
            SawBlade.TurnOff();
            SawController.EnableMovement(false);
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
            PlacePieceAtSpawnPoint(new Vector3(-3.0f, 0.0f, 0.0f));
            GameCamera.ChangeLookAtPoint(CameraRulerLookAtPoint);
            GameCamera.ChangeDistanceVariables(1.0f, 0.1f, 2.0f);
            GameCamera.ChangeVerticalRotationLimit(0.0f, 180.0f);
            GameCamera.ChangeAngle(90.0f, 89.9f);
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
        SawController.EnableMovement(false);
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
            float firstDistance = Vector3.Distance(fromPosition, LinesToCut[i].Checkpoints[0].GetPosition());
            float lastDistance = Vector3.Distance(fromPosition, LinesToCut[i].Checkpoints[LinesToCut[i].Checkpoints.Count - 1].GetPosition());

            if (i == 0 || firstDistance < smallestDistance || lastDistance < smallestDistance)
            {
                nearestLineIndex = i;
                if (i == 0)
                {
                    smallestDistance = (firstDistance < lastDistance) ? firstDistance : lastDistance;
                }
                else
                {
                    smallestDistance = (firstDistance < smallestDistance) ? firstDistance : smallestDistance;
                    smallestDistance = (lastDistance < smallestDistance) ? lastDistance : smallestDistance;
                }
            }
        }
        return LinesToCut[nearestLineIndex];
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//public enum DadoCutActionState
//{
//    UsingSaw,
//    PreppingTools,
//    SettingUpWood,
//    None
//}

public class DadoCutManager : MonoBehaviour 
{
    public List<GameObject> AvailableWoodMaterial;
    public List<DadoBlock> DadosToCut;
    public Transform FromSawSpawnPoint;
    public Transform CameraSawLookAtPoint;
    public Transform CameraRulerLookAtPoint;
    public CameraControl GameCamera;
    public DadoCutUI UI_Manager;
    public Blade SawBlade;
    public GameObject GameRuler;
    public DadoCutting CutGameplay;
    public BladeController BladeControl;

    private int currentPieceIndex = 0;
    private Transform currentSpawnPoint;
    private ActionState currentAction = ActionState.OnSaw;
    private ActionState previousAction = ActionState.None;
    private BoardController currentBoardController;

	void Start () 
    {
        BladeControl.Moveable = false;
        AvailableWoodMaterial = GameManager.instance.GetMaterialsWithDadoCuts();
        DadosToCut = new List<DadoBlock>();
        foreach (GameObject go in AvailableWoodMaterial)
        {
            WoodMaterialObject wood = go.GetComponent<WoodMaterialObject>();
            DadosToCut.AddRange(wood.RetrieveDadoCuts(GameManager.instance.GetStep()));
            BoardController controller = go.AddComponent<BoardController>();
            controller.Moveable = true;
            controller.WoodObject = wood;
            go.SetActive(false);
        }
        foreach (DadoBlock dadoBlock in DadosToCut)
        {
            MeshRenderer meshRenderer = dadoBlock.GetComponent<MeshRenderer>();
            meshRenderer.material.color = Color.white;
        }
        AvailableWoodMaterial[currentPieceIndex].SetActive(true);
        currentBoardController = AvailableWoodMaterial[currentPieceIndex].GetComponent<BoardController>();
        UI_Manager.UpdateSelectionButtons(currentPieceIndex, AvailableWoodMaterial.Count);
        SetupForCutting();
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

    public bool AreAllDadosCutOut()
    {
        return (DadosToCut.Count <= 0);
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
        GameRuler.SetActive(false);
        CutGameplay.enabled = true;
        BladeControl.Moveable = false;
    }

    public void SetupForMeasuring()
    {
        if (previousAction == ActionState.None || (previousAction == ActionState.OnSaw && currentAction == ActionState.ChangingCamera) || currentAction == ActionState.OnSaw)
        {
            GameCamera.ChangeLookAtPoint(CameraRulerLookAtPoint);
            GameCamera.ChangeDistanceVariables(0.1f, 0.1f, 0.2f);
            GameCamera.ChangeVerticalRotationLimit(0.0f, 180.0f);
            GameCamera.ChangeAngle(90.0f, 0.0f);
        }
        GameCamera.EnableRotation(false);
        GameCamera.EnableMovement(false);
        EnableCurrentBoardMovement(false);
        SwitchAction(ActionState.UsingRuler);
        SawBlade.TurnOff();
        UI_Manager.ChangeSawButtons(false);
        GameRuler.SetActive(true);
        CutGameplay.enabled = false;
        BladeControl.Moveable = true;
    }

    public void SetupForCameraControl()
    {
        SwitchAction(ActionState.ChangingCamera);
        GameCamera.EnableMovement(true);
        EnableCurrentBoardMovement(false);
        BladeControl.Moveable = false;
    }

    public DadoBlock GetNearestDadoBlock(Vector3 fromPosition)
    {
        bool lineFound = false;
        int nearestIndex = -1;
        float smallestDistance = 0.0f;
        for (int i = 0; i < DadosToCut.Count && !lineFound; i++)
        {
            float distance = Vector3.Distance(fromPosition, DadosToCut[i].transform.position);

            if (i == 0 || distance < smallestDistance)
            {
                nearestIndex = i;
                smallestDistance = distance;
            }
        }
        return DadosToCut[nearestIndex];
    }
}

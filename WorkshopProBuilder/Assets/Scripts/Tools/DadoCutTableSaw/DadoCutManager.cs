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
    public bool StillCutting { get; set; }

    private int currentPieceIndex = 0;
    private Transform currentSpawnPoint;
    private ActionState currentAction = ActionState.OnSaw;
    private ActionState previousAction = ActionState.None;
    private BoardController currentBoardController;
    private float cumulativeLineScore = 0.0f;
    private float numberOfCuts;

	void Start () 
    {
        numberOfCuts = DadosToCut.Count;
        UI_Manager.DisplayPlans(true);
        StillCutting = true;
        BladeControl.Moveable = false;
        foreach (GameObject go in AvailableWoodMaterial)
        {
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

    public void SplitMaterial(DadoBlock dadoToRemove)
    {
        if (!dadoToRemove.AnyCutsLeft())
        {
            WoodMaterialObject board = AvailableWoodMaterial[currentPieceIndex].GetComponent<WoodMaterialObject>();
            DadosToCut.Remove(dadoToRemove);
            board.RemoveDado(dadoToRemove);
            Destroy(dadoToRemove.gameObject);

            if (board.DadosToCut.Count <= 0)
            {
                AvailableWoodMaterial.RemoveAt(currentPieceIndex);
                Destroy(board.gameObject);
                if (AvailableWoodMaterial.Count > 0)
                {
                    currentPieceIndex = 0;
                    AvailableWoodMaterial[currentPieceIndex].SetActive(true);
                    currentBoardController = AvailableWoodMaterial[currentPieceIndex].GetComponent<BoardController>();
                    UI_Manager.UpdateSelectionButtons(currentPieceIndex, AvailableWoodMaterial.Count);
                    SetupForCutting();
                    PlacePiece();
                }
            }
        }
        SawBlade.TurnOff();
        UI_Manager.ChangeSawButtons(false);

        if (DadosToCut.Count > 0)
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
                Debug.Log("No Game manager");
            }
        }
    }

    public void RestrictCurrentBoardMovement(bool restrictZ, bool restrictX)
    {
        currentBoardController.RestrictZ = restrictZ;
        currentBoardController.RestrictX = restrictX;
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
        AvailableWoodMaterial[currentPieceIndex].transform.rotation = Quaternion.Euler(0.0f, 90.0f, 0.0f);
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
        AvailableWoodMaterial[currentPieceIndex].transform.rotation = Quaternion.Euler(new Vector3(0.0f, 90.0f, 0.0f));
        AvailableWoodMaterial[currentPieceIndex].transform.position = currentSpawnPoint.position;// +new Vector3(0.0f, 2.0f, 0.0f);
        //Ray ray = new Ray(currentSpawnPoint.position, -Vector3.up);
        //RaycastHit hit;
        //if (Physics.Raycast(ray, out hit))
        //{
        //    Ray toPieceRay = new Ray(hit.point, Vector3.up);
        //    RaycastHit toPiecehit;
        //    if (Physics.Raycast(toPieceRay, out toPiecehit))
        //    {
        //        float distance = (hit.point - toPiecehit.point).magnitude;
        //        AvailableWoodMaterial[currentPieceIndex].transform.position += (distance * -Vector3.up);
        //    }
        //}
    }

    public bool AreAllDadosCutOut()
    {
        return (DadosToCut.Count <= 0);
    }

    public void SwitchScene(string level)
    {
        Application.LoadLevel(level);
    }
    public void SetupForCutting()
    {
        currentSpawnPoint = FromSawSpawnPoint;
        if (previousAction == ActionState.None || (previousAction == ActionState.UsingRuler && currentAction == ActionState.ChangingCamera) || currentAction == ActionState.UsingRuler)
        {
            AvailableWoodMaterial[currentPieceIndex].transform.rotation = Quaternion.Euler(0.0f, 90.0f, 0.0f);
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


//public void EnableBoardPhysics(bool enable)
//{
//    WoodMaterialObject wood = AvailableWoodMaterial[currentPieceIndex].GetComponent<WoodMaterialObject>();
//    wood.GetComponent<Rigidbody>().isKinematic = !enable;
//    foreach (GameObject woodPiece in wood.WoodPieces)
//    {
//        if (woodPiece.tag == "Piece" || woodPiece.tag == "Leftover")
//        {
//            MeshCollider collider = woodPiece.GetComponent<MeshCollider>();
//            if (collider != null)
//            {
//                collider.convex = enable;
//            }
//        }
//    }
//}
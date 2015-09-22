using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor;

public class SwipeManager : MonoBehaviour 
{
    public List<GameObject> AvailablePieces;
    public Transform PieceSpawnPoint;
    public SwipeGameplay SwipeGameplayManager;
    public Material PaintingMaterial;
    public Material SandingMaterial;
    public Material LaqcuerMaterial;
    public SwipeUI UI_Manager;
    public bool PieceRotationEnabled { get; set; }

    private int currentPieceIndex = 0;

    void Start()
    {
        PieceRotationEnabled = false;
        AvailablePieces = GameManager.instance.GetNecessaryPieces();
        Step currentStep = GameManager.instance.CurrentProject.GetCurrentStepObject();
        Material selectedMaterial = PaintingMaterial;
        if (currentStep.ToolToUse == ToolType.PaintBrush)
        {
            SwipeGameplayManager.type = SwipeGameType.Paint;
            selectedMaterial = PaintingMaterial;
        }
        else if (currentStep.ToolToUse == ToolType.Sander)
        {
            SwipeGameplayManager.type = SwipeGameType.Sanding;
            selectedMaterial = SandingMaterial;
        }
        else if (currentStep.ToolToUse == ToolType.Stainer)
        {
            SwipeGameplayManager.type = SwipeGameType.Lacquer;
            selectedMaterial = LaqcuerMaterial;
        }
        else
        {
            Debug.LogError("The current step is not set to any of the necessary tool types!");
            SwipeGameplayManager.SwipeEnabled = false;
            return;
        }

        foreach (GameObject piece in AvailablePieces)
        {
            piece.SetActive(false);
            piece.GetComponent<WoodPiece>().EnableConvexCollider(false);
            piece.GetComponent<WoodPiece>().ChangeMaterial(selectedMaterial);
        }
        SwipeGameplayManager.CurrentPiece = AvailablePieces[currentPieceIndex];
        SwipeGameplayManager.CurrentPiece.SetActive(true);
        SwipeGameplayManager.Setup();
        PlacePiece();
    }

    public void SwitchToNextPiece()
    {
        SwitchPiece(currentPieceIndex + 1);
    }

    private void SwitchPiece(int indexToSwitchTo)
    {
        AvailablePieces[currentPieceIndex].transform.position = Vector3.zero;
        AvailablePieces[currentPieceIndex].SetActive(false);
        AvailablePieces[indexToSwitchTo].SetActive(true);
        AvailablePieces[indexToSwitchTo].GetComponent<WoodPiece>().EnableConvexCollider(false);
        currentPieceIndex = indexToSwitchTo;
        SwipeGameplayManager.CurrentPiece = AvailablePieces[currentPieceIndex];
        PlacePiece();
    }

    public void PlacePiece()
    {
        AvailablePieces[currentPieceIndex].transform.position = PieceSpawnPoint.position;
    }

    public void RotateObject(Gesture gesture)
    {
        if (PieceRotationEnabled)
        {
            AvailablePieces[currentPieceIndex].transform.Rotate(gesture.deltaPosition.y, -gesture.deltaPosition.x, 0.0f, Space.World);
        }
    }

    public void PieceCompleted()
    {

        Texture2D textureToAnalyze = SwipeGameplayManager.GetPaintedTexture();
        Debug.Log(textureToAnalyze);
        SwipeGameplayManager.ResetSwipeBackgroundTexture();
        if (currentPieceIndex != AvailablePieces.Count - 1)
        {
            SwitchToNextPiece();
        }
        else
        {
            Debug.Log("All pieces are done");
        }
    }

    void OnEnable()
    {
        EasyTouch.On_Drag += RotateObject;
    }

    void OnDisable()
    {
        EasyTouch.On_Drag -= RotateObject;
    }

    void OnDestory()
    {
        EasyTouch.On_Drag -= RotateObject;
    }
}

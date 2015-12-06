using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SnapManager : MonoBehaviour 
{
    public GameObject GameCamera;
    public Transform CameraStartingPosition;
    public Transform CameraLookAtTransform;
    public PlayerPieceSnapping PieceSnapping;
    public DragButtonContainer ButtonContainer;

    private List<GameObject> DraggablePieces = new List<GameObject>();
    private Transform cameraTransform;
    private OrbitCamera cameraControl;

	void Start () 
    {
        PieceSnapping.enabled = false;
        cameraTransform = GameCamera.transform;
        cameraControl = GameCamera.GetComponent<OrbitCamera>();
	}

    public void SetUpPieceSnapping()
    {
        GameCamera.transform.position = CameraStartingPosition.position;
        cameraControl.LookAtPoint = CameraLookAtTransform;
        cameraControl.enabled = true;
        PieceSnapping.enabled = true;
    }

    public void PausePieceSnapping()
    {
        cameraControl.enabled = false;
        PieceSnapping.enabled = false;
    }

    public SnapPiece GetDraggablePiece(int index)
    {
        return DraggablePieces[index].GetComponent<SnapPiece>();
    }

    internal void AddDragPiece(GameObject gameObject)
    {
        DraggablePieces.Add(gameObject);
    }
}

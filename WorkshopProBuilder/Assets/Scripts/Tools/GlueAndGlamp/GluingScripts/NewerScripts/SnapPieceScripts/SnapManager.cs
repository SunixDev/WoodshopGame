using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SnapManager : MonoBehaviour 
{
    public GameObject GameCamera;
    public Transform CameraStartingPosition;
    public Transform CameraLookAt;
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
        ActivatePieceSnapping();
	}

    public void ActivatePieceSnapping()
    {
        GameCamera.transform.position = CameraStartingPosition.position;
        cameraControl.LookAtPoint = CameraLookAt;
        cameraControl.Distance = 1.5f;
        cameraControl.ChangeAngle(0f, 0f);
        cameraControl.EnableZoom = true;
        PieceSnapping.enabled = true;
    }

    public void PausePieceSnapping()
    {
        PieceSnapping.enabled = false;
    }

    public SnapPiece GetDraggablePiece(int index)
    {
        return DraggablePieces[index].GetComponent<SnapPiece>();
    }

    public void AddDragPiece(GameObject gameObject)
    {
        DraggablePieces.Add(gameObject);
    }


    #region Touch Events
    private void EnableTouchEvents()
    {

    }

    private void DisableTouchEvents()
    {

    }

    void OnEnable()
    {
        EnableTouchEvents();
    }
    void OnDisable()
    {
        DisableTouchEvents();
    }
    void OnDestroy()
    {
        DisableTouchEvents();
    }
    #endregion
}

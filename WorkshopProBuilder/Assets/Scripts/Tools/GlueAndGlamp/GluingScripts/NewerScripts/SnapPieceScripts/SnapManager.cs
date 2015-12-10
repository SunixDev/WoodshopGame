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
    public GameObject ConnectedProject;
    public SnapPieceGameManager MiniGameManager;

    private int DragPieceIndex;
    private List<GameObject> DraggablePieces = new List<GameObject>();
    private Transform cameraTransform;
    private OrbitCamera cameraControl;

	void Awake () 
    {
        DragPieceIndex = -1;
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

    public void OnDragButtonTouch(Gesture gesture)
    {
        if (ButtonContainer.ContainsButton(gesture.pickedUIElement))
        {
            if (gesture.pickedUIElement.GetComponent<UIDragButton>() != null)
            {
                DragPieceIndex = ButtonContainer.IndexOfButton(gesture.pickedUIElement);
                PieceSnapping.StartPieceSnapping(DraggablePieces[DragPieceIndex], gesture.pickedUIElement);
            }
        }
    }

    public void ConnectPieces(SnapPiece draggedPiece, SnapPiece otherPiece)
    {
        List<SnapPoint> pointsReadyToConnect;
        bool PieceSnapIsPossible = EvaluateAnchorPoints(draggedPiece, otherPiece, out pointsReadyToConnect);
        if (PieceSnapIsPossible && pointsReadyToConnect.Count > 0 && DragPieceIndex >= 0)
        {
            draggedPiece.gameObject.SetActive(true);
            draggedPiece.SnapToProject(ConnectedProject.transform);
            ConnectedProject.GetComponent<WoodProject>().AddPieceToProject(draggedPiece.gameObject);
            foreach (SnapPoint point in pointsReadyToConnect)
            {
                point.IsConnected = true;
                MiniGameManager.UpdateConnections();
            }
            MiniGameManager.UpdateGame(DraggablePieces[DragPieceIndex]);
            UpdateAvailablePieces();
        }
        ResetSelection();
    }

    private bool EvaluateAnchorPoints(SnapPiece piece, SnapPiece otherPiece, out List<SnapPoint> pointsReadyToConnect)
    {
        pointsReadyToConnect = new List<SnapPoint>();
        bool AllPointsCanConnect = true;
        for (int i = 0; i < piece.SnapPoints.Count && AllPointsCanConnect; i++)
        {
            SnapPoint currentPoint = piece.SnapPoints[i];
            if (!currentPoint.IsConnected)
            {
                for (int j = 0; j < otherPiece.SnapPoints.Count && AllPointsCanConnect; j++)
                {
                    SnapPoint otherPoint = otherPiece.SnapPoints[j];
                    if (currentPoint.ConnectionID == otherPoint.ConnectionID && !otherPoint.IsConnected)
                    {
                        if (currentPoint.CanConnect && otherPoint.CanConnect)
                        {
                            pointsReadyToConnect.Add(currentPoint);
                            pointsReadyToConnect.Add(otherPoint);
                        }
                        else
                        {
                            AllPointsCanConnect = false;
                            pointsReadyToConnect = new List<SnapPoint>();
                        }
                    }
                }
            }
        }
        return AllPointsCanConnect;
    }

    public void UpdateAvailablePieces()
    {
        DraggablePieces[DragPieceIndex] = null;
        ButtonContainer.DisableButton(DragPieceIndex);
    }

    public void AddDragPiece(GameObject gameObject)
    {
        DraggablePieces.Add(gameObject);
    }

    public void ResetSelection()
    {
        DragPieceIndex = -1;
    }



    #region Touch Events
    private void EnableTouchEvents()
    {
        EasyTouch.On_TouchStart += OnDragButtonTouch;
    }

    private void DisableTouchEvents()
    {
        EasyTouch.On_TouchStart -= OnDragButtonTouch;
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

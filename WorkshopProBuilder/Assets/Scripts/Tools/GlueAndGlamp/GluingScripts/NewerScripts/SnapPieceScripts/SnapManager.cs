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
    public Transform ConnectedProject;
    public SnapPieceGameManager MiniGameManager;

    private List<GameObject> DraggablePieces = new List<GameObject>();
    public List<SnapPoint> SnapPointsToConnect = new List<SnapPoint>();
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

    public void OnDragButtonTouch(Gesture gesture)
    {
        if (ButtonContainer.ContainsButton(gesture.pickedUIElement))
        {
            if (gesture.pickedUIElement.GetComponent<UIDragButton>() != null)
            {
                int pieceIndex = ButtonContainer.IndexOfButton(gesture.pickedUIElement);
                PieceSnapping.StartPieceSnapping(DraggablePieces[pieceIndex], gesture.pickedUIElement);
            }
        }
    }

    public void ConnectPieces(SnapPiece draggedPiece, SnapPiece otherPiece)
    {
        List<SnapPoint> pointsReadyToConnect;
        bool PieceSnapIsPossible = EvaluateAnchorPoints(draggedPiece, otherPiece, out pointsReadyToConnect);
        if (PieceSnapIsPossible && pointsReadyToConnect.Count > 0)
        {
            draggedPiece.SnapToProject(ConnectedProject);
            foreach (SnapPoint point in pointsReadyToConnect)
            {
                point.IsConnected = true;
                MiniGameManager.UpdateConnections();
            }
        }
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
                        }
                    }
                }
            }
        }
        return AllPointsCanConnect;
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

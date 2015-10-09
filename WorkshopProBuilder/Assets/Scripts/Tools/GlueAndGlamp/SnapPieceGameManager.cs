using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SnapPieceGameManager : MonoBehaviour 
{
    public List<GameObject> AvailablePieces;
    public SnapPieceUI UI_Manager;
    public CameraControl GameCamera;
    public Transform SpawnPoint;
    public float ValidConnectionDistance = 0.1f;

    public List<GlueBox> Glues { get; set; }
    public List<SnapPoint> SnapPoints { get; set; }

    private WoodPiece CurrentPiece;
    private int currentPieceIndex = 0;
    private List<GameObject> ConnectedPieces;

	void Start () 
    {
        AvailablePieces = GameManager.instance.GetNecessaryPieces();
        foreach (GameObject go in AvailablePieces)
        {
            WoodPiece wood = go.GetComponent<WoodPiece>();
            Glues = wood.ActivateGlueBoxes(GameManager.instance.GetStep());
            SnapPoints = wood.ActivateSnapPoints(GameManager.instance.GetStep());
        }

	}

    void Update()
    {
        bool piecesConnected = true;
        for (int i = 0; i < SnapPoints.Count && piecesConnected; i++)
        {
            piecesConnected = SnapPoints[i].IsConnected;
        }

        if (!piecesConnected)
        {
            foreach (GameObject availablePiece in AvailablePieces)
            {
                WoodPiece piece = availablePiece.GetComponent<WoodPiece>();
                if (piece != CurrentPiece)
                {
                    foreach (SnapPoint otherPoint in piece.SnapPoints)
                    {
                        foreach (SnapPoint currentPoint in CurrentPiece.SnapPoints)
                        {
                            if (currentPoint.CanConnectTo(otherPoint) && currentPoint.DistanceFromPoint(otherPoint) <= ValidConnectionDistance)
                            {
                                currentPoint.ConnectToPoint(otherPoint);
                                break;
                            }
                        }
                    }
                }
            }
        }
        else
        {
            Debug.Log("Step Done");
        }
    }

    public void SwitchFromPiece(WoodPiece piece)
    {
        AvailablePieces.Remove(piece.gameObject);
    }

    public void SwitchPiece(int index)
    {
        if (index >= 0 && index < AvailablePieces.Count && index != currentPieceIndex)
        {
            AvailablePieces[currentPieceIndex].SetActive(false);
            AvailablePieces[index].SetActive(true);
            AvailablePieces[index].transform.position = SpawnPoint.position;
            AvailablePieces[index].transform.rotation = Quaternion.identity;
        }
        else
        {
            Debug.Log("Index #" + index + " is invalid.");
            Debug.Log("Index #" + index + " is invalid.");
        }
    }
}

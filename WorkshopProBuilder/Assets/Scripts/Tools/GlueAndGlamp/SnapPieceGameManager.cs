using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SnapPieceGameManager : MonoBehaviour 
{
    public List<GameObject> AvailablePieces;
    public SnapPieceUI UI_Manager;
    public Transform InitialSpawnPoint;

    private int currentPieceIndex = 0;
    public int NumberOfGlues { get; set; }
    public int NumberOfSnapPoints { get; set; }

	void Start () 
    {
        AvailablePieces = GameManager.instance.GetNecessaryPieces();
        foreach (GameObject go in AvailablePieces)
        {
            WoodPiece wood = go.GetComponent<WoodPiece>();
            NumberOfGlues = wood.ActivateGlueBoxes(GameManager.instance.GetStep());
            NumberOfSnapPoints = wood.ActivateSnapPoints(GameManager.instance.GetStep());
        }
	}

    public bool StepCompleted()
    {
        return (NumberOfGlues == 0 && NumberOfSnapPoints == 0);
    }

    public void SwitchFromPiece(WoodPiece piece)
    {
        AvailablePieces.Remove(piece.gameObject);
    }

    public void SwitchPiece(int index)
    {

    }
}

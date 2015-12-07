using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerPieceSnapping : MonoBehaviour 
{
    public LayerMask pickableLayers;

    private SnapPiece snapPiece;
    private Image woodPieceIcon;
    private SnapPoint selectedOtherPoint;
    private bool placingPiece;
    private Vector2 currentTouchPosition;
    private Vector2 previousTouchPosition;

    void Awake()
    {
        snapPiece = null;
        woodPieceIcon = null;
        selectedOtherPoint = null;
        placingPiece = false;
        currentTouchPosition = Vector2.zero;
        previousTouchPosition = Vector2.zero;
	}
	
	void Update () 
    {

	}

    public void SetSelectedPiece(GameObject pieceSelected, GameObject pieceButtonSelected)
    {
        if (ObjectIsWoodPieceAndDraggable(pieceSelected, pieceButtonSelected))
        {
            snapPiece = pieceSelected.GetComponent<SnapPiece>();
            woodPieceIcon = pieceButtonSelected.GetComponent<UIDragButton>().GetElementImage();
        }
    }

    private bool ObjectIsWoodPieceAndDraggable(GameObject piece, GameObject pieceButton)
    {
        bool valid = (piece.GetComponent<SnapPiece>() != null && pieceButton.GetComponent<UIDragButton>() != null);
        if (!valid)
        {
            if (piece.GetComponent<SnapPiece>() == null)
            {
                Debug.LogError(piece + " does not have a SnapPiece component");
            }
            if (pieceButton.GetComponent<UIDragButton>() == null)
            {
                Debug.LogError(pieceButton + " does not have a UIDragButton component");
            }
        }
        return valid;
    }

    public void GetTouchPosition(Gesture gesture)
    {
        currentTouchPosition = gesture.position;
        placingPiece = true;
    }

    public void OnTouchRelease(Gesture gesture)
    {
        placingPiece = false;
    }



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
}

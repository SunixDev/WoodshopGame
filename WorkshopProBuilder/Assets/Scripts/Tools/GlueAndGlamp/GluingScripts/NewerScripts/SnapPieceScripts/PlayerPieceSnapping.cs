using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerPieceSnapping : MonoBehaviour 
{
    public Color NeutralDragColor = new Color(1f, 1f, 1f, 1f);
    public Color OverSnapPointDragColor = new Color(0.2f, 1f, 0.5f, 1f);
    public LayerMask pickableLayers;

    private SnapManager Manager;
    private SnapPiece snapPiece;
    private Image woodPieceIcon;
    private SnapPoint selectedOtherPoint;
    private bool placingPiece;
    private Vector2 currentTouchPosition;
    private Vector2 previousTouchPosition;

    void Awake()
    {
        ResetSnapping();
        currentTouchPosition = Vector2.zero;
        previousTouchPosition = Vector2.zero;
        Manager = GetComponent<SnapManager>();
	}
	
	void Update () 
    {
        if (previousTouchPosition != currentTouchPosition && placingPiece && PieceAndImageAreSet())
        {
            DetectSnapPoint();
        }
	}

    public void DetectSnapPoint()
    {
        Ray ray = Camera.main.ScreenPointToRay(currentTouchPosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f, pickableLayers))
        {
            string hitTag = hit.collider.gameObject.tag;
            SnapPoint otherPoint = hit.collider.gameObject.GetComponent<SnapPoint>();
            if (hitTag == "AnchorPoint" && otherPoint != null)
            {
                if (selectedOtherPoint != otherPoint && snapPiece.CanConnectAt(otherPoint))
                {
                    selectedOtherPoint = otherPoint;
                    woodPieceIcon.color = OverSnapPointDragColor;
                }
            }
            else
            {
                ResetSelection();
            }
        }
        else
        {
            ResetSelection();
        }
        previousTouchPosition = currentTouchPosition;
    }

    public void StartPieceSnapping(GameObject pieceSelected, GameObject pieceButtonSelected)
    {
        if (ObjectIsWoodPieceAndDraggable(pieceSelected, pieceButtonSelected))
        {
            snapPiece = pieceSelected.GetComponent<SnapPiece>();
            woodPieceIcon = pieceButtonSelected.GetComponent<UIDragButton>().GetElementImage();
            placingPiece = true;
        }
    }

    public void SetupPointsToConnect()
    {
        SnapPiece otherSnapPiece = selectedOtherPoint.ParentSnapPiece;
        Manager.ConnectPieces(snapPiece, otherSnapPiece);
        placingPiece = false;
        ResetSnapping();
    }


    public void ResetSnapping()
    {
        ResetSelection();
        snapPiece = null;
        woodPieceIcon = null;
        placingPiece = false;
    }

    private void ResetSelection()
    {
        selectedOtherPoint = null;
        if (woodPieceIcon != null)
        {
            woodPieceIcon.color = NeutralDragColor;
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

    private bool PieceAndImageAreSet()
    {
        return (woodPieceIcon != null && snapPiece != null);
    }

    public void GetTouchPosition(Gesture gesture)
    {
        if (gesture.touchCount == 1)
        {
            currentTouchPosition = gesture.position;
            placingPiece = true;
        }
    }

    public void OnTouchRelease(Gesture gesture)
    {
        if (selectedOtherPoint != null && placingPiece)
        {
            SetupPointsToConnect();
        }
    }



    private void EnableTouchEvents()
    {
        EasyTouch.On_TouchDown += GetTouchPosition;
        EasyTouch.On_TouchUp += OnTouchRelease;
    }

    private void DisableTouchEvents()
    {
        EasyTouch.On_TouchDown -= GetTouchPosition;
        EasyTouch.On_TouchUp -= OnTouchRelease;
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

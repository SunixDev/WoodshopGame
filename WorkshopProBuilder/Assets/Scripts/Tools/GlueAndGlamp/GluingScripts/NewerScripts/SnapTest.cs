using UnityEngine;
using System.Collections;

public class SnapTest : MonoBehaviour 
{
    public Transform projectCenter;

    private SnapPiece selectedPiece;
    private SnapPoint otherPoint;
    private SnapPoint selectedPiecePoint;
    private bool pieceSelected;
    private Vector2 touchPosition;

	void Start () 
    {
        otherPoint = null;
        selectedPiecePoint = null;
        touchPosition = Vector2.zero;
        selectedPiece = null;
        pieceSelected = false;
	}
	
	void Update () 
    {
        if (pieceSelected)
        {
            Ray ray = Camera.main.ScreenPointToRay(touchPosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log(hit.collider.gameObject);
                SnapPoint pointHit = hit.collider.gameObject.GetComponent<SnapPoint>();
                bool connectionPossible = false;
                if (pointHit != null)
                {
                    for (int i = 0; i < selectedPiece.SnapPoints.Count && !connectionPossible; i++)
                    {
                        if (selectedPiece.SnapPoints[i].CanConnectTo(pointHit))
                        {
                            otherPoint = pointHit;
                            selectedPiecePoint = selectedPiece.SnapPoints[i];
                            connectionPossible = true;
                        }
                    }
                }
                if (!connectionPossible)
                {
                    otherPoint = null;
                    selectedPiecePoint = null;
                }
            }
            else
            {
                otherPoint = null;
                selectedPiecePoint = null;
            }
        }
	}

    private void ConnectPieces()
    {
        selectedPiecePoint.ConnectPieceToPoint(otherPoint, projectCenter);
    }

    private void Reset()
    {
        if (selectedPiece != null)
        {
            selectedPiece.gameObject.GetComponent<Collider>().enabled = true;
        }
        selectedPiece = null;
        pieceSelected = false;
    }

    public void OnPieceSelected(Gesture gesture)
    {
        if (gesture.pickedObject != null)
        {
            if (gesture.pickedObject.tag == "Piece")
            {
                SnapPiece piece = gesture.pickedObject.GetComponent<SnapPiece>();

                pieceSelected = true;
                touchPosition = gesture.position;
                gesture.pickedObject.GetComponent<Collider>().enabled = false;
                selectedPiece = gesture.pickedObject.GetComponent<SnapPiece>();
            }
        }
    }

    public void UpdateTouchPosition(Gesture gesture)
    {
        if (pieceSelected)
        {
            touchPosition = gesture.position;
        }
    }

    public void OnPieceReleased(Gesture gesture)
    {
        if (otherPoint == null)
        {
            Reset();
        }
        else
        {
            ConnectPieces();
        }
    }



    private void EnableTouchEvents()
    {
        EasyTouch.On_TouchStart += OnPieceSelected;
        EasyTouch.On_Drag += UpdateTouchPosition;
        EasyTouch.On_TouchUp += OnPieceReleased;
    }

    private void DisableTouchEvents()
    {
        EasyTouch.On_TouchStart -= OnPieceSelected;
        EasyTouch.On_Drag -= UpdateTouchPosition;
        EasyTouch.On_TouchUp -= OnPieceReleased;     
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

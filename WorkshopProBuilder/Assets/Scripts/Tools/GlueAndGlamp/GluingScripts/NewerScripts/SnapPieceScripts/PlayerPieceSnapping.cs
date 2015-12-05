using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerPieceSnapping : MonoBehaviour 
{
    public LayerMask pickableLayers;

    private SnapPiece selectedPiece;
    private Image selectedPieceIcon;
    private SnapPoint pointToConnectTo;
    private bool placingPiece;
    private Vector2 touchPosition;

    void Awake()
    {
        selectedPiece = null;
        selectedPieceIcon = null;
        pointToConnectTo = null;
        placingPiece = false;
        touchPosition = Vector2.zero;
	}
	
	void Update () 
    {

	}

    private void EnableTouchEvents()
    {
        //EasyTouch.On_TouchDown += GetTouchPosition;
        //EasyTouch.On_TouchUp += OnTouchRelease;
    }

    private void DisableTouchEvents()
    {
        //EasyTouch.On_TouchDown -= GetTouchPosition;
        //EasyTouch.On_TouchUp -= OnTouchRelease;
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

using UnityEngine;
using System.Collections;

public class MoveToCamera : MonoBehaviour 
{
    public Transform PieceGluingPosition;
    public Transform CameraTransform;
    public PlayerGlue playerGlue;

    private Transform selectedPieceTransform;
    private Vector3 pieceOrigin = Vector3.zero;
    private Quaternion pieceOriginRotation = Quaternion.identity;
    private Vector3 cameraOrigin = Vector3.zero;

    void Start()
    {
        playerGlue.enabled = false;
    }

    public void OnDoubleTap(Gesture gesture)
    {
        if (gesture.pickedObject != null)
        {
            if (gesture.pickedObject.tag == "Piece" && !playerGlue.enabled)
            {
                selectedPieceTransform = gesture.pickedObject.transform;
                ActivateGluing();
            }
        }
    }

    public void OnPinchIn(Gesture gesture)
    {
        if (playerGlue.enabled)
        {
            DeactivateGluing();
        }
    }

    private void ActivateGluing()
    {
        playerGlue.enabled = true;
        pieceOrigin = selectedPieceTransform.position;
        pieceOriginRotation = selectedPieceTransform.rotation;
        cameraOrigin = CameraTransform.position;

        selectedPieceTransform.position = PieceGluingPosition.position;
        Renderer pieceRenderer = selectedPieceTransform.gameObject.GetComponent<Renderer>();
        float zDistance = pieceRenderer.bounds.extents.magnitude * 2f;
        CameraTransform.position = new Vector3(PieceGluingPosition.position.x, PieceGluingPosition.position.y, PieceGluingPosition.position.z - zDistance);
    }

    private void DeactivateGluing()
    {
        playerGlue.enabled = false;
        selectedPieceTransform.position = pieceOrigin;
        selectedPieceTransform.rotation = pieceOriginRotation;
        CameraTransform.position = cameraOrigin;

        selectedPieceTransform = null;
    }


    private void EnableTouchEvents()
    {
        EasyTouch.On_DoubleTap += OnDoubleTap;
        EasyTouch.On_PinchIn += OnPinchIn;
    }

    private void DisableTouchEvents()
    {
        EasyTouch.On_DoubleTap -= OnDoubleTap;
        EasyTouch.On_PinchIn -= OnPinchIn;       
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

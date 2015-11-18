using UnityEngine;
using System.Collections;

public class GlueManager : MonoBehaviour 
{
    public Transform PiecePosition;
    public GameObject GameCamera;
    public PlayerGlue PlayerGlue;

    private Transform pieceTransform;
    private Vector3 pieceOrigin = Vector3.zero;
    private Quaternion pieceOriginRotation = Quaternion.identity;
    private PieceController pieceController;

    private Vector3 cameraOrigin = Vector3.zero;
    private Transform cameraTransform;
    private GameCameraControl cameraControl;
    private Transform cameraLookAt;

    void Start()
    {
        PlayerGlue.enabled = false;
        cameraTransform = GameCamera.transform;
        cameraControl = GameCamera.GetComponent<GameCameraControl>();
    }

    void LateUpdate()
    {
        if (PlayerGlue.enabled)
        {
            cameraTransform.LookAt(cameraLookAt);
        }
    }

    //Replace with method that starts up gluing
    //Called by minigame game manager, which is called by a UI button
    public void OnDoubleTap(Gesture gesture) 
    {
        if (gesture.pickedObject != null)
        {
            if (!PlayerGlue.enabled)
            {
                if (gesture.pickedObject.tag == "Piece")
                {
                    pieceTransform = gesture.pickedObject.transform;
                    pieceController = gesture.pickedObject.GetComponent<PieceController>();
                    ActivateGluing();
                }
            }
            else
            {
                if (gesture.pickedObject.tag == "GlueHitBox")
                {
                    
                }
            }
        }
    }

    public void OnPinchIn(Gesture gesture)
    {
        if (PlayerGlue.enabled)
        {
            DeactivateGluing();
        }
    }

    private void ActivateGluing()
    {
        PlayerGlue.enabled = true;
        pieceOrigin = pieceTransform.position;
        pieceOriginRotation = pieceTransform.rotation;
        pieceController.state = PieceControlState.Rotate;

        cameraControl.enabled = false;
        cameraOrigin = cameraTransform.position;
        cameraLookAt = PiecePosition;

        pieceTransform.position = PiecePosition.position;
        Renderer pieceRenderer = pieceTransform.gameObject.GetComponent<Renderer>();
        float zDistance = pieceRenderer.bounds.extents.magnitude * 2f;
        cameraTransform.position = new Vector3(PiecePosition.position.x, PiecePosition.position.y, PiecePosition.position.z - zDistance);
    }

    private void DeactivateGluing()
    {
        PlayerGlue.enabled = false;
        pieceTransform.position = pieceOrigin;
        pieceTransform.rotation = pieceOriginRotation;

        cameraTransform.position = cameraOrigin;
        cameraControl.enabled = true;

        pieceTransform = null;
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

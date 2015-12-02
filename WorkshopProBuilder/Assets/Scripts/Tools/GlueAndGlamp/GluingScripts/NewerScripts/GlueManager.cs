using UnityEngine;
using System.Collections;

public class GlueManager : MonoBehaviour 
{
    public Transform PiecePosition;
    public GameObject GameCamera;
    public PlayerGlue PlayerGlue;
    public SnapPieceGameManager gameManager;

    private Transform pieceTransform;

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

    //Called by main game manager, which is called by the "Apply Glue" button
    //Setups the passed in piece
    public void ActivateGluing(GameObject pieceToGlue)
    {
        pieceToGlue.SetActive(true);
        pieceTransform = pieceToGlue.transform;
        PlayerGlue.enabled = true;
        SetupScene();
    }

    public void SwitchPiece(GameObject newPieceToGlue)
    {
        pieceTransform = newPieceToGlue.transform;

        SetupScene();
    }

    //Called by main game manager, which is called by the "Connect Pieces" button
    public void DeactivateGluing()
    {
        PlayerGlue.enabled = false;
        cameraTransform.position = cameraOrigin;
        cameraControl.enabled = true;
        pieceTransform = null;
    }

    private void SetupScene()
    {
        pieceTransform.gameObject.GetComponent<PieceController>().state = PieceControlState.Rotate;
        pieceTransform.position = PiecePosition.position;
        Renderer pieceRenderer = pieceTransform.gameObject.GetComponent<Renderer>();
        float zDistance = pieceRenderer.bounds.extents.magnitude * 2f;
        cameraTransform.position = new Vector3(PiecePosition.position.x, PiecePosition.position.y, PiecePosition.position.z - zDistance);
    }



    //public void OnDoubleTap(Gesture gesture) 
    //{
    //    if (gesture.pickedObject != null)
    //    {
    //        if (!PlayerGlue.enabled)
    //        {
    //            if (gesture.pickedObject.tag == "Piece")
    //            {
    //                SetupGluing(gesture.pickedObject);
    //            }
    //        }
    //        else
    //        {
    //            if (gesture.pickedObject.tag == "GlueHitBox")
    //            {

    //            }
    //        }
    //    }
    //}

    //private void EnableTouchEvents()
    //{
    //    EasyTouch.On_DoubleTap += OnDoubleTap;
    //}

    //private void DisableTouchEvents()
    //{
    //    EasyTouch.On_DoubleTap -= OnDoubleTap;
    //}

    //void OnEnable()
    //{
    //    EnableTouchEvents();
    //}
    //void OnDisable()
    //{
    //    DisableTouchEvents();
    //}
    //void OnDestroy()
    //{
    //    DisableTouchEvents();
    //}
}

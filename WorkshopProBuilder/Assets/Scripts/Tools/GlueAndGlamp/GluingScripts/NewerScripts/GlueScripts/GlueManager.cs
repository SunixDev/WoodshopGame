using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GlueManager : MonoBehaviour 
{
    public GameObject GameCamera;
    public Transform CameraPosition;
    public PlayerGlue glue;

    private List<GameObject> GluingPieces = new List<GameObject>();
    private Transform selectedPieceTransform;
    private Vector3 previousPositionOfSelectedPiece;
    private Vector3 cameraOrigin;
    private Transform cameraTransform;
    private OrbitCamera cameraControl;

    void Start()
    {
        glue.enabled = false;
        selectedPieceTransform = null;
        previousPositionOfSelectedPiece = Vector3.zero;
        cameraOrigin = Vector3.zero;
        cameraTransform = GameCamera.transform;
        cameraControl = GameCamera.GetComponent<OrbitCamera>();
    }

    void LateUpdate()
    {
        if (glue.enabled)
        {
            cameraTransform.LookAt(selectedPieceTransform);
        }
    }

    private void SetupScene()
    {
        cameraTransform.position = CameraPosition.position;
        float zDistance = 5f;
        if (selectedPieceTransform.gameObject.tag == "Piece")
        {
            selectedPieceTransform.gameObject.GetComponent<PieceController>().state = PieceControlState.Rotate;
            Renderer pieceRenderer = selectedPieceTransform.gameObject.GetComponent<Renderer>();
            zDistance = pieceRenderer.bounds.extents.magnitude * 2f;
        }
        else if (selectedPieceTransform.gameObject.tag == "WoodProject")
        {
            Bounds projectBounds = selectedPieceTransform.gameObject.GetComponent<WoodProject>().GetBounds();
            zDistance = projectBounds.extents.magnitude * 2f;
        }
        selectedPieceTransform.position = new Vector3(CameraPosition.position.x, CameraPosition.position.y, CameraPosition.position.z + zDistance);
    }

    //Called by main game manager, which is called by the "Apply Glue" button
    //Setups the passed in piece
    public void ActivateGluing(GameObject pieceToGlue)
    {
        glue.enabled = true;
        selectedPieceTransform = pieceToGlue.transform;
        cameraControl.enabled = false;
        SetupScene();
    }

    //Called by main game manager, which is called by the "Connect Pieces" button
    public void DeactivateGluing()
    {
        glue.enabled = false;
        cameraTransform.position = cameraOrigin;
        cameraControl.enabled = true;
        selectedPieceTransform = null;
    }

    public void AddGluingPiece(GameObject pieceToGlue)
    {
        GluingPieces.Add(pieceToGlue);
    }

    public void SwitchPiece(GameObject newPieceToGlue)
    {
        selectedPieceTransform = newPieceToGlue.transform;
        SetupScene();
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

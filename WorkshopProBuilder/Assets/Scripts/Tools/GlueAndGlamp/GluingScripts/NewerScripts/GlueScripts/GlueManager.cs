using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GlueManager : MonoBehaviour 
{
    public GameObject GameCamera;
    public Transform CameraPosition;
    public PlayerGlue glue;

    private List<GameObject> GluingPieces;
    private int selectedPieceIndex;
    private Vector3 previousSelectedPiecePosition;
    private Vector3 cameraOrigin;
    private Transform cameraTransform;
    private OrbitCamera cameraControl;

    void Awake()
    {
        GluingPieces = new List<GameObject>();
        glue.enabled = false;
        selectedPieceIndex = 0;
        previousSelectedPiecePosition = Vector3.zero;
        cameraOrigin = Vector3.zero;
        cameraTransform = GameCamera.transform;
        cameraControl = GameCamera.GetComponent<OrbitCamera>();
    }

    //void LateUpdate()
    //{
    //    if (glue.enabled)
    //    {
            
    //    }
    //}

    private void SetupScene()
    {
        cameraTransform.position = CameraPosition.position;
        float zDistance = 5f;
        if (GluingPieces[selectedPieceIndex].tag == "Piece")
        {
            GluingPieces[selectedPieceIndex].GetComponent<PieceController>().state = PieceControlState.Rotate;
            Renderer pieceRenderer = GluingPieces[selectedPieceIndex].GetComponent<Renderer>();
            zDistance = pieceRenderer.bounds.extents.magnitude * 2f;
        }
        else if (GluingPieces[selectedPieceIndex].tag == "WoodProject")
        {
            Bounds projectBounds = GluingPieces[selectedPieceIndex].GetComponent<WoodProject>().GetBounds();
            zDistance = projectBounds.extents.magnitude * 2f;
        }
        GluingPieces[selectedPieceIndex].transform.position = new Vector3(CameraPosition.position.x, CameraPosition.position.y, CameraPosition.position.z + zDistance);
    }

    //Called by main game manager, which is called by the "Apply Glue" button
    //Setups the passed in piece
    public void ActivateGluing()
    {
        glue.enabled = true;
        cameraControl.ChangeAngle(0f, 0f);
        cameraControl.LookAtPoint = GluingPieces[selectedPieceIndex].transform;
        previousSelectedPiecePosition = GluingPieces[selectedPieceIndex].transform.position;
        SetupScene();
    }

    //Called by main game manager, which is called by the "Connect Pieces" button
    public void DisableGluing()
    {
        glue.enabled = false;
        cameraTransform.position = cameraOrigin;
        cameraControl.enabled = true;
        GluingPieces[selectedPieceIndex].transform.position = previousSelectedPiecePosition;
    }

    public void AddGluingPiece(GameObject pieceToGlue)
    {
        GluingPieces.Add(pieceToGlue);
    }

    public void SwitchPiece(int newPieceToGlue)
    {
        selectedPieceIndex = newPieceToGlue;
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

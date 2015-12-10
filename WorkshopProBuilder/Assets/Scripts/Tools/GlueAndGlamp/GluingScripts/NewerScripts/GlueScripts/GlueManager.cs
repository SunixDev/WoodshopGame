using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GlueManager : MonoBehaviour 
{
    public GameObject GameCamera;
    public Transform CameraStartingPosition;
    public PlayerGlue glue;
    public GlueButtonContainer ButtonContainer;

    private List<GameObject> GluingPieces;
    private int selectedPieceIndex;
    private Vector3 selectedPiecePreviousPosition;
    private Transform cameraTransform;
    private OrbitCamera cameraControl;
    private float distancePadding;

    void Awake()
    {
        GluingPieces = new List<GameObject>();
        glue.enabled = false;
        selectedPieceIndex = 0;
        selectedPiecePreviousPosition = Vector3.zero;
        cameraTransform = GameCamera.transform;
        cameraControl = GameCamera.GetComponent<OrbitCamera>();
        distancePadding = 2.2f;
    }

    private void SetupScene()
    {
        cameraTransform.position = CameraStartingPosition.position;
        float zDistance = 5f;
        if (GluingPieces[selectedPieceIndex].tag == "Piece")
        {
            GluingPieces[selectedPieceIndex].GetComponent<PieceController>().state = PieceControlState.Rotate;
            Renderer pieceRenderer = GluingPieces[selectedPieceIndex].GetComponent<Renderer>();
            zDistance = pieceRenderer.bounds.extents.magnitude * distancePadding;
        }
        else if (GluingPieces[selectedPieceIndex].tag == "WoodProject")
        {
            GluedPieceController controller = GluingPieces[selectedPieceIndex].GetComponent<GluedPieceController>();
            controller.RotateX_Axis = true;
            controller.RotateY_Axis = true;
            Bounds projectBounds = GluingPieces[selectedPieceIndex].GetComponent<WoodProject>().GetBounds();
            zDistance = projectBounds.extents.magnitude * distancePadding;
        }
        GluingPieces[selectedPieceIndex].transform.position = new Vector3(CameraStartingPosition.position.x, CameraStartingPosition.position.y, CameraStartingPosition.position.z + zDistance);
        GluingPieces[selectedPieceIndex].transform.rotation = Quaternion.identity;
        cameraControl.Distance = zDistance;
    }

    public void ActivateGluing()
    {
        glue.enabled = true;
        SetUpPiece(selectedPieceIndex);
        cameraControl.ChangeAngle(0f, 0f);
        cameraControl.MinDistance = 0.2f;
        cameraControl.LookAtPoint = GluingPieces[selectedPieceIndex].transform;
        SetupScene();
    }

    public void DisableGluing()
    {
        glue.enabled = false;
        cameraControl.MinDistance = 0.5f;
        ReturnSelectedPiece();
    }

    public void AddGluingPiece(GameObject pieceToGlue)
    {
        GluingPieces.Add(pieceToGlue);
    }

    public void SwitchPiece(int pieceIndex)
    {
        ReturnSelectedPiece();
        SetUpPiece(pieceIndex);
        selectedPieceIndex = pieceIndex;
        cameraControl.LookAtPoint = GluingPieces[selectedPieceIndex].transform;
        SetupScene();
    }

    private void ReturnSelectedPiece()
    {
        GluingPieces[selectedPieceIndex].transform.position = selectedPiecePreviousPosition;
        GluingPieces[selectedPieceIndex].transform.rotation = Quaternion.identity;
        if (GluingPieces[selectedPieceIndex].tag == "Piece")
        {
            GluingPieces[selectedPieceIndex].SetActive(false);
        }
        if (GluingPieces[selectedPieceIndex].tag == "WoodProject")
        {
            GluedPieceController controller = GluingPieces[selectedPieceIndex].GetComponent<GluedPieceController>();
            controller.RotateX_Axis = false;
            controller.RotateY_Axis = true;
        }
    }

    public void UpdateAvailablePieces(int pieceIndex)
    {
        GluingPieces[pieceIndex] = null;
        ButtonContainer.DisableButton(pieceIndex);
        ResetSelection();
    }

    public bool ContainsPiece(GameObject piece)
    {
        bool available = false;
        if (piece != null)
        {
            available = GluingPieces.Contains(piece);
        }
        return available;
    }

    public int IndexOfPiece(GameObject piece)
    {
        if (piece == null || !ContainsPiece(piece))
        {
            return -1;
        }

        return GluingPieces.IndexOf(piece);
    }

    private void SetUpPiece(int index)
    {
        GluingPieces[index].SetActive(true);
        selectedPiecePreviousPosition = GluingPieces[index].transform.position;
    }

    private void ResetSelection()
    {
        selectedPieceIndex = 0;
        while (GluingPieces[selectedPieceIndex] == null && selectedPieceIndex < GluingPieces.Count)
        {
            selectedPieceIndex++;
        }
        if (selectedPieceIndex < GluingPieces.Count)
        {
            ButtonContainer.SwitchSelectedButton(selectedPieceIndex);
        }
    }
}

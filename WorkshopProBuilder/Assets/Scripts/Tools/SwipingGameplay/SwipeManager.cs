using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor;

public class SwipeManager : MonoBehaviour 
{
    public List<GameObject> AvailablePieces;
    public Transform PieceSpawnPoint;
    public SwipeGameplay SwipeGameplayManager;
    public Material PaintingMaterial;
    public Material SandingMaterial;
    public Material LaqcuerMaterial;
    public SwipeUI UI_Manager;
    public string NextScene;
    public bool PieceRotationEnabled { get; set; }

    private int currentPieceIndex = 0;
    private List<TextureScanner> textureScans = new List<TextureScanner>();
    private bool swiping = true;
    private bool scanning = true;

    void Start()
    {
        PieceRotationEnabled = false;
        Material selectedMaterial = PaintingMaterial;
        if (SwipeGameplayManager.type == SwipeGameType.Sanding)
        {
            selectedMaterial = SandingMaterial;
        }
        else if (SwipeGameplayManager.type == SwipeGameType.Lacquer)
        {
            selectedMaterial = LaqcuerMaterial;
        }

        foreach (GameObject piece in AvailablePieces)
        {
            piece.SetActive(false);
            piece.GetComponent<WoodPiece>().EnableConvexCollider(false);
            piece.GetComponent<WoodPiece>().ChangeMaterial(selectedMaterial);
        }
        SwipeGameplayManager.CurrentPiece = AvailablePieces[currentPieceIndex];
        SwipeGameplayManager.CurrentPiece.SetActive(true);
        SwipeGameplayManager.Setup();
        PlacePiece();
    }

    public void SwitchToNextPiece()
    {
        SwitchPiece(currentPieceIndex + 1);
    }

    private void SwitchPiece(int indexToSwitchTo)
    {
        AvailablePieces[currentPieceIndex].transform.position = Vector3.zero;
        AvailablePieces[currentPieceIndex].SetActive(false);
        AvailablePieces[indexToSwitchTo].SetActive(true);
        AvailablePieces[indexToSwitchTo].GetComponent<WoodPiece>().EnableConvexCollider(false);
        currentPieceIndex = indexToSwitchTo;
        SwipeGameplayManager.CurrentPiece = AvailablePieces[currentPieceIndex];
        PlacePiece();
    }

    public void PieceCompleted()
    {
        Texture2D textureToAnalyze = SwipeGameplayManager.GetPaintedTexture();
        TextureScanner scan = gameObject.AddComponent<TextureScanner>();
        scan.texture = textureToAnalyze;
        scan.objMeshFilter = AvailablePieces[currentPieceIndex].GetComponent<MeshFilter>();
        scan.StartScanning();
        textureScans.Add(scan);

        SwipeGameplayManager.ResetSwipeBackgroundTexture();
        if (currentPieceIndex != AvailablePieces.Count - 1)
        {
            SwitchToNextPiece();
        }
        else
        {
            AvailablePieces[currentPieceIndex].transform.position = Vector3.zero;
            AvailablePieces[currentPieceIndex].SetActive(false);
            UI_Manager.DisplayMessagePanelWithText("Evaluating performance...");
            swiping = false;
        }
    }

    void Update()
    {
        if (!swiping && scanning)
        {
            bool scansComplete = true;
            for (int i = 0; i < textureScans.Count && scansComplete; i++)
            {
                scansComplete = textureScans[i].ScanIsComplete();
            }
            if (scansComplete)
            {
                //Get score from each scan for points
                UI_Manager.DisplayFullMessagePanel("All pieces are done. Go to the next step");
                scanning = false;
            }
        }
    }

    public void PlacePiece()
    {
        AvailablePieces[currentPieceIndex].transform.position = PieceSpawnPoint.position;
    }

    public void RotateObject(Gesture gesture)
    {
        if (PieceRotationEnabled && gesture.touchCount == 1)
        {
            AvailablePieces[currentPieceIndex].transform.Rotate(gesture.deltaPosition.y, -gesture.deltaPosition.x, 0.0f, Space.World);
        }
    }

    public void GoToNextScene()
    {
        Application.LoadLevel(NextScene);
    }

    void OnEnable()
    {
        EasyTouch.On_Drag += RotateObject;
    }

    void OnDisable()
    {
        EasyTouch.On_Drag -= RotateObject;
    }

    void OnDestory()
    {
        EasyTouch.On_Drag -= RotateObject;
    }
}



//string swipeType = "paint";
//if (SwipeGameplayManager.type == SwipeGameType.Sanding)
//{
//    swipeType = "sand";
//}
//else if (SwipeGameplayManager.type == SwipeGameType.Lacquer)
//{
//    swipeType = "shine";
//}
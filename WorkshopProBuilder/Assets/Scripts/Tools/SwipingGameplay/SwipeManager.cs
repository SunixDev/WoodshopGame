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
    public GameObject ColorPickerContainer;
    public DrawerSlide DrawerSlider;
    public bool PieceRotationEnabled { get; set; }

    private int currentPieceIndex = 0;
    private List<TextureScanner> textureScans = new List<TextureScanner>();
    private bool swiping = true;
    private bool scanning = true;

    void Start()
    {
        UI_Manager.DisplayPlans(true);
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
        if (DrawerSlider != null && ColorPickerContainer != null)
        {
            if (DrawerSlider.Hiding && ColorPickerContainer.activeInHierarchy)
            {
                ColorPickerContainer.SetActive(false);
            }
            else if (!DrawerSlider.Hiding && !ColorPickerContainer.activeInHierarchy)
            {
                ColorPickerContainer.SetActive(true);
            }
        }

        if (!swiping && scanning)
        {
            bool scansComplete = true;
            for (int i = 0; i < textureScans.Count && scansComplete; i++)
            {
                scansComplete = textureScans[i].ScanIsComplete();
            }
            if (scansComplete)
            {
                float totalScore = 0;
                for (int i = 0; i < textureScans.Count; i++)
                {
                    totalScore += textureScans[i].GetPercentageCorrect();
                }
                float overallScore = totalScore / textureScans.Count;
                if (GameManager.instance != null)
                {
                    GameManager.instance.ApplyScore(overallScore);
                }
                else
                {
                    Debug.Log("No game manager");
                }
                if (overallScore >= 90.0f)
                {
                    UI_Manager.DisplayFullMessagePanel("Excellent!\nYou work was detailed and thorough. The end result is well done.");
                }
                else if (overallScore < 90.0f && overallScore >= 80.0f)
                {
                    UI_Manager.DisplayFullMessagePanel("Good job!\nSome minor issues here and there, but nothing to be worried about");
                }
                else
                {
                    UI_Manager.DisplayFullMessagePanel("Good enough.\nLooks like you missed several spots. Be a bit more thorough next time.");
                }
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
        if (PieceRotationEnabled && gesture.touchCount == 1 && !gesture.IsOverUIElement())
        {
            AvailablePieces[currentPieceIndex].transform.Rotate(gesture.deltaPosition.y, -gesture.deltaPosition.x, 0.0f, Space.World);
        }
    }

    public void GoToNextScene(string nextScene)
    {
        if (GameManager.instance != null)
        {
            Application.LoadLevel(nextScene);
        }
        else
        {
            Debug.Log("Next Scene: " + nextScene);
        }
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
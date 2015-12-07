using UnityEngine;
using System.Collections;

public class SnapPieceUI : MainUI 
{
    [Header("Snap Piece UI")]
    public DragButtonContainer SnapPieceButtonList;
    public GlueButtonContainer GluingButtonList;

    private GameObject SnapPieceButtonsPanel;
    private GameObject GluingButtonsPanel;


    override public void Initialize()
    {
        base.Initialize();
        SnapPieceButtonsPanel = SnapPieceButtonList.gameObject;
        GluingButtonsPanel = GluingButtonList.gameObject;
    }

    public void CreateDragButton(GameObject pieceToSnap)
    {
        WoodPiece woodPiece = pieceToSnap.GetComponent<WoodPiece>();
        Sprite icon = woodPiece.ButtonIcon;
        SnapPieceButtonList.CreateButton(icon, pieceToSnap.name);
    }

    public void CreateGluingButton(GameObject pieceToGlue, GlueManager manager)
    {
        WoodPiece woodPiece = pieceToGlue.GetComponent<WoodPiece>();
        Sprite icon = woodPiece.ButtonIcon;
        GluingButtonList.CreateButton(icon, pieceToGlue.name, manager);
    }

    public void CreateGluingButton(WoodProject project, GlueManager manager)
    {
        GluingButtonList.CreateButton(null, project.name, manager);
    }

    public void DisplaySnapPieceButtonsPanel(bool display)
    {
        SnapPieceButtonsPanel.SetActive(display);
    }

    public void DisplayGlueButtonPanel(bool display)
    {
        GluingButtonsPanel.SetActive(display);
    }
}

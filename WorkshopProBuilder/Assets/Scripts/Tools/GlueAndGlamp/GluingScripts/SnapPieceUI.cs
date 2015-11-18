using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class SnapPieceUI : MainUI 
{
    public WoodListPanel ListPanel;

    public void EnableAllButtons()
    {
        base.EnableOptions();
        ListPanel.EnableButtons();
    }

    public void DisableAllButtons()
    {
        base.DisableOptions();
        ListPanel.DisableButtons();
    }
}

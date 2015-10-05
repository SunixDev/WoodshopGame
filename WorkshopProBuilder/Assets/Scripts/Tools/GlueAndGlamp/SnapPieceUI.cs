﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class SnapPieceUI : MonoBehaviour 
{
    public Color SelectedButtonColor;
    public GameObject SelectedButton;
    public List<Button> OptionButtons;
    public WoodListPanel ListPanel;

    void Start()
    {
        SelectedButton.GetComponent<Image>().color = SelectedButtonColor;
        SelectedButton.GetComponent<Button>().enabled = false;
    }

    public void SwitchActiveToolButton(GameObject buttonToUse)
    {
        if (buttonToUse != SelectedButton)
        {
            SelectedButton.GetComponent<Image>().color = Color.white;
            SelectedButton.GetComponent<Button>().enabled = true;

            SelectedButton = buttonToUse;
            SelectedButton.GetComponent<Image>().color = SelectedButtonColor;
            SelectedButton.GetComponent<Button>().enabled = false;
        }
    }

    public void SwitchSelectedPieceButtons(int index)
    {
        ListPanel.SwitchSelectedButton(index);
    }

    public void EnableOptions()
    {
        foreach (Button button in OptionButtons)
        {
            button.interactable = true;
            button.GetComponent<Image>().color = Color.white;
        }
        SelectedButton.GetComponent<Button>().interactable = false;
    }

    public void DisableOptions()
    {
        foreach (Button button in OptionButtons)
        {
            button.interactable = false;
            button.GetComponent<Image>().color = Color.white;
        }
    }

    public void EnableAllButtons()
    {
        EnableOptions();
        ListPanel.EnableButtons();
    }

    public void DisableAllButtons()
    {
        DisableOptions();
        ListPanel.DisableButtons();
    }
}

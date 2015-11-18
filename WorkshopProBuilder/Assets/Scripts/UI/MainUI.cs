using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class MainUI : MonoBehaviour 
{
    public Color SelectedButtonColor;
    public GameObject SelectedButton;
    public List<Button> OptionButtons;

    public GameObject PlansPanel;
    public Button HideButton;

    public GameObject ResultsPanel;
    public Text ResultsText;
    public Button StartOverButton;
    public Button NextSceneButton;

    protected void Initialize()
    {
        SelectedButton.GetComponent<Image>().color = SelectedButtonColor;
        SelectedButton.GetComponent<Button>().enabled = false;
        PlansPanel.SetActive(false);
        ResultsPanel.SetActive(false);
    }

    public void DisplayPlans(bool showPlans)
    {
        PlansPanel.SetActive(showPlans);
    }

    public void DisplayResultsPanel(string textToDisplay, bool displayStartOverButton = false, bool displayNextSceneButton = false)
    {
        ResultsPanel.SetActive(true);
        StartOverButton.gameObject.SetActive(displayStartOverButton);
        NextSceneButton.gameObject.SetActive(displayNextSceneButton);
    }

    public void HideResultsPanel()
    {
        ResultsPanel.SetActive(true);
    }

    public void SwitchActiveButton(GameObject buttonToUse)
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

    protected void EnableOptions()
    {
        foreach (Button button in OptionButtons)
        {
            button.interactable = true;
            button.GetComponent<Image>().color = Color.white;
        }
        SelectedButton.GetComponent<Button>().interactable = false;
    }

    protected void DisableOptions()
    {
        foreach (Button button in OptionButtons)
        {
            button.interactable = false;
            button.GetComponent<Image>().color = Color.white;
        }
    }
}

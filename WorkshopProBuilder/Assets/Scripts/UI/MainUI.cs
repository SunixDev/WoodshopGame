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
    public Button HidePlansButton;

    public GameObject ResultsPanel;
    public Text ResultsText;
    public Button StartOverButton;
    public Button NextSceneButton;

    protected void Initialize()
    {
        if (OptionButtons.Count > 0)
        {
            SelectedButton.GetComponent<Image>().color = SelectedButtonColor;
            SelectedButton.GetComponent<Button>().enabled = false;
        }
        DisplayPlans(true);
        ResultsPanel.SetActive(false);
    }

    public void DisplayPlans(bool showPlans)
    {
        PlansPanel.SetActive(showPlans);
        ResultsPanel.SetActive(false);
    }

    public void DisplayResultsPanel(string textToDisplay, bool displayStartOverButton = false, bool displayNextSceneButton = false)
    {
        PlansPanel.SetActive(false);
        ResultsPanel.SetActive(true);
        if (StartOverButton != null)
        {
            StartOverButton.gameObject.SetActive(displayStartOverButton);
        }
        if (NextSceneButton != null)
        {
            NextSceneButton.gameObject.SetActive(displayNextSceneButton);
        }
    }

    public void HideResultsPanel()
    {
        ResultsPanel.SetActive(true);
    }

    public void SwitchActiveButton(GameObject buttonToUse)
    {
        if (buttonToUse != SelectedButton && OptionButtons.Count > 0)
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
        if (SelectedButton != null)
        {
            SelectedButton.GetComponent<Button>().interactable = false;
        }
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

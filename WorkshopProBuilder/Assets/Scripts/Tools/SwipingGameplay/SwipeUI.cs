using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class SwipeUI : MonoBehaviour 
{
    public Color SelectedButtonColor;
    public GameObject SelectedButton;
    public List<Button> OptionButtons;

    public Button NextButton;
    public Button PreviousButton;
    public Button CompletionButton;
    public GameObject MessagePanel;
    public Text MessagePanelText;
    public GameObject MessagePanelButton;
    public GameObject PlansPanel;

    void Awake()
    {
        SelectedButton.GetComponent<Image>().color = SelectedButtonColor;
        SelectedButton.GetComponent<Button>().enabled = false;
        MessagePanel.SetActive(false);
        PlansPanel.SetActive(false);
    }

    public void DisplayPlans(bool showPlans)
    {
        PlansPanel.SetActive(showPlans);
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

    public void UpdateSelectionButtons(int index, int totalWoodMaterial)
    {
        NextButton.interactable = (index < totalWoodMaterial - 1);
        PreviousButton.interactable = (index > 0);
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
        NextButton.interactable = true;
        PreviousButton.interactable = true;
        CompletionButton.interactable = true;
    }

    public void DisableAllButtons()
    {
        DisableOptions();
        NextButton.interactable = false;
        PreviousButton.interactable = false;
        CompletionButton.interactable = false;
    }

    public void DisplayMessagePanelWithText(string textInPanel)
    {
        MessagePanel.SetActive(true);
        MessagePanelText.text = textInPanel;
        MessagePanelButton.SetActive(false);
    }

    public void DisplayFullMessagePanel(string textInPanel)
    {
        MessagePanel.SetActive(true);
        MessagePanelText.text = textInPanel;
        MessagePanelButton.SetActive(true);
    }
}

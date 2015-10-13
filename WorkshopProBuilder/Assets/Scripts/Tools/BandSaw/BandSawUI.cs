using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BandSawUI : MonoBehaviour 
{
    public Button ReviewPlansButton;
    public Button StartSawButton;
    public Button StopSawButton;
    public GameObject PlansPanel;
    public GameObject InfoPanel;
    public Text InfoText;
    public Button HideButton;
    public Button StartOverButton;
    public Button NextSceneButton;


    private bool SawButtonsActive = true;

    void Awake()
    {
        PlansPanel.SetActive(false);
    }

    public void DisplayPlans(bool showPlans)
    {
        PlansPanel.SetActive(showPlans);
    }

    public void DisplayInfo(bool showPlans)
    {
        InfoPanel.SetActive(showPlans);
    }

    public void ChangeSawButtons(bool bladeIsActive)
    {
        if (SawButtonsActive)
        {
            if (bladeIsActive)
            {
                StartSawButton.interactable = false;
                StopSawButton.interactable = true;
            }
            else
            {
                StartSawButton.interactable = true;
                StopSawButton.interactable = false;
            }
        }
    }

    public void EnableAllButtons()
    {
        ReviewPlansButton.interactable = true;
        StartSawButton.interactable = true;
        StopSawButton.interactable = true;
    }

    public void DisableAllButtons()
    {
        ReviewPlansButton.interactable = false;
        StartSawButton.interactable = false;
        StopSawButton.interactable = false;
    }
}

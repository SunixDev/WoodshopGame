using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BandSawUI : MonoBehaviour 
{
    public Button ReviewPlansButton;
    public Button StartSawButton;
    public Button StopSawButton;
    public GameObject PlansPanel;

    private bool SawButtonsActive = true;

    void Start()
    {
        PlansPanel.SetActive(false);
    }

    public void DisplayPlans(bool showPlans)
    {
        PlansPanel.SetActive(showPlans);
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

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BandSawUI : MonoBehaviour 
{
    public Button ReviewPlansButton;
    public Button NextButton;
    public Button PreviousButton;
    public Button StartSawButton;
    public Button StopSawButton;

    void Start()
    {

    }

    public void ChangeSawButtons(bool bladeActive)
    {
        if (bladeActive)
        {
            StartSawButton.gameObject.SetActive(false);
            StopSawButton.gameObject.SetActive(true);
        }
        else
        {
            StartSawButton.gameObject.SetActive(true);
            StopSawButton.gameObject.SetActive(false);
        }
    }

    public void UpdateSelectionButtons(int index, int totalWoodMaterial)
    {
        NextButton.interactable = (index < totalWoodMaterial - 1);
        PreviousButton.interactable = (index > 0);
    }

    public void EnableAllButtons()
    {
        ReviewPlansButton.interactable = true;
        NextButton.interactable = true;
        PreviousButton.interactable = true;
        StartSawButton.interactable = true;
        StopSawButton.interactable = true;
    }

    public void DisableAllButtons()
    {
        ReviewPlansButton.interactable = false;
        NextButton.interactable = false;
        PreviousButton.interactable = false;
        StartSawButton.interactable = false;
        StopSawButton.interactable = false;
    }
}

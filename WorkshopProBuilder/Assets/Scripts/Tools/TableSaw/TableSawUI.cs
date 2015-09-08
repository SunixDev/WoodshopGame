using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class TableSawUI : MonoBehaviour 
{
    public CameraControl camera;
    public Color SelectedButtonColor;
    public GameObject SelectedButton;
    public List<Button> OptionButtons;

    void Start()
    {
        SelectedButton.GetComponent<Image>().color = SelectedButtonColor;
        SelectedButton.GetComponent<Button>().enabled = false;
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

    public void EnableOptions()
    {
        foreach (Button button in OptionButtons)
        {
            button.enabled = true;
        }
        SelectedButton.GetComponent<Button>().enabled = false;
    }

    public void DisableOptions()
    {
        foreach (Button button in OptionButtons)
        {
            button.enabled = false;
        }
    }
}

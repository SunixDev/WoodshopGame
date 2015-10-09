using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;

public class WoodListPanel : MonoBehaviour 
{
    public GameObject WoodPieceButton;
    public GameObject ButtonContainer;
    public Color SelectedButtonColor;
    public int SelectedButtonIndex { get; set; }

    private List<GameObject> AvailableButtonsList = new List<GameObject>();

    public void AddWoodMaterialButton(string woodName, SnapPieceGameManager manager)
    {
        GameObject button = Instantiate(WoodPieceButton) as GameObject;
        button.transform.SetParent(ButtonContainer.transform);
        button.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);

        Button newButton = button.GetComponent<Button>();
        newButton.GetComponentInChildren<Text>().text = woodName;
        newButton.GetComponent<Image>().color = Color.white;

        int index = AvailableButtonsList.Count;
        newButton.onClick.AddListener(() => SwitchSelectedButton(index));
        //newButton.onClick.AddListener(() => manager.SwitchPiece(index));
        if (AvailableButtonsList.Count == 0)
        {
            newButton.GetComponent<Image>().color = SelectedButtonColor;
            newButton.GetComponent<Button>().enabled = false;
            SelectedButtonIndex = 0;
        }
        AvailableButtonsList.Add(button);
    }

    public void RemoveButton(int index)
    {
        if (index == SelectedButtonIndex)
        {
            if (index != 0)
            {
                SwitchSelectedButton(index - 1);
            }
            else if (index == 0 && AvailableButtonsList.Count > 1)
            {
                SwitchSelectedButton(AvailableButtonsList.Count - 1);
            }
            else if (index == 0 && AvailableButtonsList.Count == 1)
            {
                SelectedButtonIndex = -1;
            }
        }
        AvailableButtonsList.RemoveAt(index);
        GameObject buttonToRemove = ButtonContainer.transform.GetChild(index).gameObject;
        Destroy(buttonToRemove);
    }

    public void SwitchSelectedButton(int indexToUse)
    {
        if (indexToUse >= 0 && indexToUse < AvailableButtonsList.Count && indexToUse != SelectedButtonIndex)
        {
            if (SelectedButtonIndex > -1)
            {
                AvailableButtonsList[SelectedButtonIndex].GetComponent<Image>().color = Color.white;
                AvailableButtonsList[SelectedButtonIndex].GetComponent<Button>().enabled = true;
            }

            AvailableButtonsList[indexToUse].GetComponent<Image>().color = SelectedButtonColor;
            AvailableButtonsList[indexToUse].GetComponent<Button>().enabled = false;
            SelectedButtonIndex = indexToUse;
        }
    }

    public void EnableButtons()
    {
        foreach (GameObject button in AvailableButtonsList)
        {
            button.GetComponent<Button>().interactable = true;
        }
    }

    public void DisableButtons()
    {
        foreach (GameObject button in AvailableButtonsList)
        {
            button.GetComponent<Button>().interactable = true;
        }
    }
}

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
    public Color ConnectedButtonColor;
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
        newButton.onClick.AddListener(() => manager.SwitchPiece(index));
        if (AvailableButtonsList.Count == 0)
        {
            newButton.GetComponent<Image>().color = SelectedButtonColor;
            newButton.GetComponent<Button>().interactable = false;
            SelectedButtonIndex = 0;
        }
        AvailableButtonsList.Add(button);
    }

    public void RemoveButton(int index)
    {
        AvailableButtonsList[index].GetComponent<Image>().color = ConnectedButtonColor;
        AvailableButtonsList[index].GetComponent<Button>().enabled = false;
        int nextIndex = index + 1;
        if (nextIndex == AvailableButtonsList.Count || !AvailableButtonsList[nextIndex].GetComponent<Button>().enabled)
        {
            bool found = false;
            for (int i = 0; i < AvailableButtonsList.Count && !found; i++)
            {
                if (AvailableButtonsList[i].GetComponent<Button>().enabled)
                {
                    found = true;
                    nextIndex = i;
                }
            }
        }
        AvailableButtonsList[nextIndex].GetComponent<Image>().color = SelectedButtonColor;
        AvailableButtonsList[nextIndex].GetComponent<Button>().interactable = false;
        SelectedButtonIndex = nextIndex;
    }

    public void SwitchSelectedButton(int indexToUse)
    {
        if (indexToUse >= 0 && indexToUse < AvailableButtonsList.Count && indexToUse != SelectedButtonIndex)
        {
            AvailableButtonsList[SelectedButtonIndex].GetComponent<Image>().color = Color.white;
            AvailableButtonsList[SelectedButtonIndex].GetComponent<Button>().interactable = true;

            AvailableButtonsList[indexToUse].GetComponent<Image>().color = SelectedButtonColor;
            AvailableButtonsList[indexToUse].GetComponent<Button>().interactable = false;
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
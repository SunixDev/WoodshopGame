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
    [HideInInspector]
    public int SelectedButtonIndex;

    private List<GameObject> AvailableButtonsList = new List<GameObject>();

    void Awake()
    {
        SelectedButtonIndex = 0;
    }

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
        AvailableButtonsList.Add(button);
    }

    public void DisableButton(int index)
    {
        AvailableButtonsList[index].GetComponent<Image>().color = ConnectedButtonColor;
        AvailableButtonsList[index].GetComponent<Button>().enabled = false;
        AvailableButtonsList.RemoveAt(index);
    }

    public void SwitchSelectedButton(int indexToUse)
    {
        if (AvailableButtonsList.Count > 0)
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
        else
        {
            Debug.Log("There are not buttons in the list");
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
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

    private int SelectedButtonIndex;
    private List<GameObject> AvailableButtonsList = new List<GameObject>();

    void Awake()
    {

    }

    public void AddWoodMaterialButton(string woodName)
    {
        GameObject button = Instantiate(WoodPieceButton) as GameObject;
        button.transform.SetParent(ButtonContainer.transform);
        button.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);

        Button newButton = button.GetComponent<Button>();
        newButton.GetComponentInChildren<Text>().text = woodName;
        newButton.GetComponent<Image>().color = Color.white;

        int index = AvailableButtonsList.Count;
        AvailableButtonsList.Add(button);
    }

    //public void DisableButton(int index)
    //{
    //    AvailableButtonsList[index].GetComponent<Image>().color = ConnectedButtonColor;
    //    AvailableButtonsList[index].GetComponent<Button>().enabled = false;
    //    AvailableButtonsList.RemoveAt(index);
    //}

    //public void SwitchSelectedButton(int indexToUse)
    //{
    //    if (AvailableButtonsList.Count > 0)
    //    {
    //        if (indexToUse >= 0 && indexToUse < AvailableButtonsList.Count && indexToUse != SelectedButtonIndex)
    //        {
    //            AvailableButtonsList[SelectedButtonIndex].GetComponent<Image>().color = Color.white;
    //            AvailableButtonsList[SelectedButtonIndex].GetComponent<Button>().interactable = true;

    //            AvailableButtonsList[indexToUse].GetComponent<Image>().color = SelectedButtonColor;
    //            AvailableButtonsList[indexToUse].GetComponent<Button>().interactable = false;
    //            SelectedButtonIndex = indexToUse;
    //        }
    //    }
    //    else
    //    {
    //        Debug.Log("There are not buttons in the list");
    //    }
    //}

    //public void EnableButtons()
    //{
    //    foreach (GameObject button in AvailableButtonsList)
    //    {
    //        button.GetComponent<Button>().interactable = true;
    //    }
    //}

    //public void DisableButtons()
    //{
    //    foreach (GameObject button in AvailableButtonsList)
    //    {
    //        button.GetComponent<Button>().interactable = true;
    //    }
    //}
}
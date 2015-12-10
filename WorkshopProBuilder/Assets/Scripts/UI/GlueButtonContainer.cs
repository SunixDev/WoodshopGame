using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GlueButtonContainer : ScrollingButtonContainer
{
    public Color SelectedButtonColor;

    private Color PreviousButtonColor;

    public void CreateButton(Sprite iconSprite, string pieceName, GlueManager manager)
    {
        GameObject button = base.CreateButton(iconSprite, pieceName);

        Button buttonComponent = button.GetComponent<Button>();
        if (buttonComponent != null)
        {
            int i = AvailableButtonsList.Count - 1;
            buttonComponent.onClick.AddListener(() => manager.SwitchPiece(i));
            buttonComponent.onClick.AddListener(() => SwitchSelectedButton(button));
        }

        if (SelectedButtonIndex == -1)
        {
            SwitchSelectedButton(button);
        }
    }

    public void SwitchSelectedButton(int indexToUse)
    {
        if (AvailableButtonsList.Count > 0)
        {
            if (SelectedButtonIndex == -1)
            {
                SelectedButtonIndex = indexToUse;
                PreviousButtonColor = AvailableButtonsList[SelectedButtonIndex].GetComponent<Image>().color;
                AvailableButtonsList[SelectedButtonIndex].GetComponent<Image>().color = SelectedButtonColor;
                AvailableButtonsList[SelectedButtonIndex].GetComponent<Button>().interactable = false;
            }
            else if (indexToUse >= 0 && indexToUse < AvailableButtonsList.Count)
            {
                if (AvailableButtonsList[SelectedButtonIndex] != null)
                {
                    AvailableButtonsList[SelectedButtonIndex].GetComponent<Image>().color = PreviousButtonColor;
                    AvailableButtonsList[SelectedButtonIndex].GetComponent<Button>().interactable = true;
                }

                PreviousButtonColor = AvailableButtonsList[indexToUse].GetComponent<Image>().color;
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

    public void SwitchSelectedButton(GameObject selectedButton)
    {
        if (AvailableButtonsList.Count > 0)
        {
            if (AvailableButtonsList.Contains(selectedButton))
            {
                int indexToUse = AvailableButtonsList.IndexOf(selectedButton);
                SwitchSelectedButton(indexToUse);
            }
            else
            {
                Debug.Log(this + " does not contain " + selectedButton + ".");
            }
        }
        else
        {
            Debug.Log("There are not buttons in the list");
        }
    }

    public void DisableButton(int index)
    {
        if (index >= 0 && index < AvailableButtonsList.Count)
        {
            if (SelectedButtonIndex == index)
            {
                AvailableButtonsList[SelectedButtonIndex].GetComponent<Image>().color = PreviousButtonColor;
            }
            AvailableButtonsList[index].GetComponent<Button>().interactable = false;
            AvailableButtonsList[index] = null;
        }
    }
}

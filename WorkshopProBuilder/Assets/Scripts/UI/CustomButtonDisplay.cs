using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CustomButtonDisplay : MonoBehaviour 
{
    public Image ButtonIconImageComponent;
    public Text ButtonTextComponent;

	void Awake () 
    {
        if (ButtonIconImageComponent == null)
        {
            Debug.LogError("Icon's image component is not set. - " + gameObject);
        }
        if (ButtonTextComponent == null)
        {
            Debug.LogError("Button's text component is not set. - " + gameObject);
        }
	}

    public void SetIconImage(Sprite icon)
    {
        if (icon)
        {
            ButtonIconImageComponent.sprite = icon;
        }
    }

    public void SetButtonText(string textToDisplay)
    {
        if (textToDisplay != null && textToDisplay.Length > 0)
        {
            ButtonTextComponent.text = textToDisplay;
        }
        else
        {
            ButtonTextComponent.text = "Piece Name Here";
        }
    }
}

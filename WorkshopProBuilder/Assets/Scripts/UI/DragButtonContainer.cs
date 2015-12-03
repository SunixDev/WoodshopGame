using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DragButtonContainer : ScrollingButtonContainer
{
    public Transform canvas;
    public GameObject dragIconObject;

    public void CreateButton(Sprite iconSprite, string pieceName)
    {
        GameObject button = base.CreateButton(iconSprite, pieceName);

        GameObject dragIcon = Instantiate(dragIconObject) as GameObject;
        dragIcon.name = pieceName + dragIconObject.name;
        if (canvas != null)
        {
            dragIcon.transform.SetParent(canvas);
            dragIcon.transform.SetAsLastSibling();
        }

        Image iconImage = dragIcon.GetComponent<Image>();
        iconImage.sprite = iconSprite;
        iconImage.color = new Color(1f, 1f, 1f, 1f);
        dragIcon.SetActive(false);

        UIDragButton dragButton = button.GetComponent<UIDragButton>();
        dragButton.SetElementToDrag(dragIcon);
        dragIcon.SetActive(false);
    }
}

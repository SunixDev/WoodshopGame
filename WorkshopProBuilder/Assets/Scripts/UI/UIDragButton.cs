using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Image))]
public class UIDragButton : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerDownHandler
{
    public GameObject ElementToDrag;
    public bool MoveToFront = false;
    public Color NeutralColor = new Color(1f, 1f, 1f, 1f);
    public Color ActiveColor = new Color(0.5f, 0.5f, 0.5f, 0.8f);

    private RectTransform objRectTransform;
    private Image objImage;
    private Color imageColor;
    private bool selected;

    void Awake()
    {
        if (ElementToDrag != null)
        {
            SetElementToDrag(ElementToDrag);
        }
        objImage = GetComponent<Image>();
        imageColor = objImage.color;
        objImage.color = imageColor * NeutralColor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        objImage.color = imageColor * ActiveColor;
        selected = true;
        if (ElementToDrag != null)
        {
            objRectTransform.position = eventData.position;
            if (MoveToFront)
            {
                Transform canvas = objRectTransform;
                while (canvas.gameObject.GetComponent<Canvas>() == null)
                {
                    canvas = canvas.parent;
                }
                objRectTransform.parent = canvas;
                objRectTransform.SetAsLastSibling();
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (ElementToDrag != null)
        {
            objRectTransform.position = eventData.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (selected)
        {
            objImage.color = imageColor * NeutralColor;
            selected = false;
        }
    }

    public void SetElementToDrag(GameObject element)
    {
        ElementToDrag = element;
        objRectTransform = ElementToDrag.GetComponent<RectTransform>();
        objImage = ElementToDrag.GetComponent<Image>();
    }
}

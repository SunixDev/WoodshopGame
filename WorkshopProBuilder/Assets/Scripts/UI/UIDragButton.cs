using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Image))]
public class UIDragButton : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public GameObject ElementToDrag;
    public bool MoveToFront = false;
    public Color NeutralColor = new Color(1f, 1f, 1f, 1f);
    public Color ActiveColor = new Color(0.5f, 0.5f, 0.5f, 0.8f);

    private RectTransform objRectTransform;
    private Image buttonImage;
    private Color imageColor;
    private bool selected;

    void Awake()
    {
        if (ElementToDrag != null)
        {
            SetElementToDrag(ElementToDrag);
        }
        buttonImage = GetComponent<Image>();
        imageColor = buttonImage.color;
        buttonImage.color = imageColor * NeutralColor;
    }

    public void BeginDraggingIcon(Vector2 pointerPosition)
    {
        buttonImage.color = imageColor * ActiveColor;
        selected = true;
        ElementToDrag.SetActive(true);
        if (ElementToDrag != null)
        {
            objRectTransform.position = pointerPosition;
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

    public void DragIcon(Vector2 pointerPosition)
    {
        if (ElementToDrag != null)
        {
            objRectTransform.position = pointerPosition;
        }
    }

    public void StopDraggingIcon()
    {
        if (selected)
        {
            buttonImage.color = imageColor * NeutralColor;
            selected = false;
            ElementToDrag.SetActive(false);
        }
    }

    public void SetElementToDrag(GameObject element)
    {
        ElementToDrag = element;
        objRectTransform = ElementToDrag.GetComponent<RectTransform>();
        ElementToDrag.SetActive(false);
    }



    public void OnPointerDown(PointerEventData eventData)
    {
        BeginDraggingIcon(eventData.position);
    }

    public void OnDrag(PointerEventData eventData)
    {
        DragIcon(eventData.position);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        StopDraggingIcon();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        StopDraggingIcon();
    }
}

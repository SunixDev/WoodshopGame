using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIDragFromButton : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    public GameObject ElementToDrag;
    public bool MoveToFront = false;

    private RectTransform objRectTransform;
    private Image objImage;

    void Awake()
    {
        if (ElementToDrag != null)
        {
            objRectTransform = ElementToDrag.GetComponent<RectTransform>();
            objImage = ElementToDrag.GetComponent<Image>();
        }
        else
        {
            objRectTransform = GetComponent<RectTransform>();
            objImage = GetComponent<Image>();
        }

        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        objRectTransform.position = eventData.position;
        if (MoveToFront)
        {
            Transform canvas = objRectTransform.parent;
            while (canvas.parent != null)
            {
                canvas = canvas.parent;
            }
            objRectTransform.parent = canvas;
            objRectTransform.SetAsLastSibling();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        objRectTransform.position = eventData.position;
    }
}

using UnityEngine;
using System.Collections;

public class DrawerSlide : MonoBehaviour 
{
    public Vector3 VisibleAnchoredPosition;
    public Vector3 HiddenAnchoredPosition;
    public float SlideSpeed = 10.0f;
    public float DestinationOffset = 0.1f;

    private bool needToHide;
    private bool stillSliding;
    private RectTransform rect;

	void Start () 
    {
        needToHide = true;
        stillSliding = false;
        rect = GetComponent<RectTransform>();
        SnapToHidden();
	}
	
	void Update () 
    {
        Vector3 destination = rect.anchoredPosition3D;
        if (needToHide && stillSliding)
        {
            destination = HiddenAnchoredPosition;
            rect.anchoredPosition3D = Vector3.Lerp(rect.anchoredPosition3D, HiddenAnchoredPosition, SlideSpeed * Time.deltaTime);
        }
        else if (!needToHide && stillSliding)
        {
            destination = VisibleAnchoredPosition;
            rect.anchoredPosition3D = Vector3.Lerp(rect.anchoredPosition3D, VisibleAnchoredPosition, SlideSpeed * Time.deltaTime);
        }

        if ((Vector3.Distance(rect.anchoredPosition3D, destination) <= DestinationOffset) && stillSliding)
        {
            stillSliding = false;
            rect.anchoredPosition3D = destination;
        }
	}

    public void SnapToVisible()
    {
        rect.anchoredPosition3D = VisibleAnchoredPosition;
    }

    public void SnapToHidden()
    {
        rect.anchoredPosition3D = HiddenAnchoredPosition;
    }

    public void SwitchVisibility()
    {
        needToHide = !needToHide;
        stillSliding = true;
    }
}

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UICustomPanelButton : MonoBehaviour 
{
    public Color NeutralColor = new Color(1f, 1f, 1f, 1f);
    public Color ActiveColor = new Color(0.5f, 0.5f, 0.5f, 0.8f);

    private Image objImage;
    private bool selected;

	// Use this for initialization
	void Awake () 
    {
        objImage = GetComponent<Image>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnPanelTouch(Gesture gesture)
    {
        if (gesture.pickedUIElement == gameObject && !selected)
        {
            objImage.color = ActiveColor;
            selected = true;
        }
    }

    public void Release(Gesture gesture)
    {
        if (selected)
        {
            objImage.color = NeutralColor;
            selected = false;
        }
    }

    private void EnableTouchEvents()
    {
        EasyTouch.On_TouchStart += OnPanelTouch;
        EasyTouch.On_TouchUp += Release;
    }

    private void DisableTouchEvents()
    {
        EasyTouch.On_TouchStart -= OnPanelTouch;
        EasyTouch.On_TouchUp -= Release;
    }

    void OnEnable()
    {
        EnableTouchEvents();
    }
    void OnDisable()
    {
        DisableTouchEvents();
    }
    void OnDestory()
    {
        DisableTouchEvents();
    }
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerClamping : MonoBehaviour 
{
    public GameObject ClampPrefab;
    public GameObject ClampIcon;
    public Color NormalIconColor = new Color(1f, 1f, 1f, 1f);
    public Color ActiveIconColor = new Color(0.5f, 0.5f, 0.5f, 0.8f);
    public LayerMask pickableLayers;

    private ClampPoint selectedPoint;
    private Image clampIconImage;
    private bool positioningClamp;
    private Vector2 currentFingerPosition;
    private ClampManager clampManager;
    private RaycastHit clampRayHit;

    void Start()
    {
        selectedPoint = null;
        clampIconImage = ClampIcon.GetComponent<Image>();
        positioningClamp = false;
        currentFingerPosition = Vector2.zero;
        clampManager = GetComponent<ClampManager>();
    }

    void Update()
    {
        if (positioningClamp)
        {
            DetectClampPoint();
        }
    }

    private void DetectClampPoint()
    {
        Ray clampRay = Camera.main.ScreenPointToRay(currentFingerPosition);
        RaycastHit hit;
        if (Physics.Raycast(clampRay, out hit, 100f, pickableLayers))
        {
            if (hit.collider.gameObject.tag == "ClampPoint")
            {
                selectedPoint = hit.collider.gameObject.GetComponent<ClampPoint>();
            }
            else
            {
                selectedPoint = null;
            }
        }
        else
        {
            selectedPoint = null;
        }
    }

    private void AttachClamp()
    {
        GameObject clamp = Instantiate(ClampPrefab);
        clamp.GetComponent<Clamp>().ClampAt(selectedPoint);
        //send selectedPoint to gameManager
        selectedPoint = null;
    }



    #region Event-Based Methods
    public void OnTouch(Gesture gesture)
    {
        if (gesture.pickedUIElement == ClampIcon)
        {
            positioningClamp = true;
            clampIconImage.color = ActiveIconColor;
        }
    }

    public void UpdateTouchPosition(Gesture gesture)
    {
        if (positioningClamp)
        {
            currentFingerPosition = gesture.position;
        }
    }

    public void Release(Gesture gesture)
    {
        positioningClamp = false;
        clampIconImage.color = NormalIconColor;
        //Hide moveable icon by disabling game object
        if (selectedPoint != null)
        {
            AttachClamp();
        }
        else
        {
            //Display message to user where to add clamps?
            Debug.Log("No Clamp Attached");
        }
    }
    #endregion



    private void EnableTouchEvents()
    {
        EasyTouch.On_TouchStart += OnTouch;
        EasyTouch.On_TouchDown += UpdateTouchPosition;
        EasyTouch.On_TouchUp += Release;
    }

    private void DisableTouchEvents()
    {
        EasyTouch.On_TouchStart -= OnTouch;
        EasyTouch.On_TouchDown -= UpdateTouchPosition;
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

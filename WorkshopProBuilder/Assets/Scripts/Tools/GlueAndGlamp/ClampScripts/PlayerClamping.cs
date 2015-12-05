using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerClamping : MonoBehaviour
{
    public GameObject ClampPrefab;
    public GameObject ClampIcon;

    [Header("Draggable UI Clamp Icon")]
    public GameObject MovingClampIcon;
    public Color NeutralMovingIconColor = new Color(1f, 0.2f, 0.2f, 1f);
    public Color OverClampPointColor = new Color(1f, 1f, 1f, 1f);

    [Header("Layers Detected by Clamp Icon")]
    public LayerMask pickableLayers;

    private ClampPoint selectedPoint;
    //private Image clampIconImage;
    private Image clampMovingIconImage;
    private bool positioningClamp;
    private Vector2 currentFingerPosition;
    private ClampManager clampManager;

    void Start()
    {
        selectedPoint = null;
        //clampIconImage = ClampIcon.GetComponent<Image>();
        clampMovingIconImage = MovingClampIcon.GetComponent<Image>();
        MovingClampIcon.SetActive(false);
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
            ClampPoint possiblePoint = hit.collider.gameObject.GetComponent<ClampPoint>();
            if (hit.collider.gameObject.tag == "ClampPoint" && !possiblePoint.Clamped)
            {
                selectedPoint = hit.collider.gameObject.GetComponent<ClampPoint>();
                clampMovingIconImage.color = OverClampPointColor;
            }
            else
            {
                selectedPoint = null;
                clampMovingIconImage.color = NeutralMovingIconColor;
            }
        }
        else
        {
            if (selectedPoint != null)
            {
                selectedPoint = null;
                clampMovingIconImage.color = NeutralMovingIconColor;
            }
        }
    }

    private void AttachClamp()
    {
        GameObject clamp = Instantiate(ClampPrefab);
        clamp.GetComponent<Clamp>().ClampAt(selectedPoint);
        clampManager.UpdateClampedPoints(selectedPoint);
        selectedPoint = null;
    }



    #region Event-Based Methods
    public void OnTouch(Gesture gesture)
    {
        if (gesture.pickedUIElement == ClampIcon)
        {
            positioningClamp = true;
            MovingClampIcon.SetActive(true);
            clampMovingIconImage.color = NeutralMovingIconColor;
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
        if (positioningClamp)
        {
            positioningClamp = false;
            MovingClampIcon.SetActive(false);
            if (selectedPoint != null)
            {
                AttachClamp();
            }
            else
            {
                //Display message to user where to add clamps?
            }
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

﻿using UnityEngine;
using System.Collections;

public class PlayerGlue : MonoBehaviour 
{
    public float ApplicationRate = 20f;
    public float AmountToActivateSnapPoints = 75f;
    public float MinAmountForPerfectScore = 90f;
    public float MaxAmount = 100f;
    public LayerMask pickableLayers;

    private Vector2 currentTouchPosition = new Vector2(-1f, -1f);
    private Vector2 previousTouchPosition = new Vector2(-1f, -1f);
    private bool applyingGlue = false;

    void Update()
    {
        if (previousTouchPosition != currentTouchPosition && applyingGlue)
        {
            DetectGluingAreas();
        }
    }

    private void DetectGluingAreas()
    {
        Ray ray = Camera.main.ScreenPointToRay(currentTouchPosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f, pickableLayers))
        {
            GlueBox glueBox = hit.collider.gameObject.GetComponent<GlueBox>();
            if (glueBox != null)
            {
                glueBox.ApplyGlue(this);
            }
        }
        previousTouchPosition = currentTouchPosition;
    }



    public void GetTouchPosition(Gesture gesture)
    {
        if (gesture.pickedObject != null && gesture.touchCount == 1 && !gesture.isOverGui && !gesture.IsOverUIElement())
        {
            if (gesture.pickedObject.tag == "GlueHitBox")
            {
                currentTouchPosition = gesture.position;
                applyingGlue = true;
            }
        }
    }

    public void OnTouchRelease(Gesture gesture)
    {
        applyingGlue = false;
    }



    private void EnableTouchEvents()
    {
        EasyTouch.On_TouchDown += GetTouchPosition;
        EasyTouch.On_TouchUp += OnTouchRelease;
    }

    private void DisableTouchEvents()
    {
        EasyTouch.On_TouchDown -= GetTouchPosition;
        EasyTouch.On_TouchUp -= OnTouchRelease;
    }

    void OnEnable()
    {
        EnableTouchEvents();
    }
    void OnDisable()
    {
        DisableTouchEvents();
    }
    void OnDestroy()
    {
        DisableTouchEvents();
    }
}
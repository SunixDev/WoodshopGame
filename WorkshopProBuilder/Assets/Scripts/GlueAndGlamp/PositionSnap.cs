using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PositionSnap : MonoBehaviour 
{
    public List<AnchorPoint> PieceAnchorPoints;
    public float SnapOffset = 0.01f;
    public bool AllAnchorsConnected { get; set; }

    private PieceController controller;
    private bool PieceSelected;

	void Start ()
    {
        Initialize(GetComponent<PieceController>());
    }

    public void Initialize(PieceController con)
    {
        controller = con;
        AllAnchorsConnected = false;
    }

    void Update()
    {
        foreach (AnchorPoint currentAnchor in PieceAnchorPoints)
        {
            if (!AllAnchorsConnected && currentAnchor.CanConnect && PieceSelected)
            {
                foreach (AnchorPoint otherAnchor in currentAnchor.ConnectingAnchorPoints)
                {
                    if (otherAnchor != null)
                    {
                        if (!otherAnchor.Connected && otherAnchor.CanConnect)
                        {
                            float distance = Vector3.Distance(currentAnchor.GetPosition(), otherAnchor.GetPosition());
                            if (distance <= SnapOffset)
                            {
                                SnapToAnchors(currentAnchor, otherAnchor);
                            }
                        }
                    }
                }
            }
        }
    }

    private void SnapToAnchors(AnchorPoint currentAnchor, AnchorPoint otherAnchor)
    {
        Vector3 nextPosition = Vector3.MoveTowards(currentAnchor.GetPosition(), otherAnchor.GetPosition(), 1.0f);
        float magnitude = Vector3.Magnitude(nextPosition - currentAnchor.GetPosition());
        Vector3 direction = Vector3.Normalize(nextPosition - currentAnchor.GetPosition());
        transform.position += (direction * magnitude);
        currentAnchor.Connected = true;
        otherAnchor.Connected = true;
        if (controller != null)
        {
            controller.enabled = false;
        }
    }

    public void EnableSnapping(Gesture gesture)
    {
        PieceSelected = (gesture.pickedObject == gameObject);
    }
    
    void OnEnable()
    {
        EasyTouch.On_TouchStart += EnableSnapping;
    }

    void OnDestroy()
    {
        EasyTouch.On_TouchStart -= EnableSnapping;
    }

    void OnDisable()
    {
        EasyTouch.On_TouchStart -= EnableSnapping;
    }
}

using UnityEngine;
using System.Collections;

public class PlayerGlue : MonoBehaviour 
{
    public float applicationRate = 20f;
    public float maxAmount = 100f;

    private int glueLayerMask;
    private Vector2 currentTouchPosition;
    private Vector2 previousTouchPosition;

    void Awake()
    {
        glueLayerMask = LayerMask.GetMask("Glue");
        currentTouchPosition = new Vector2(-1f, -1f);
        previousTouchPosition = currentTouchPosition;
    }

    void Update()
    {
        if (previousTouchPosition != currentTouchPosition)
        {
            Ray ray = Camera.main.ScreenPointToRay(currentTouchPosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100f, glueLayerMask))
            {
                GlueBox glueBox = hit.collider.gameObject.GetComponent<GlueBox>();
                if (glueBox != null)
                {
                    glueBox.ApplyGlue(applicationRate, maxAmount);
                }
            }
            previousTouchPosition = currentTouchPosition;
        }

    }

    public void GetTouchPosition(Gesture gesture)
    {
        if (gesture.pickedObject != null)
        {
            if (gesture.pickedObject.tag == "GlueHitBox")
            {
                currentTouchPosition = gesture.position;
            }
        }
    }

    private void EnableTouchEvents()
    {
        EasyTouch.On_TouchDown += GetTouchPosition;
    }

    private void DisableTouchEvents()
    {
        EasyTouch.On_TouchDown -= GetTouchPosition;        
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





//private Vector3 origin;
//private Transform piece;
//private bool playerPickedPiece;

//void Start()
//{
//    origin = Vector3.zero;
//    piece = null;
//    playerPickedPiece = false;
//}
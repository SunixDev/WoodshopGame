using UnityEngine;
using System.Collections;

public class BladeController : MonoBehaviour 
{
    public float MaxHeight;
    public float MinHeight;
    public float MovementSpeed = 0.05f;
    public bool Moveable = false;

    private bool selected = false;
    private Vector2 previousCursorPosition;

    public void SelectBlade(Gesture gesture)
    {
        if (gesture.pickedObject != null)
        {
            if (Moveable && gesture.pickedObject == gameObject && gesture.touchCount == 1)
            {
                selected = true;
                previousCursorPosition = gesture.position;
            }
        }
    }

    public void MoveBlade(Gesture gesture)
    {
        if (Moveable && selected && gesture.touchCount == 1)
        {
            Vector2 currentCursorPosition = gesture.position;
            if (currentCursorPosition != previousCursorPosition)
            {
                Vector2 delta = currentCursorPosition - previousCursorPosition;
                float speed = MovementSpeed;
                //if (delta.y < 0)
                //{
                //    speed *= -1;
                //}
                gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + (delta.y * speed * Time.deltaTime), gameObject.transform.position.z);
                if (gameObject.transform.position.y > MaxHeight)
                {
                    gameObject.transform.position = new Vector3(gameObject.transform.position.x, MaxHeight, gameObject.transform.position.z);
                }
                if (gameObject.transform.position.y < MinHeight)
                {
                    gameObject.transform.position = new Vector3(gameObject.transform.position.x, MinHeight, gameObject.transform.position.z);
                }
                previousCursorPosition = currentCursorPosition;
            }
        }
    }

    public void ReleaseBlade(Gesture gesture)
    {
        if (Moveable && selected)
        {
            selected = false;
        }
    }

    void OnEnable()
    {
        Subscribe();
    }

    void OnDisable()
    {
        Unsubscribe();
    }

    void OnDestory()
    {
        Unsubscribe();
    }

    private void Subscribe()
    {
        EasyTouch.On_TouchStart += SelectBlade;
        EasyTouch.On_Drag += MoveBlade;
        EasyTouch.On_DragEnd += ReleaseBlade;
    }

    private void Unsubscribe()
    {
        EasyTouch.On_TouchStart -= SelectBlade;
        EasyTouch.On_Drag -= MoveBlade;
        EasyTouch.On_DragEnd -= ReleaseBlade;
    }
}

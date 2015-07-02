using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoardController : MonoBehaviour
{
    private float cursorZ_Position;
    private bool movingBoard;

    void Start()
    {
        movingBoard = false;
    }

    void Update()
    {
        RaycastHit hit;
        if (Input.GetMouseButtonDown(0))
        {
            Ray cursorRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(cursorRay, out hit))
            {
                string hitTag = hit.collider.tag;
                if (!movingBoard && (hitTag == "Piece" || hitTag == "Leftover"))
                {
                    movingBoard = true;
                    cursorZ_Position = hit.point.z;
                }
            }
        }
        else if (Input.GetMouseButton(0))
        {
            Ray cursorRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(cursorRay, out hit))
            {
                string hitTag = hit.collider.tag;
                if (movingBoard)
                {
                    moveBoard(hit.point.z);
                }
            }
        }else if (Input.GetMouseButtonUp(0))
        {
            movingBoard = false;
        }
    }

    private void moveBoard(float hitPosition)
    {
        float cursorZ_DeltaPosition = hitPosition;
        float delta = cursorZ_Position - cursorZ_DeltaPosition;
        if (delta != 0.0f)
        {
            transform.position += new Vector3(0.0f, 0.0f, -delta);
        }
        cursorZ_Position = cursorZ_DeltaPosition;
    }

    public bool isBoardMoving()
    {
        return movingBoard;
    }
}


/*Movement that includes side to side control*/
//Vector3 cursorDeltaPosition = hitPosition;
//float delta = Vector3.Distance(cursorPosition, cursorDeltaPosition);
//if (delta != 0.0f)
//{
//    Vector3 deltaVector = cursorDeltaPosition - cursorPosition;
//    transform.position += new Vector3(deltaVector.x, 0.0f, deltaVector.z);
//}
//cursorPosition = cursorDeltaPosition;
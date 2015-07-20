using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoardController : MonoBehaviour
{
    private Vector3 cursor_Position;
    private bool movingBoard;

    void Start()
    {
        movingBoard = false;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray cursorRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(cursorRay, out hit))
            {
                string hitTag = hit.collider.tag;
                if (!movingBoard && (hitTag == "Piece" || hitTag == "Leftover" || hitTag == "ProjectMaterial"))
                {
                    movingBoard = true;
                    cursor_Position = hit.point;
                }
                else
                {
                    Debug.Log("BoardController Error - Object Hit Tag: " + hitTag);
                }
            }
        }
        else if (Input.GetMouseButton(0))
        {
            RaycastHit hit;
            Ray cursorRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(cursorRay, out hit))
            {
                if (movingBoard)
                {
                    moveBoard(hit.point);
                }
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            movingBoard = false;
        }
    }

    private void moveBoard(Vector3 hitPosition)
    {
        Vector3 cursorDeltaPosition = hitPosition;
        float delta = Vector3.Distance(cursor_Position, cursorDeltaPosition);
        if (delta != 0.0f)
        {
            Vector3 deltaVector = cursorDeltaPosition - cursor_Position;
            transform.position += new Vector3(deltaVector.x, 0.0f, deltaVector.z);
        }
        cursor_Position = cursorDeltaPosition;
    }

    public bool isBoardMoving()
    {
        return movingBoard;
    }
}


/*Movement that includes side to side control*/
//float cursor_DeltaPosition = hitPosition;
//float delta = cursor_Position - cursor_DeltaPosition;
//if (delta != 0.0f)
//{
//    transform.position += new Vector3(0.0f, 0.0f, -delta);
//}
//cursor_Position = cursor_DeltaPosition;
using UnityEngine;
using System.Collections;

public class DadoBlockArea : MonoBehaviour 
{
    public Transform TopLeftCorner;
    public Transform TopRightCorner;
    public Transform BottomLeftCorner;
    public Transform BottomRightCorner;
    //public Transform TestPosition;

    //void Update()
    //{
    //    Debug.Log("WithinTopLeftCorner: " + WithinTopLeftCorner(TestPosition.position));
    //    Debug.Log("WithinTopRightCorner: " + WithinTopRightCorner(TestPosition.position));
    //    Debug.Log("WithinBottomLeftCorner: " + WithinBottomLeftCorner(TestPosition.position));
    //    Debug.Log("WithinBottomRightCorner: " + WithinBottomRightCorner(TestPosition.position));
    //}

    private bool WithinTopLeftCorner(Vector3 vector)
    {
        Vector3 toRight = (TopLeftCorner.position + TopLeftCorner.right) - TopLeftCorner.position;
        Vector3 toBottom = (TopLeftCorner.position + -TopLeftCorner.up) - TopLeftCorner.position;
        Vector3 towardsVector = vector - TopLeftCorner.position;

        float angleFromRight = Vector3.Angle(toRight, towardsVector);
        float angleFromBottom = Vector3.Angle(toBottom, towardsVector);

        return (angleFromRight < 90.0f && angleFromBottom < 90.0f);
    }

    private bool WithinTopRightCorner(Vector3 vector)
    {
        Vector3 toLeft = (TopRightCorner.position + -TopRightCorner.right) - TopRightCorner.position;
        Vector3 toBottom = (TopRightCorner.position + -TopRightCorner.up) - TopRightCorner.position;
        Vector3 towardsVector = vector - TopRightCorner.position;

        float angleFromLeft = Vector3.Angle(toLeft, towardsVector);
        float angleFromBottom = Vector3.Angle(toBottom, towardsVector);

        return (angleFromLeft < 90.0f && angleFromBottom < 90.0f);
    }

    private bool WithinBottomLeftCorner(Vector3 vector)
    {
        Vector3 toRight = (BottomLeftCorner.position + BottomLeftCorner.right) - BottomLeftCorner.position;
        Vector3 toTop = (BottomLeftCorner.position + BottomLeftCorner.up) - BottomLeftCorner.position;
        Vector3 towardsVector = vector - BottomLeftCorner.position;

        float angleFromRight = Vector3.Angle(toRight, towardsVector);
        float angleFromTop = Vector3.Angle(toTop, towardsVector);

        return (angleFromRight < 90.0f && angleFromTop < 90.0f);
    }

    private bool WithinBottomRightCorner(Vector3 vector)
    {
        Vector3 toLeft = (BottomRightCorner.position + -BottomRightCorner.right) - BottomRightCorner.position;
        Vector3 toTop = (BottomRightCorner.position + BottomRightCorner.up) - BottomRightCorner.position;
        Vector3 towardsVector = vector - BottomRightCorner.position;

        float angleFromLeft = Vector3.Angle(toLeft, towardsVector);
        float angleFromTop = Vector3.Angle(toTop, towardsVector);

        return (angleFromLeft < 90.0f && angleFromTop < 90.0f);
    }

    public bool WithinDadoCut(Vector3 position)
    {
        bool withinTopRight = WithinTopRightCorner(position);
        bool withinTopLeft = WithinTopLeftCorner(position);
        bool withinBottomRight = WithinBottomRightCorner(position);
        bool withinBottomLeft = WithinBottomLeftCorner(position);
        return (withinTopRight && withinTopLeft && withinBottomRight && withinBottomLeft);
    }

    void OnDrawGizmos()
    {
        if (TopLeftCorner != null && TopRightCorner != null && BottomLeftCorner != null && BottomRightCorner != null)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawLine(TopLeftCorner.position, TopRightCorner.position);
            Gizmos.DrawLine(TopRightCorner.position, BottomRightCorner.position);
            Gizmos.DrawLine(BottomRightCorner.position, BottomLeftCorner.position);
            Gizmos.DrawLine(BottomLeftCorner.position, TopLeftCorner.position);
        }
    }
}

using UnityEngine;
using System.Collections;

public class Ruler : MonoBehaviour 
{
    public float OffsetFromLine = 0.01f;
    private IToolManager manager;
    private LineRenderer line;
    private Transform pickedObjectTransform;
    private Vector3 SecondPosition;
    public bool CanMeasure { get; set; }

    public void AssignManager(IToolManager thisManager)
    {
        manager = thisManager;
        CanMeasure = false;
    }

    public void AddMark(Gesture gesture)
    {
        if (gesture.pickedObject != null && CanMeasure)
        {
            if (gesture.pickedObject.tag == "Piece" || gesture.pickedObject.tag == "Leftover")
            {
                GameObject markLine = new GameObject();
                markLine.name = "Line Mark";

                Ray ray = Camera.main.ScreenPointToRay(new Vector3(gesture.position.x, gesture.position.y, 0.0f));
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    markLine.transform.position = hit.point;
                    markLine.transform.parent = gesture.pickedObject.transform;
                    line = markLine.AddComponent<LineRenderer>();
                    line.SetVertexCount(2);
                    line.SetWidth(0.005f, 0.005f);
                    line.SetPosition(0, markLine.transform.position + new Vector3(0.0f, 0.001f, 0.0f));
                    line.SetPosition(1, markLine.transform.position + new Vector3(0.0f, 0.001f, 0.0f));
                    pickedObjectTransform = gesture.pickedObject.transform;
                }
            }
        }
    }

    public void SwipeMark(Gesture gesture)
    {
        if (gesture.pickedObject != null && CanMeasure)
        {
            if (gesture.pickedObject.tag == "Piece" || gesture.pickedObject.tag == "Leftover")
            {
                Ray ray = Camera.main.ScreenPointToRay(new Vector3(gesture.position.x, gesture.position.y, 0.0f));
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == gesture.pickedObject)
                {
                    Vector3 position = hit.point;
                    SecondPosition = position + new Vector3(0.0f, 0.001f, 0.0f);
                    line.SetPosition(1, SecondPosition);
                }
            }
        }
    }

    public void LeaveMark(Gesture gesture)
    {
        if (gesture.pickedObject != null && CanMeasure)
        {
            if (gesture.pickedObject.tag == "Piece" || gesture.pickedObject.tag == "Leftover")
            {
                CutLine nearestLine = manager.GetNearestLine(line.gameObject.transform.position);
                Vector3 markedPosition = line.gameObject.transform.position;
                if (MarkIsNearLine(nearestLine, markedPosition))
                {
                    if (nearestLine.IsMarked)
                    {
                        GameObject previousLine = nearestLine.LineMark;
                        nearestLine.LineMark = null;
                        Destroy(previousLine);
                    }
                    nearestLine.LineMark = line.gameObject;
                    nearestLine.IsMarked = true;
                    line.SetColors(Color.green, Color.green);
                    LineMark markedLine = line.gameObject.AddComponent<LineMark>();
                    markedLine.StartPoint = line.gameObject.transform;
                    GameObject secondPoint = new GameObject();
                    secondPoint.transform.position = SecondPosition;
                    secondPoint.transform.parent = line.gameObject.transform.parent;
                    markedLine.EndPoint = secondPoint.transform;
                    markedLine.line = line;
                }
                else
                {
                    Destroy(line.gameObject);
                }
            }
        }
    }

    public bool MarkIsNearLine(CutLine line, Vector3 markPosition)
    {
        bool closeToLine = false;
        if (line.CutType == CutLineType.TableSawCut)
        {
            float distanceToLine = line.CalculateDistance(line.gameObject.transform.position);
            closeToLine = (distanceToLine < OffsetFromLine);
        }
        else if (line.CutType == CutLineType.ChopSawCut)
        {
            Vector3 linePosition = line.Checkpoints[0].GetPosition();
            Vector3 adjustedMarkedPosition = new Vector3(markPosition.x, linePosition.y, markPosition.z);
            Vector3 toAdjustedMark = adjustedMarkedPosition - linePosition;
            Vector3 rightOfLine = (linePosition + line.Checkpoints[0].transform.forward) - linePosition;
            float projectionOntoLine = Vector3.Dot(toAdjustedMark, rightOfLine);
            closeToLine = (Mathf.Abs(projectionOntoLine) < OffsetFromLine);
        }
        return closeToLine;
    }

    void OnEnable()
    {
        SubscribeAll();
    }

    void OnDisable()
    {
        UnsubscribeAll();
    }

    void OnDestroy()
    {
        UnsubscribeAll();
    }

    private void SubscribeAll()
    {
        EasyTouch.On_TouchStart += AddMark;
        EasyTouch.On_Drag += SwipeMark;
        EasyTouch.On_DragEnd += LeaveMark;
    }

    private void UnsubscribeAll()
    {
        EasyTouch.On_TouchStart -= AddMark;
        EasyTouch.On_Drag -= SwipeMark;
        EasyTouch.On_DragEnd -= LeaveMark;
    }
}

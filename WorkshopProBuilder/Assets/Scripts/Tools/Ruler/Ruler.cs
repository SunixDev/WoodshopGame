using UnityEngine;
using System.Collections;

public class Ruler : MonoBehaviour 
{
    public float OffsetFromLine = 0.01f;
    public Material LineMarkMaterial;
    public Color GoodLineColor = new Color(0f, 0.6f, 0.01f, 1f);
    public Color BadLineColor = new Color(0.7f, 0f, 0f, 1f);

    private IToolManager manager;
    private LineRenderer currentLineRenderer;
    private Transform pickedObjectTransform;
    private Vector3 SecondPosition;
    public bool CanMeasure { get; set; }

    private GameObject currentLineMark;

    public void AssignManager(IToolManager thisManager)
    {
        manager = thisManager;
        CanMeasure = false;
    }

    public void AddMark(Gesture gesture)
    {
        if (gesture.pickedObject != null && CanMeasure && gesture.touchCount == 1)
        {
            if (gesture.pickedObject.tag == "Piece" || gesture.pickedObject.tag == "Leftover")
            {
                currentLineMark = new GameObject();
                currentLineMark.name = "Line Mark";

                Ray ray = Camera.main.ScreenPointToRay(new Vector3(gesture.position.x, gesture.position.y, 0.0f));
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    currentLineMark.transform.position = hit.point;
                    currentLineMark.transform.parent = gesture.pickedObject.transform;
                    currentLineRenderer = currentLineMark.AddComponent<LineRenderer>();
                    currentLineRenderer.material = new Material(LineMarkMaterial);
                    currentLineRenderer.SetVertexCount(2);
                    currentLineRenderer.SetWidth(0.005f, 0.005f);
                    currentLineRenderer.SetPosition(0, currentLineMark.transform.position + new Vector3(0.0f, 0.001f, 0.0f));
                    currentLineRenderer.SetPosition(1, currentLineMark.transform.position + new Vector3(0.0f, 0.001f, 0.0f));
                    pickedObjectTransform = gesture.pickedObject.transform;
                }
            }
        }
    }

    public void SwipeMark(Gesture gesture)
    {
        if (gesture.pickedObject != null && CanMeasure && gesture.touchCount == 1)
        {
            if (gesture.pickedObject.tag == "Piece" || gesture.pickedObject.tag == "Leftover")
            {
                Ray ray = Camera.main.ScreenPointToRay(new Vector3(gesture.position.x, gesture.position.y, 0.0f));
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit) && (gesture.pickedObject.tag == "Piece" || gesture.pickedObject.tag == "Leftover"))
                {
                    Vector3 position = hit.point;
                    SecondPosition = position + new Vector3(0.0f, 0.001f, 0.0f);
                    currentLineRenderer.SetPosition(1, SecondPosition);
                }
            }
        }
    }

    public void LeaveMark(Gesture gesture)
    {
        if (gesture.pickedObject != null && CanMeasure && gesture.touchCount == 1)
        {
            if ((gesture.pickedObject.tag == "Piece" || gesture.pickedObject.tag == "Leftover") && SecondPointIsOnWoodMaterial(gesture.position))
            {
                LineMark markedLine = currentLineRenderer.gameObject.AddComponent<LineMark>();
                CutLine nearestLine = manager.GetNearestLine(currentLineRenderer.gameObject.transform.position);
                Vector3 markedPosition = currentLineRenderer.gameObject.transform.position;
                if (nearestLine.IsMarked)
                {
                    GameObject previousLine = nearestLine.LineMark;
                    nearestLine.LineMark = null;
                    Destroy(previousLine);
                }
                if (MarkIsNearLine(nearestLine, markedPosition))
                {
                    markedLine.GoodLineMark = true;
                    nearestLine.IsMarked = true;
                    currentLineRenderer.material.color = GoodLineColor;
                }
                else
                {
                    markedLine.GoodLineMark = false;
                    nearestLine.IsMarked = true;
                    currentLineRenderer.material.color = BadLineColor;
                }
                nearestLine.LineMark = currentLineRenderer.gameObject;
                markedLine.StartPoint = currentLineRenderer.gameObject.transform;
                GameObject secondPoint = new GameObject();
                secondPoint.transform.position = SecondPosition;
                secondPoint.transform.parent = currentLineMark.transform;
                markedLine.EndPoint = secondPoint.transform;
                markedLine.line = currentLineRenderer;

            }
            else
            {
                if (currentLineRenderer != null)
                {
                    Destroy(currentLineRenderer.material);
                    Destroy(currentLineRenderer.gameObject);
                }
            }
        }
        else
        {
            if (currentLineRenderer != null)
            {
                Destroy(currentLineRenderer.material);
                Destroy(currentLineRenderer.gameObject);
            }
        }
    }

    public bool MarkIsNearLine(CutLine line, Vector3 markPosition)
    {
        bool closeToLine = false;
        if (line.CutType == CutLineType.TableSawCut)
        {
            float distanceToLine = line.CalculateDistance(markPosition);
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

    private bool SecondPointIsOnWoodMaterial(Vector2 gesturePosition)
    {
        bool valid = false;
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(gesturePosition.x, gesturePosition.y, 0f));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            valid = (hit.collider.gameObject.tag == "Piece" || hit.collider.gameObject.tag == "Leftover");
        }
        return valid;
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

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CutLine : MonoBehaviour 
{
    public CutLineType CutType;
    public List<Checkpoint> Checkpoints;
    public List<Connection> Connections;
    public bool IsMarked = false;
    public GameObject LineMark { get; set; }
    public LineRenderer lineRenderer;

    private bool CutBackwards = false;
    private int CheckpointIndex = 0;

    void Start()
    {
        lineRenderer = gameObject.GetComponent<LineRenderer>();
        if (gameObject.GetComponent<LineRenderer>() == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }
        lineRenderer.SetVertexCount(Checkpoints.Count);
        lineRenderer.SetWidth(0.005f, 0.005f);
        for (int i = 0; i < Checkpoints.Count; i++)
        {
            Vector3 offset = (CutType == CutLineType.ChopSawCut) ? -(Checkpoints[0].transform.right * 0.001f) : new Vector3(0.0f, 0.001f, 0.0f);
            lineRenderer.SetPosition(i, Checkpoints[i].GetPosition() + offset);
        }
        lineRenderer.enabled = false;
    }

    void Update()
    {
        if (lineRenderer.enabled)
        {
            for (int i = 0; i < Checkpoints.Count; i++)
            {
                Vector3 offset = (CutType == CutLineType.ChopSawCut) ? -(Checkpoints[0].transform.right * 0.001f) : new Vector3(0.0f, 0.001f, 0.0f);
                lineRenderer.SetPosition(i, Checkpoints[i].GetPosition() + offset);
            }
        }
    }

    public Vector3 GetCurrentCheckpointPosition()
    {
        return Checkpoints[CheckpointIndex].GetPosition();
    }

    public void SeverConnections()
    {
        foreach (Connection c in Connections)
        {
            c.Disconnect();
        }
    }

    public bool ContainsPiece(Node node)
    {
        bool HasPiece = false;
        for (int i = 0; i < Connections.Count && !HasPiece; i++)
        {
            Connection c = Connections[i];
            HasPiece = (c.FirstPiece == node || c.SecondPiece == node);
        }
        return HasPiece;
    }

    public Node GetFirstBaseNode()
    {
        return Connections[0].FirstPiece;
    }

    public Node GetSecondBaseNode()
    {
        return Connections[0].SecondPiece;
    }

    public bool LineIsCut()
    {
        bool lineCut = (CheckpointIndex < 0 || CheckpointIndex >= Checkpoints.Count);
        return lineCut;
    }

    public void DisplayLine(bool display, bool showMark)
    {
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.GetComponent<LineRenderer>();
        }

        lineRenderer.enabled = display;
        if (LineMark != null)
        {
            LineMark.SetActive(showMark);
        }

    }

    public void UpdateLine(Vector3 bladePosition)
    {
        if (CutType == CutLineType.TableSawCut)
        {
            Vector3 difference = Checkpoints[CheckpointIndex].GetPosition() - bladePosition;
            if (difference.z >= 0)
            {
                UpdateToNextCheckpoint();
            }
        }
        else if (CutType == CutLineType.ChopSawCut)
        {
            Vector3 difference = Checkpoints[CheckpointIndex].GetPosition() - bladePosition;
            if (difference.y <= 0)
            {
                UpdateToNextCheckpoint();
            }
        }
        else if (CutType == CutLineType.CurvatureCut)
        {
            float validDistance = 0.001f;
            float distance = Vector3.Distance(Checkpoints[CheckpointIndex].GetPosition(), bladePosition);
            if (distance <= validDistance)
            {
                UpdateToNextCheckpoint();
            }
        }
    }

    public void DetermineCutDirection(Vector3 position)
    {
        Vector3 firstPosition = Checkpoints[0].GetPosition();
        Vector3 lastPosition = Checkpoints[Checkpoints.Count - 1].GetPosition();
        float distanceFromFirst = Vector3.Distance(position, firstPosition);
        float distanceFromLast = Vector3.Distance(position, lastPosition);

        if (distanceFromFirst < distanceFromLast)
        {
            CheckpointIndex = 0;
            CutBackwards = false;
        }
        else if (distanceFromLast < distanceFromFirst)
        {
            CheckpointIndex = Checkpoints.Count - 1;
            CutBackwards = true;
        }
    }

    private void UpdateToNextCheckpoint()
    {
        if (CutBackwards)
        {
            CheckpointIndex--;
        }
        else
        {
            CheckpointIndex++;
        }
    }

    public float CalculateDistance(Vector3 bladePosition)
    {
        Vector3 origin = GetPreviousCheckpoint();
        Vector3 nextCheckpoint = Checkpoints[CheckpointIndex].GetPosition();
        if ((CutBackwards && CheckpointIndex == Checkpoints.Count - 1) || (!CutBackwards && CheckpointIndex == 0))
        {
            origin = Checkpoints[CheckpointIndex].GetPosition();
            nextCheckpoint = GetNextCheckpoint();
        }

        Vector3 toBladeEdge = bladePosition - origin;
        Vector3 toCheckpoint = nextCheckpoint - origin;
        toCheckpoint.Normalize();

        float projection = Vector3.Dot(toBladeEdge, toCheckpoint);
        toCheckpoint = toCheckpoint * projection;
        Vector3 rejectionVector = toBladeEdge - toCheckpoint;

        return rejectionVector.magnitude;
    }

    private Vector3 GetFirstCheckpoint()
    {
        Checkpoint firstPoint = null;
        if (Checkpoints.Count == 1)
        {
            firstPoint = Checkpoints[0];
        }
        else if (Checkpoints.Count == 0)
        {
            firstPoint = null;
        }
        else if (CutBackwards)
        {
            firstPoint = Checkpoints[Checkpoints.Count - 1];
        }
        else
        {
            firstPoint = Checkpoints[0];
        }
        return firstPoint.GetPosition();
    }

    private Vector3 GetNextCheckpoint()
    {
        Checkpoint nextPoint = null;
        int nextIndex = (CutBackwards) ? CheckpointIndex - 1 : CheckpointIndex + 1;
        if (nextIndex >= 0 && nextIndex < Checkpoints.Count)
        {
            nextPoint = Checkpoints[nextIndex];
        }
        else
        {
            return Vector3.zero;
        }
        return nextPoint.GetPosition();
    }

    private Vector3 GetPreviousCheckpoint()
    {
        Checkpoint previousPoint = null;
        int previousIndex = (CutBackwards) ? CheckpointIndex + 1 : CheckpointIndex - 1;
        if (previousIndex >= 0 && previousIndex < Checkpoints.Count)
        {
            previousPoint = Checkpoints[previousIndex];
        }
        else
        {
            return Vector3.zero;
        }
        return previousPoint.GetPosition();
    }

    private Vector3 GetLastCheckpoint()
    {
        Checkpoint lastPoint = null;
        if (Checkpoints.Count == 1)
        {
            lastPoint = Checkpoints[0];
        }
        else if (Checkpoints.Count == 0)
        {
            lastPoint = null;
        }
        else if (CutBackwards)
        {
            lastPoint = Checkpoints[0];
        }
        else
        {
            lastPoint = Checkpoints[Checkpoints.Count - 1];
        }

        return lastPoint.GetPosition();
    }

    void OnDrawGizmos()
    {
        for (int i = 0; i < Checkpoints.Count && i + 1 < Checkpoints.Count; i++)
        {
            if (Checkpoints[i] == null)
            {
                Debug.LogError("Checkpoint at index " + i + " is missing");
            }
            else if (Checkpoints[i + 1] == null)
            {
                Debug.LogError("Checkpoint at index " + (i + 1) + " is missing");
            }
            else
            {
                Gizmos.color = Color.red;
                Vector3 fromVector = Checkpoints[i].gameObject.transform.position;
                Vector3 toVector = Checkpoints[i + 1].gameObject.transform.position;
                Gizmos.DrawLine(fromVector, toVector);
            }
        }
    }

    void OnDestroy()
    {
        Destroy(LineMark);
    }
}
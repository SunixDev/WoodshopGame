using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CutLine : MonoBehaviour 
{
    public CutLineType CutType;
    public List<Checkpoint> Checkpoints;
    public List<Connection> Connections;
    public bool ShowLines = false;
    public bool CutBackwards { get; set; }

    private int CheckpointIndex = 0;
    private LineRenderer lineRenderer = null;
    private int firstCheckpoint;
    private int lastCheckpoint;


	void Start ()
    {
        if (Checkpoints.Count <= 1)
        {
            Debug.LogWarning("Not enough points to make a line");
        }
        if (ShowLines)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.SetVertexCount(Checkpoints.Count);
            lineRenderer.SetWidth(0.005f, 0.005f);
        }
        firstCheckpoint = 0;
        lastCheckpoint = Checkpoints.Count - 1;
	}

    void Update()
    {
        if (ShowLines)
        {
            for (int i = 0; i < Checkpoints.Count; i++)
            {
                lineRenderer.SetPosition(i, Checkpoints[i].GetPosition() + new Vector3(0.0f, 0.001f, 0.0f));
            }
        }
    }

    public Checkpoint GetCurrentCheckpoint()
    {
        Checkpoint currentPoint = null;
        if (CutBackwards && CheckpointIndex >= 0)
        {
            currentPoint = Checkpoints[CheckpointIndex];
        }
        else if(!CutBackwards && CheckpointIndex < Checkpoints.Count)
        {
            currentPoint = Checkpoints[CheckpointIndex];
        }
        return currentPoint;
    }

    public void UpdateToNextCheckpoint()
    {
        if (CutBackwards)
        {
            CheckpointIndex--;
            lastCheckpoint--;
        }
        else
        {
            CheckpointIndex++;
            firstCheckpoint++;
        }
    }

    public Checkpoint GetNextCheckpoint()
    {
        Checkpoint nextPoint = null;
        if (CutBackwards && (CheckpointIndex - 1) >= 0)
        {
            nextPoint = Checkpoints[CheckpointIndex - 1];
        }
        else if(!CutBackwards && (CheckpointIndex + 1) < Checkpoints.Count)
        {
            nextPoint = Checkpoints[CheckpointIndex + 1];
        }
        return nextPoint;
    }

    public Checkpoint GetPreviousCheckpoint()
    {
        Checkpoint previousPoint = null;
        if (CutBackwards && (CheckpointIndex + 1) < Checkpoints.Count)
        {
            previousPoint = Checkpoints[CheckpointIndex + 1];
        }
        else if (!CutBackwards && (CheckpointIndex - 1) >= 0)
        {
            previousPoint = Checkpoints[CheckpointIndex - 1];
        }
        return previousPoint;
    }

    public Checkpoint GetFirstCheckpoint()
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
        return firstPoint;
    }

    public Checkpoint GetFirstCheckpointInList()
    {
        if (Checkpoints.Count == 0)
        {
            return null;
        }
        return Checkpoints[firstCheckpoint];
    }

    public Checkpoint GetLastCheckpoint()
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

        return lastPoint;
    }

    public Checkpoint GetLastCheckpointInList()
    {
        if (Checkpoints.Count == 0)
        {
            return null;
        }
        return Checkpoints[lastCheckpoint];
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
        for (int i = 0; i < Connections.Count && !HasPiece; i++ )
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

    public void SetCutDirection(Checkpoint nearestCheckpoint)
    {
        int index = Checkpoints.IndexOf(nearestCheckpoint);
        CutBackwards = (index == (Checkpoints.Count - 1));
    }

    public bool OnFirstCheckpoint()
    {
        return GetPreviousCheckpoint() == null && GetNextCheckpoint() != null;
    }

    public bool OnConnectedCheckpoint()
    {
        return GetPreviousCheckpoint() != null && GetNextCheckpoint() != null;
    }

    public bool OnLastCheckpoint()
    {
        return GetPreviousCheckpoint() != null && GetNextCheckpoint() == null;
    }

    void OnDrawGizmos()
    {
        for (int i = 0; i < Checkpoints.Count && i+1 < Checkpoints.Count; i++)
        {
            if (Checkpoints[i] == null)
            {
                Debug.LogError("Checkpoint at index " + i + " is missing");
            }
            else if (Checkpoints[i + 1] == null)
            {
                Debug.LogError("Checkpoint at index " + (i+1) + " is missing");
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
}

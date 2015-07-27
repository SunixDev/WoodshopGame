using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CutLine : MonoBehaviour 
{
    public CutLineType CutType;
    public List<Checkpoint> Checkpoints;
    public List<Connection> Connections;

    private int CheckpointIndex = 0;

	void Start () 
    {
        if (Checkpoints.Count <= 1)
        {
            Debug.LogWarning("Not enough points to make a line");
        }
	}

    public Checkpoint GetCurrentCheckpoint()
    {
        if (CheckpointIndex >= Checkpoints.Count)
        {
            return null;
        }
        return Checkpoints[CheckpointIndex];
    }

    public void UpdateToNextCheckpoint()
    {
        CheckpointIndex++;
    }

    public Checkpoint GetNextCheckpoint()
    {
        if ((CheckpointIndex + 1) >= Checkpoints.Count)
        {
            return null;
        }
        return Checkpoints[CheckpointIndex + 1];
    }

    public Checkpoint GetPreviousCheckpoint()
    {
        if ((CheckpointIndex - 1) < 0)
        {
            return null;
        }
        return Checkpoints[CheckpointIndex - 1];
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

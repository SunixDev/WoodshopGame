using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CutLine : MonoBehaviour 
{
    public LineCutType CutType;
    public List<Checkpoint> Checkpoints;
    public List<GameObject> AttachedPiecesSideOne;
    public List<GameObject> AttachedPiecesSideTwo;

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

    public bool ContainsPiece(GameObject piece)
    {
        return AttachedPiecesSideOne.Contains(piece) || AttachedPiecesSideTwo.Contains(piece);
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

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TableSawCut : MonoBehaviour 
{
    public List<CutLine> LinesToCut;
    public GameObject WoodPiece;
    public Transform BladeEdge;
    public CutState state { get; set; }

    private CutLine CurrentLine;
    private int NearestLineIndex;
    //private float CutOffset = 0.01f;

	void Start () 
    {
        state = CutState.ReadyToCut;
        CurrentLine = null;
	}
	
	void Update () 
    {
        if (state == CutState.ReadyToCut)
        {
            //CutLine line = GetNearestLine();
            //Vector3 bladeEdge = new Vector3(BladeEdge.position.x, 0.0f, BladeEdge.position.z);
            //Vector3 checkpoint = new Vector3(line.GetCurrentCheckpoint().GetPosition().x, 0.0f, line.GetCurrentCheckpoint().GetPosition().z);
            //Vector3 difference = checkpoint - bladeEdge;
            //if (difference.z > 0.0f)
            //{
            //    state = CutState.Cutting;
            //    CurrentLine = line;
            //    CurrentLine.UpdateToNextCheckpoint();
            //}
        }
        else if (state == CutState.Cutting)
        {
            //Vector3 bladeEdge = new Vector3(BladeEdge.position.x, 0.0f, BladeEdge.position.z);
            //Vector3 checkpoint = new Vector3(CurrentLine.GetCurrentCheckpoint().GetPosition().x, 0.0f, CurrentLine.GetCurrentCheckpoint().GetPosition().z);
            //Vector3 difference = checkpoint - bladeEdge;
            //if (difference.z > 0.0f)
            //{
            //    CurrentLine.UpdateToNextCheckpoint();
            //    if (CurrentLine.GetCurrentCheckpoint() == null)
            //    {
            //        SeparatePieces();
            //        LinesToCut.RemoveAt(NearestLineIndex);
            //        Destroy(CurrentLine);
            //        NearestLineIndex = -1;
            //        state = CutState.ReadyToCut;
            //    }
            //}
        }
        else
        {
            //Debug.LogError("Issue with line cutting");
        }
	}

    public void SeparatePieces()
    {
        //List<GameObject> freeObjects = new List<GameObject>();
        //foreach (GameObject piece in CurrentLine.AttachedPieces)
        //{
        //    bool isSeparated = true;
        //    for (int i = 0; i < LinesToCut.Count && isSeparated && i != NearestLineIndex; i++)
        //    {
        //        if (LinesToCut[i].AttachedPieces.Contains(piece))
        //        {
        //            isSeparated = false;
        //        }
        //    }
        //    if (isSeparated)
        //    {
        //        freeObjects.Add(piece);
        //    }
        //}
        //GameObject newPiece = new GameObject();
        //newPiece.AddComponent<Rigidbody>();
        //foreach (GameObject piece in freeObjects)
        //{
        //    piece.transform.parent = null;
        //    piece.transform.parent = newPiece.transform;
        //}
    }

    private CutLine GetNearestLine()
    {
        //float nearestDistance = 0;
        //int nearestLineIndex = 0;
        //for (int i = 0; i < LinesToCut.Count; i++)
        //{
        //    if (i == 0)
        //    {
        //        nearestDistance = Vector3.Distance(LinesToCut[i].GetCurrentCheckpoint().GetPosition(), BladeEdge.position);
        //        nearestLineIndex = 0;
        //    }
        //    else
        //    {
        //        float distance = Vector3.Distance(LinesToCut[i].GetCurrentCheckpoint().GetPosition(), BladeEdge.position);
        //        if (distance < nearestDistance)
        //        {
        //            nearestDistance = distance;
        //            nearestLineIndex = i;
        //        }
        //    }
        //}
        //NearestLineIndex = nearestLineIndex;
        //return LinesToCut[nearestLineIndex];
        return null;
    }
}

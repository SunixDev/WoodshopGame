using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WoodMaterialObject : MonoBehaviour 
{
    public List<GameObject> WoodPieces;
    public List<CutLine> LinesToCut;

    private Transform objTransform;

    void Awake()
    {
        if (WoodPieces == null)
        {
            WoodPieces = new List<GameObject>();
        }

        if (LinesToCut == null)
        {
            LinesToCut = new List<CutLine>();
        }

        objTransform = transform;
    }

    public Vector3 GetPosition()
    {
        if(objTransform == null)
        {
            objTransform = transform;
        }
        return objTransform.position;
    }

    public bool ContainsPiece(GameObject piece)
    {
        return WoodPieces.Contains(piece);
    }

    public List<CutLine> RetrieveLinesByType(CutLineType cutType)
    {
        List<CutLine> lines = new List<CutLine>();
        foreach (CutLine cl in LinesToCut)
        {
            if (cl.CutType == cutType)
            {
                lines.Add(cl);
            }
        }
        return lines;
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WoodMaterialObject : MonoBehaviour 
{
    public List<GameObject> WoodPieces;
    public List<CutLine> LinesToCut;

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

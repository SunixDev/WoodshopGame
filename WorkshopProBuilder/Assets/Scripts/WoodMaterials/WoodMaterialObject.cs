using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WoodMaterialObject : MonoBehaviour 
{
    public List<GameObject> WoodPieces;
    public List<CutLine> LinesToCut;
    public List<DadoBlock> DadosToCut;

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

        if (DadosToCut == null)
        {
            DadosToCut = new List<DadoBlock>();
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

    public bool ContainsLine(CutLine line)
    {
        return LinesToCut.Contains(line);
    }

    public bool ContainsDado(DadoBlock block)
    {
        return DadosToCut.Contains(block);
    }

    public void AddLine(CutLine lineToAdd)
    {
        LinesToCut.Add(lineToAdd);
    }

    public void AddDado(DadoBlock blockToAdd)
    {
        DadosToCut.Add(blockToAdd);
    }

    public void RemoveLine(int index)
    {
        LinesToCut.RemoveAt(index);
    }

    public void RemoveDado(int index)
    {
        DadosToCut.RemoveAt(index);
    }

    public void RemoveLine(CutLine lineToRemove)
    {
        LinesToCut.Remove(lineToRemove);
    }

    public void RemoveDado(DadoBlock blockToRemove)
    {
        DadosToCut.Remove(blockToRemove);
    }

    public bool ContainsLinesOfType(CutLineType type)
    {
        bool doesContain = false;
        for (int i = 0; i < LinesToCut.Count && !doesContain; i++)
        {
            if (LinesToCut[i].CutType == type)
            {
                doesContain = true;
            }
        }
        return doesContain;
    }

    public List<CutLine> RetrieveLines(CutLineType cutType, int stepNumber)
    {
        List<CutLine> lines = new List<CutLine>();
        foreach (CutLine cl in LinesToCut)
        {
            StepID step = cl.GetComponent<StepID>();
            if (step.UsedInStep(stepNumber) && cl.CutType == cutType)
            {
                lines.Add(cl);
            }
        }
        return lines;
    }









    //public List<CutLine> RetrieveLinesByType(CutLineType cutType)
    //{
    //    List<CutLine> lines = new List<CutLine>();
    //    foreach (CutLine cl in LinesToCut)
    //    {
    //        if (cl.CutType == cutType)
    //        {
    //            lines.Add(cl);
    //        }
    //    }
    //    return lines;
    //}

    //public List<CutLine> RetrieveLinesByStep(int stepNumber)
    //{
    //    List<CutLine> lines = new List<CutLine>();
    //    foreach (CutLine cl in LinesToCut)
    //    {
    //        StepID step = cl.GetComponent<StepID>();
    //        if (step.UsedInStep(stepNumber))
    //        {
    //            lines.Add(cl);
    //        }
    //    }
    //    return lines;
    //}
}

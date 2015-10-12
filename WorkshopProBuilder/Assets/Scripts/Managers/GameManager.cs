using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour 
{
    //public Project CurrentProject;
    //public Inventory PlayerInventory;
    //public WoodMaterialManager WoodManager;

    private static GameManager _instance;
    public static GameManager instance
    {
        get
        {
            return _instance;
        }
    }

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (this != _instance)
            {
                Destroy(this.gameObject);
            }
        }

        //if (PlayerInventory == null)
        //{
        //    PlayerInventory = GetComponent<Inventory>();
        //}
        //if (WoodManager == null)
        //{
        //    WoodManager = GetComponent<WoodMaterialManager>();
        //}
    }

    public int GetStep()
    {
        return -1;// CurrentProject.GetCurrentStepIndex();
    }

    public List<GameObject> GetNecessaryMaterials(CutLineType cutType)
    {
        //int currentStep = CurrentProject.GetCurrentStepIndex();
        //List<GameObject> allMaterials = WoodManager.GetPiecesByLine(cutType);
        //List<GameObject> allValidMaterials = new List<GameObject>();
        //foreach (GameObject go in allMaterials)
        //{
        //    List<CutLine> lines = go.GetComponent<WoodMaterialObject>().LinesToCut;
        //    foreach (CutLine line in lines)
        //    {
        //        StepID stepID = line.GetComponent<StepID>();
        //        if (stepID != null)
        //        {
        //            if (stepID.UsedInStep(currentStep))
        //            {
        //                allValidMaterials.Add(go);
        //                break;
        //            }
        //        }
        //    }
        //}
        return null;
    }

    public List<GameObject> GetMaterialsWithDadoCuts()
    {
        //int currentStep = CurrentProject.GetCurrentStepIndex();
        //List<GameObject> allMaterials = WoodManager.GetAllWoodMaterials();
        //List<GameObject> allValidMaterials = new List<GameObject>();
        //foreach (GameObject go in allMaterials)
        //{
        //    if (go.GetComponent<WoodMaterialObject>() != null)
        //    {
        //        List<DadoBlock> dadoCuts = go.GetComponent<WoodMaterialObject>().DadosToCut;
        //        foreach (DadoBlock dadoCut in dadoCuts)
        //        {
        //            StepID stepID = dadoCut.GetComponent<StepID>();
        //            if (stepID != null)
        //            {
        //                if (stepID.UsedInStep(currentStep))
        //                {
        //                    allValidMaterials.Add(go);
        //                    break;
        //                }
        //            }
        //        }
        //    }
        //}
        return null;// allValidMaterials;
    }

    public List<GameObject> GetNecessaryPieces()
    {
        //int currentStep = CurrentProject.GetCurrentStepIndex();
        //List<GameObject> allMaterials = WoodManager.GetRevealedPieces();
        //List<GameObject> allValidMaterials = new List<GameObject>();

        //foreach (GameObject go in allMaterials)
        //{
        //    List<StepID> steps = new List<StepID>(go.GetComponents<StepID>());
        //    bool found = false;
        //    for (int i = 0; i < steps.Count && !found; i++)
        //    {
        //        if (steps[i].UsedInStep(currentStep))
        //        {
        //            found = true;
        //            allValidMaterials.Add(go);
        //        }
        //    }
        //}

        return null;//allValidMaterials;
    }

    public void OnApplicationQuit()
    {
        _instance = null;
    }
}

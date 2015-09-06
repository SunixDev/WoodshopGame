using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour 
{
    public Project CurrentProject;
    public Inventory PlayerInventory;
    public WoodMaterialManager WoodManager;

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

        if (PlayerInventory == null)
        {
            PlayerInventory = GetComponent<Inventory>();
        }
        if (WoodManager == null)
        {
            WoodManager = GetComponent<WoodMaterialManager>();
        }
    }

    public int GetStep()
    {
        return CurrentProject.GetCurrentStepIndex();
    }

    public List<GameObject> GetNecessaryMaterials(CutLineType cutType)
    {
        int currentStep = CurrentProject.GetCurrentStepIndex();
        List<GameObject> allMaterials = WoodManager.GetPiecesByLine(cutType);
        List<GameObject> allValidMaterials = new List<GameObject>();
        foreach (GameObject go in allMaterials)
        {
            List<CutLine> lines = go.GetComponent<WoodMaterialObject>().LinesToCut;
            foreach (CutLine line in lines)
            {
                StepID stepID = line.GetComponent<StepID>();
                if (stepID != null)
                {
                    if (stepID.UsedInStep(currentStep))
                    {
                        allValidMaterials.Add(go);
                        break;
                    }
                }
            }
        }
        return allValidMaterials;
    }

    public void OnApplicationQuit()
    {
        _instance = null;
    }
}

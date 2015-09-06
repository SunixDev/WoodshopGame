using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class Requirements : MonoBehaviour
{
    public List<WorkMaterial> Materials = new List<WorkMaterial>();
    public List<int> MaterialCount = new List<int>();
    public Dictionary<WorkMaterial, int> RequiredMaterialsList;

    void Awake()
    {
        RequiredMaterialsList = new Dictionary<WorkMaterial, int>();
        for (int i = 0; i != Math.Min(Materials.Count, MaterialCount.Count); i++)
        {
            RequiredMaterialsList.Add(Materials[i], MaterialCount[i]);
        }
    }

    void Start()
    {
        //foreach (KeyValuePair<WorkMaterial, int> c in RequiredMaterialsList)
        //{
        //    Debug.Log("Material: " + c.Key + " | Count: " + c.Value);
        //}
    }

    public bool HasAllRequiredMaterials(Inventory playerInventory)
    {
        bool hasMaterials = true;

        foreach (KeyValuePair<WorkMaterial, int> mat in RequiredMaterialsList)
        {
            int count = playerInventory.GetMaterialCount(mat.Key);
            if (count == -1 || count < mat.Value)
            {
                hasMaterials = false;
                break;
            }
        }

        return hasMaterials;
    }

    public Dictionary<WorkMaterial, int> GetNeededMaterials(Inventory playerInventory)
    {
        Dictionary<WorkMaterial, int> missingMaterials = new Dictionary<WorkMaterial, int>();

        foreach (KeyValuePair<WorkMaterial, int> mat in RequiredMaterialsList)
        {
            int count = playerInventory.GetMaterialCount(mat.Key);
            if (count < mat.Value)
            {
                missingMaterials.Add(mat.Key, mat.Value - mat.Value);
            }
            else if (count == -1)
            {

            }
        }

        return missingMaterials;
    }
}

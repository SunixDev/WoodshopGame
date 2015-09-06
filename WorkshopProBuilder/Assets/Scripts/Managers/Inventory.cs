using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Inventory : MonoBehaviour 
{
    public List<WorkMaterial> Materials = new List<WorkMaterial>();
    public List<int> MaterialCount = new List<int>();
    public Dictionary<WorkMaterial, int> AvailableMaterials;

    void Awake()
    {
        AvailableMaterials = new Dictionary<WorkMaterial, int>();
        for (int i = 0; i != Math.Min(Materials.Count, MaterialCount.Count); i++)
        {
            AvailableMaterials.Add(Materials[i], MaterialCount[i]);
        }
    }

    public bool IncreaseMaterialAmount(WorkMaterial material, int amount = 1)
    {
        bool successful = true;
        if (AvailableMaterials.ContainsKey(material))
        {
            AvailableMaterials[material] = AvailableMaterials[material] + amount;
        }
        else
        {
            successful = false;
        }

        return successful;
    }

    public bool DecreaseMaterialAmount(WorkMaterial material, int amount = 1)
    {
        bool successful = true;
        if (AvailableMaterials.ContainsKey(material))
        {
            AvailableMaterials[material] = AvailableMaterials[material] - amount;
        }
        else
        {
            successful = false;
        }

        return successful;
    }

    public int GetMaterialCount(WorkMaterial material)
    {
        if (AvailableMaterials.ContainsKey(material))
        {
            return AvailableMaterials[material];
        }
        else
        {
            return -1;
        }
    }
}

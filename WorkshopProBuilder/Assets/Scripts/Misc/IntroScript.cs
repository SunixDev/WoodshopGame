using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IntroScript : MonoBehaviour 
{
    public GameObject ProjectToInstantiate;
    public List<GameObject> WoodPieces;

    public void StartProject()
    {
        GameObject project = Instantiate(ProjectToInstantiate);
        GameManager.instance.CurrentProject = project.GetComponent<Project>();
        foreach (GameObject wood in WoodPieces)
        {
            GameObject woodPiece = Instantiate(wood);
            GameManager.instance.WoodManager.WoodMaterials.Add(woodPiece);
            woodPiece.SetActive(false);
        }
        Application.LoadLevel("None");
    }
}

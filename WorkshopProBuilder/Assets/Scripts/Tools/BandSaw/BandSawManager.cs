using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BandSawManager : MonoBehaviour 
{
    public List<GameObject> AvailableWoodMaterial;
    public List<CutLine> LinesToCut;
    public Transform PlacementFromBlade;
    public BandSawUI UI_Manager;

    private int currentPieceIndex = 0;

	void Start () 
    {
        AvailableWoodMaterial = GameManager.instance.GetNecessaryMaterials(CutLineType.CurvatureCut);
        LinesToCut = new List<CutLine>();
        foreach (GameObject go in AvailableWoodMaterial)
        {
            WoodMaterialObject wood = go.GetComponent<WoodMaterialObject>();
            LinesToCut.AddRange(wood.RetrieveLines(CutLineType.CurvatureCut, GameManager.instance.GetStep()));
            go.SetActive(false);
        }
        if (AvailableWoodMaterial.Count > 0)
        {
            AvailableWoodMaterial[currentPieceIndex].SetActive(true);
            PlacePiece();
            UI_Manager.UpdateSelectionButtons(currentPieceIndex, AvailableWoodMaterial.Count);
        }
        else
        {
            Debug.Log("No pieces are available");
        }
	}

    public void SwitchToNextPiece()
    {
        SwitchPiece(currentPieceIndex + 1);
        UI_Manager.UpdateSelectionButtons(currentPieceIndex, AvailableWoodMaterial.Count);
    }

    public void SwitchToPreviousPiece()
    {
        SwitchPiece(currentPieceIndex - 1);
        UI_Manager.UpdateSelectionButtons(currentPieceIndex, AvailableWoodMaterial.Count);
    }

    private void SwitchPiece(int indexToSwitchTo)
    {
        AvailableWoodMaterial[currentPieceIndex].transform.position = Vector3.zero;
        AvailableWoodMaterial[currentPieceIndex].SetActive(false);
        AvailableWoodMaterial[indexToSwitchTo].SetActive(true);
        currentPieceIndex = indexToSwitchTo;
        PlacePiece();
    }

    public void PlacePiece()
    {
        AvailableWoodMaterial[currentPieceIndex].transform.position = PlacementFromBlade.position + new Vector3(0.0f, 0.0f, -3.0f);
        Ray ray = new Ray(PlacementFromBlade.position, -Vector3.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            float distance = (hit.point - PlacementFromBlade.position).magnitude;
            AvailableWoodMaterial[currentPieceIndex].transform.position += (distance * Vector3.forward);
        }
    }

    public bool AreAllLinesCut()
    {
        return (LinesToCut.Count <= 0);
    }

    public CutLine GetNearestLine(Vector3 fromPosition)
    {
        bool lineFound = false;
        int nearestLineIndex = -1;
        float smallestDistance = 0.0f;
        for (int i = 0; i < LinesToCut.Count && !lineFound; i++)
        {
            float firstDistance = Vector3.Distance(fromPosition, LinesToCut[i].Checkpoints[0].GetPosition());
            float lastDistance = Vector3.Distance(fromPosition, LinesToCut[i].Checkpoints[LinesToCut[i].Checkpoints.Count - 1].GetPosition());

            if (i == 0 || firstDistance < smallestDistance || lastDistance < smallestDistance)
            {
                nearestLineIndex = i;
                smallestDistance = (firstDistance < smallestDistance) ? firstDistance : lastDistance;
            }
        }
        return LinesToCut[nearestLineIndex];
    }
}

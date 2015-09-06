using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TableSawManager : MonoBehaviour 
{
    public List<GameObject> AvailableWoodMaterial;
    public List<CutLine> LinesToCut;
    public CutState CurrentState { get; set; }
    public Transform InitialPlacement;
    public WoodListPanel buttonPanel;

    private int currentPieceIndex = 0;
    private Transform currentSpawnPoint;

	void Start () 
    {
        AvailableWoodMaterial = GameManager.instance.GetNecessaryMaterials(CutLineType.TableSawCut);
        LinesToCut = new List<CutLine>();
        foreach (GameObject go in AvailableWoodMaterial)
        {
            WoodMaterialObject wood = go.GetComponent<WoodMaterialObject>();
            LinesToCut.AddRange(wood.RetrieveLines(CutLineType.TableSawCut, GameManager.instance.GetStep()));
            buttonPanel.AddWoodMaterialButton(wood.gameObject.name);
            go.SetActive(false);
        }
        CurrentState = CutState.ReadyToCut;
        AvailableWoodMaterial[currentPieceIndex].SetActive(true);
        //AvailableWoodMaterial[currentPieceIndex].transform.position = InitialPlacement.position;
        //currentSpawnPoint = InitialPlacement;
	}

    public void SwitchPiece(int index)
    {
        AvailableWoodMaterial[currentPieceIndex].transform.position = Vector3.zero;
        AvailableWoodMaterial[currentPieceIndex].SetActive(false);
        currentPieceIndex = index;
        AvailableWoodMaterial[currentPieceIndex].SetActive(true);
        AvailableWoodMaterial[currentPieceIndex].transform.position = currentSpawnPoint.position;
    }

    public void SwitchSpawnPoint(Transform spawn)
    {
        currentSpawnPoint = spawn;
        AvailableWoodMaterial[currentPieceIndex].transform.position = currentSpawnPoint.position;
    }

    public bool CurrentStateIs(CutState state)
    {
        return CurrentState == state;
    }

    public void UpdateState(CutState nextState)
    {
        CurrentState = nextState;
    }

    public CutLine GetNearestLine(Vector3 fromPosition)
    {
        bool lineFound = false;
        int nearestLineIndex = -1;
        float smallestDistance = 0.0f;
        for (int i = 0; i < LinesToCut.Count && !lineFound; i++)
        {
            if (nearestLineIndex == -1)
            {
                nearestLineIndex = i;
                float firstDistance = Vector3.Distance(fromPosition, LinesToCut[i].Checkpoints[0].GetPosition());
                float lastDistance = Vector3.Distance(fromPosition, LinesToCut[i].Checkpoints[LinesToCut[i].Checkpoints.Count - 1].GetPosition());
                if (firstDistance < lastDistance)
                {
                    smallestDistance = firstDistance;
                    LinesToCut[i].CutBackwards = false;
                }
                else
                {
                    smallestDistance = lastDistance;
                    LinesToCut[i].CutBackwards = true;
                }

            }
            else
            {
                float firstDistance = Vector3.Distance(fromPosition, LinesToCut[i].Checkpoints[0].GetPosition());
                float lastDistance = Vector3.Distance(fromPosition, LinesToCut[i].Checkpoints[LinesToCut[i].Checkpoints.Count - 1].GetPosition());
                if (firstDistance < lastDistance)
                {
                    if (firstDistance < smallestDistance)
                    {
                        nearestLineIndex = i;
                        smallestDistance = firstDistance;
                        LinesToCut[i].CutBackwards = false;
                    }
                }
                else
                {
                    if (lastDistance < smallestDistance)
                    {
                        nearestLineIndex = i;
                        smallestDistance = lastDistance;
                        LinesToCut[i].CutBackwards = true;
                    }
                }
            }
        }
        return LinesToCut[nearestLineIndex];
    }
}

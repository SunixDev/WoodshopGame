using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChopSawManager : MonoBehaviour 
{
    public List<GameObject> AvailableWoodMaterial;
    public List<CutLine> LinesToCut;
    public Transform InitialPlacementFromBlade;
    public ChopSawUI UI_Manager;

    private int currentPieceIndex = 0;
    private Transform currentSpawnPoint;
    private ActionState currentAction = ActionState.OnSaw;
    private ActionState previousAction = ActionState.None;
    private bool switchingPieces = false;

	void Start () 
    {
        AvailableWoodMaterial = GameManager.instance.GetNecessaryMaterials(CutLineType.ChopSawCut);
        LinesToCut = new List<CutLine>();
        foreach (GameObject go in AvailableWoodMaterial)
        {
            go.transform.Rotate(Vector3.up, 90, Space.World);
            WoodMaterialObject wood = go.GetComponent<WoodMaterialObject>();
            LinesToCut.AddRange(wood.RetrieveLines(CutLineType.ChopSawCut, GameManager.instance.GetStep()));
            go.SetActive(false);
        }
        AvailableWoodMaterial[currentPieceIndex].SetActive(true);
        currentSpawnPoint = InitialPlacementFromBlade;
        PlacePiece();
        UI_Manager.UpdateSelectionButtons(currentPieceIndex, AvailableWoodMaterial.Count);
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
        switchingPieces = true;
        AvailableWoodMaterial[currentPieceIndex].transform.position = Vector3.zero;
        AvailableWoodMaterial[currentPieceIndex].SetActive(false);
        AvailableWoodMaterial[indexToSwitchTo].SetActive(true);
        currentPieceIndex = indexToSwitchTo;
        PlacePiece();
    }

    public void SwitchSpawnPoint(Transform spawnPoint)
    {
        if (currentSpawnPoint != spawnPoint)
        {
            currentSpawnPoint = spawnPoint;
        }
    }

    public void SwitchAction(string actionState)
    {
        ActionState newState = (ActionState)System.Enum.Parse(typeof(ActionState), actionState);
        if (currentAction != newState)
        {
            previousAction = currentAction;
            currentAction = newState;
        }
    }

    public void PlacePiece()
    {
        if (switchingPieces || previousAction != currentAction)
        {
            if (currentAction == ActionState.UsingRuler)
            {
                AvailableWoodMaterial[currentPieceIndex].transform.position = currentSpawnPoint.position + new Vector3(0.0f, 0.0f, -3.0f);
                Ray ray = new Ray(currentSpawnPoint.position, -Vector3.forward);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    float distance = (hit.point - currentSpawnPoint.position).magnitude;
                    AvailableWoodMaterial[currentPieceIndex].transform.position += (distance * Vector3.forward);
                }
            }
            else if (currentAction == ActionState.OnSaw)
            {
                AvailableWoodMaterial[currentPieceIndex].transform.position = currentSpawnPoint.position + new Vector3(3.0f, 0.0f, 0.0f);
                Ray ray = new Ray(currentSpawnPoint.position, Vector3.right);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    float distance = (hit.point - currentSpawnPoint.position).magnitude;
                    AvailableWoodMaterial[currentPieceIndex].transform.position += (distance * -Vector3.right);
                }
            }
            previousAction = currentAction;
            switchingPieces = false;
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

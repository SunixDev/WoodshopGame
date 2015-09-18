using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum DadoCutActionState
{
    UsingSaw,
    PreppingTools,
    SettingUpWood,
    None
}

public class DadoCutManager : MonoBehaviour 
{
    public List<GameObject> AvailableWoodMaterial;
    public List<DadoBlock> DadosToCut;
    public Transform InitialPlacementFromBlade;
    public TableSawUI UI_Manager;

    private int currentPieceIndex = 0;
    private Transform currentSpawnPoint;
    private DadoCutActionState currentAction = DadoCutActionState.UsingSaw;
    private DadoCutActionState previousAction = DadoCutActionState.None;
    private bool switchingPieces = false;

	void Start () 
    {
        //AvailableWoodMaterial = GameManager.instance.GetNecessaryMaterials(CutLineType.TableSawCut);
        //LinesToCut = new List<CutLine>();
        //foreach (GameObject go in AvailableWoodMaterial)
        //{
        //    WoodMaterialObject wood = go.GetComponent<WoodMaterialObject>();
        //    LinesToCut.AddRange(wood.RetrieveLines(CutLineType.TableSawCut, GameManager.instance.GetStep()));
        //    go.SetActive(false);
        //}
        //AvailableWoodMaterial[currentPieceIndex].SetActive(true);
        //currentSpawnPoint = InitialPlacementFromBlade;
        //PlacePiece();
        //UI_Manager.UpdateSelectionButtons(currentPieceIndex, AvailableWoodMaterial.Count);
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
        DadoCutActionState newState = (DadoCutActionState)System.Enum.Parse(typeof(DadoCutActionState), actionState);
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
            AvailableWoodMaterial[currentPieceIndex].transform.position = currentSpawnPoint.position + new Vector3(0.0f, 0.0f, -3.0f);
            Ray ray = new Ray(currentSpawnPoint.position, -Vector3.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                float distance = (hit.point - currentSpawnPoint.position).magnitude;
                AvailableWoodMaterial[currentPieceIndex].transform.position += (distance * Vector3.forward);
            }
            previousAction = currentAction;
            switchingPieces = false;
        }
    }
}

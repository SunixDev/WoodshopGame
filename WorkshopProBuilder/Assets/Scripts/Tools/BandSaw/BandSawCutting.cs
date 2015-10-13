using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BandSawCutting : MonoBehaviour 
{
    public BandSawManager manager;
    public BandSawBlade Blade;
    public float ValidCutOffset = 0.005f;
    public Color SelectedLineColor;
    public Color NotSelectedLineColor;
    public CutState CurrentState { get; set; }

    private CutLine currentLine;
    private Vector3 previousBoardPosition;
    private float timeStalling;
    private float lineScore = 100.0f;

    private float totalTimeNotCuttingLine = 0.0f;
    private float MaxTimeAwayFromLine = 2.0f;

    private float totalTimePassed = 0.0f;
    private float timeUpdateFrequency = 0.1f;

	void Start () 
    {
        currentLine = null;
        timeStalling = 0.0f;
        CurrentState = CutState.ReadyToCut;
	}

    private void SwitchLine()
    {
        CutLine nearestLine = manager.GetNearestLine(Blade.transform.position);
        nearestLine.ChangeColor(SelectedLineColor);
        if (currentLine != null && currentLine != nearestLine)
        {
            currentLine.ResetColor(NotSelectedLineColor);
        }
        currentLine = nearestLine;
    }

    private void StartWoodCutting()
    {
        currentLine.DetermineCutDirection(Blade.transform.position);
        previousBoardPosition = manager.GetCurrentBoardPosition();
        CurrentState = CutState.Cutting;
        manager.SetUpBoardForCutting(true);
    }
	
	void Update () 
    {
        #region CuttingCode
        if (manager.StillCutting)
        {
            totalTimePassed += Time.deltaTime;
            if (CurrentState == CutState.ReadyToCut)
            {
                SwitchLine();

                if (Blade.CuttingWoodBoard && Blade.SawActive)
                {
                    Vector3 origin = Blade.transform.position + new Vector3(0.0f, 0.1f, 0.0f);
                    Ray ray = new Ray(origin, Vector3.down);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit) && (hit.collider.tag == "Piece" || hit.collider.tag == "Leftover" || hit.collider.tag == "Dado"))
                    {
                        hit.collider.gameObject.transform.parent.GetComponent<BandSawPieceController>().RotationPoint = hit.point;
                        StartWoodCutting();
                    }
                }
            }
            else if (CurrentState == CutState.Cutting && Blade.SawActive)
            {
                float distanceFromBlade = currentLine.CalculateDistance(Blade.transform.position);
                bool cuttingAlongLine = (distanceFromBlade <= ValidCutOffset);
                if (cuttingAlongLine)
                {
                    currentLine.UpdateLine(Blade.transform.position, ValidCutOffset);
                    if (currentLine.LineIsCut())
                    {
                        CurrentState = CutState.EndOfCut;
                    }
                    if (totalTimePassed >= timeUpdateFrequency)
                    {
                        totalTimePassed = 0.0f;
                        if (distanceFromBlade <= ValidCutOffset && distanceFromBlade >= 0.015f)
                        {
                            lineScore -= 0.2f;
                        }
                        else if (distanceFromBlade <= 0.015f && distanceFromBlade >= 0.01f)
                        {
                            lineScore -= 0.1f;
                        }
                    }
                }
                else
                {
                    if (totalTimePassed >= timeUpdateFrequency)
                    {
                        totalTimePassed = 0.0f;
                        lineScore -= 0.5f;
                    }
                    totalTimeNotCuttingLine += Time.deltaTime;
                    if (totalTimeNotCuttingLine >= MaxTimeAwayFromLine)
                    {
                        manager.StopGameDueToLowScore("You've messed up the wood too much.");
                    }
                    else if (Blade.NoInteractionWithBoard)
                    {
                        CurrentState = CutState.ReadyToCut;
                        Blade.ResetEdgePosition();
                        currentLine = null;
                        manager.SetUpBoardForCutting(false);
                    }
                }
            }
            else if (CurrentState == CutState.EndOfCut)
            {
                if (!Blade.CuttingWoodBoard && Blade.NoInteractionWithBoard)
                {
                    Debug.Log("lineScore: " + lineScore);
                    manager.DisplayScore(lineScore);
                    lineScore = 100.0f;
                    manager.SetUpBoardForCutting(false);
                    manager.SplitMaterial(currentLine);
                    currentLine = null;
                    Blade.ResetEdgePosition();
                    CurrentState = CutState.ReadyToCut;
                }
            }

            if (lineScore <= 0.0f)
            {
                manager.StopGameDueToLowScore("This cut is too messed up to keep going.");
            }

            if (totalTimePassed >= timeUpdateFrequency)
            {
                totalTimePassed = 0.0f;
            }
        }
        #endregion
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BandSawCutting : MonoBehaviour 
{
    public BandSawManager manager;
    public BandSawBlade Blade;
    public float ValidCutOffset = 0.005f;
    public float MaxStallTime = 3.0f;
    //public float PushRateOffset = 0.001;
    public Color SelectedLineColor;
    public Color NotSelectedLineColor;
    public CutState CurrentState { get; set; }

    private CutLine currentLine;
    private bool cuttingAlongLine;
    private Vector3 previousBoardPosition;
    private float timeStalling;

	void Start () 
    {
        currentLine = null;
        cuttingAlongLine = false;
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

        float distanceFromBlade = currentLine.CalculateDistance(Blade.transform.position);
        Debug.Log("distanceFromBlade: " + distanceFromBlade);
        cuttingAlongLine = (distanceFromBlade <= ValidCutOffset);

        previousBoardPosition = manager.GetCurrentBoardPosition();
        CurrentState = CutState.Cutting;
    }

    private float TrackPushRate()
    {
        Vector3 currentPosition = manager.GetCurrentBoardPosition();
        Vector3 deltaVector = currentPosition - previousBoardPosition;
        previousBoardPosition = currentPosition;
        return deltaVector.magnitude;
    }
	
	void Update () 
    {
        #region CuttingCode
        if (manager.WoodToCut.Count > 0)
        {
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
                        StartWoodCutting();
                    }
                }
            }
            else if (CurrentState == CutState.Cutting && Blade.SawActive)
            {
                if (cuttingAlongLine)
                {
                    currentLine.UpdateLine(Blade.transform.position);
                    if (currentLine.LineIsCut())
                    {
                        CurrentState = CutState.EndOfCut;
                    }
                    else
                    {
                        float pushRate = TrackPushRate();
                        //Debug.Log("Push Rate: " + (pushRate * 100));
                        timeStalling = 0.0f;
                        //Calculate push rate is within consistent rate
                        //Lose points if too slow or too fast
                    }
                }
                else
                {
                    //Decrease value by certain amount until zero and need to start over
                    if (Blade.NoInteractionWithBoard)
                    {
                        CurrentState = CutState.ReadyToCut;
                        Blade.ResetEdgePosition();
                        currentLine = null;
                    }
                }
            }
            else if (CurrentState == CutState.EndOfCut)
            {
                if (!Blade.CuttingWoodBoard && Blade.NoInteractionWithBoard)
                {
                    manager.SplitMaterial(currentLine);
                    cuttingAlongLine = false;
                    currentLine = null;
                    Blade.ResetEdgePosition();
                    CurrentState = CutState.ReadyToCut;
                }
            }
        }
        #endregion
	}
}

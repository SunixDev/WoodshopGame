using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SnapPieceGameplay : MonoBehaviour 
{
    public WoodPiece CurrentPiece;
    public List<WoodPiece> PlacedPieces = new List<WoodPiece>();
    public SnapPieceGameManager manager;
    public float ValidConnectionDistance = 0.1f;

    public void SetUp(WoodPiece placedPiece, WoodPiece currentPiece)
    {
        PlacedPieces.Add(placedPiece);
        CurrentPiece = currentPiece;
    }
	
	void Update () 
    {
        //foreach (WoodPiece placedPiece in PlacedPieces)
        //{
        //    foreach (SnapPoint point in placedPiece.SnapPoints)
        //    {
        //        foreach (SnapPoint currentPoint in CurrentPiece.SnapPoints)
        //        {
        //            if (currentPoint.CanConnectTo(point) && currentPoint.DistanceFromPoint(point) <= ValidConnectionDistance)
        //            {
        //                currentPoint.ConnectToPoint(point);
        //                PlacedPieces.Add(CurrentPiece);
        //                manager.SwitchFromPiece(CurrentPiece);
        //                break;
        //            }
        //        }
        //    }
        //}
	}
}

using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class Connection 
{
    public Node FirstPiece;
    public Node SecondPiece;

    public void Disconnect()
    {
        FirstPiece.SeverConnection(SecondPiece);
        SecondPiece.SeverConnection(FirstPiece);
    }
}

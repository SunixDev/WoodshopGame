using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WoodProject : MonoBehaviour 
{
    public List<WoodPiece> ConnectecPieces;

    private Bounds projectBounds;

    public bool RequiresGlue
    {
        get
        {
            bool glueRequired = false;
            for (int i = 0; i < ConnectecPieces.Count && !glueRequired; i++)
            {
                glueRequired = ConnectecPieces[i].RequiresGlue;
            }
            return glueRequired;
        }
    }

    void Awake()
    {
        projectBounds = new Bounds();
        if (ConnectecPieces == null)
        {
            ConnectecPieces = new List<WoodPiece>();
        }
        else
        {
            for (int i = 0; i < ConnectecPieces.Count && ConnectecPieces.Count > 0; i++)
            {
                ConnectecPieces[i].gameObject.transform.SetParent(transform);
                projectBounds.Encapsulate(ConnectecPieces[i].gameObject.GetComponent<Renderer>().bounds);
            }
        }
    }

    public void AddPieceToConnect(GameObject woodPieceObject)
    {
        WoodPiece woodPiece = woodPieceObject.GetComponent<WoodPiece>();
        if(woodPiece != null)
        {
            ConnectecPieces.Add(woodPiece);
            woodPiece.transform.SetParent(transform);
            projectBounds.Encapsulate(woodPieceObject.GetComponent<Renderer>().bounds);
        }
    }

    public Bounds GetBounds()
    {
        return projectBounds;
    }
}

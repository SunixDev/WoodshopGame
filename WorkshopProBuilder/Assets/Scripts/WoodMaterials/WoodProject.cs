using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WoodProject : MonoBehaviour 
{
    public List<WoodPiece> ConnectedPieces;
    public bool displayBounds;

    private Bounds projectBounds;

    public bool RequiresGlue
    {
        get
        {
            bool glueRequired = false;
            for (int i = 0; i < ConnectedPieces.Count && !glueRequired; i++)
            {
                glueRequired = ConnectedPieces[i].RequiresGlue;
            }
            return glueRequired;
        }
    }

    void Awake()
    {
        projectBounds = new Bounds();
        if (ConnectedPieces == null)
        {
            ConnectedPieces = new List<WoodPiece>();
        }
        else
        {
            Vector3 previousPosition = transform.position;
            transform.position = Vector3.zero;
            for (int i = 0; i < ConnectedPieces.Count && ConnectedPieces.Count > 0; i++)
            {
                ConnectedPieces[i].gameObject.transform.SetParent(transform);
                projectBounds.Encapsulate(ConnectedPieces[i].gameObject.GetComponent<Renderer>().bounds);
            }
            transform.position = previousPosition;
        }
    }

    public void AddPieceToConnect(GameObject woodPieceObject)
    {
        WoodPiece woodPiece = woodPieceObject.GetComponent<WoodPiece>();
        if(woodPiece != null)
        {
            ConnectedPieces.Add(woodPiece);
            woodPiece.transform.SetParent(transform);
            EncapsulateBounds(woodPieceObject.GetComponent<Renderer>());
        }
    }

    public Bounds GetBounds()
    {
        return projectBounds;
    }

    private void EncapsulateBounds(Renderer pieceRenderer)
    {
        Vector3 previousPosition = transform.position;
        transform.position = Vector3.zero;
        projectBounds.Encapsulate(pieceRenderer.bounds);
        transform.position = previousPosition;
    }



    void OnDrawGizmos()
    {
        if (displayBounds)
        {
            Vector3 center = projectBounds.center;
            float radius = projectBounds.extents.magnitude;
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(center, radius);
            float x = projectBounds.center.x + projectBounds.size.x;
            float y = projectBounds.center.y + projectBounds.size.y;
            float z = projectBounds.center.z + projectBounds.size.z;
            Gizmos.DrawWireCube(center, new Vector3(x, y, z));
        }
    }
}

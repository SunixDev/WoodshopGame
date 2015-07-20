using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DisplayCutLine : MonoBehaviour 
{
    public GameObject piece;
    public GameObject[] leftovers;

    private List<Vector3> lineVerts;

    void Start() 
    {
        List<Vector3[]> leftoversMeshVerts = new List<Vector3[]>();
        foreach (GameObject leftover in leftovers)
        {
            leftoversMeshVerts.Add(leftover.GetComponent<MeshFilter>().mesh.vertices);
        }
        Vector3[] pieceVerts = piece.GetComponent<MeshFilter>().mesh.vertices;

        lineVerts = new List<Vector3>();
        Transform pieceTransform = piece.transform;
        for ( int i = 0; i < leftoversMeshVerts.Count; i++ )
        {
            Vector3[] leftoverVerts = leftoversMeshVerts[i];
            for (int vert = 0; vert < leftoverVerts.Length; vert++)
            {
                for (int pieceVert = 0; pieceVert < pieceVerts.Length; pieceVert++)
                {
                    if (pieceVerts[pieceVert].y > 0 && leftoverVerts[vert].y > 0)
                    {
                        Vector3 pieceLocation = pieceTransform.TransformPoint(pieceVerts[pieceVert]);
                        Vector3 leftoverLocation = leftovers[i].transform.TransformPoint(leftoverVerts[vert]);
                        float distance = Vector3.Distance(pieceLocation, leftoverLocation);

                        if (distance <= 0.0001f && distance >= 0 && !lineVerts.Contains(pieceVerts[pieceVert]))
                        {
                            lineVerts.Add(pieceVerts[pieceVert]);
                        }
                    }
                }
            }
        }
    }

    void Update()
    {
        for (int i = 0; i < lineVerts.Count; i++)
        {
            int connectionIndex = (i < lineVerts.Count - 1) ? i + 1 : 0;
            Vector3 pointLocation = piece.transform.TransformPoint(lineVerts[i]);
            Vector3 connectionLocation = piece.transform.TransformPoint(lineVerts[connectionIndex]);
            Debug.DrawLine(pointLocation, connectionLocation, Color.red);
        }
    }
}

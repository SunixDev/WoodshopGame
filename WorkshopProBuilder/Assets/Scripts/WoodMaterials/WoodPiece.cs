using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshCollider))]
public class WoodPiece : MonoBehaviour
{
    public string Name;
    public Texture WoodTexture;
    public List<SnapPoint> SnapPoints;
    public List<ClampPoint> ClampPoints;
    public List<GlueBox> GlueBoxes;
    public MeshCollider objMeshCollider;

    public Vector3 Position
    {
        get
        {
            return transform.position;
        }
    }

    public void ChangeMaterial(Material newMaterial)
    {
        GetComponent<Renderer>().material = newMaterial;
        GetComponent<Renderer>().material.mainTexture = WoodTexture;
    }

    public void EnableConvexCollider(bool enable)
    {
        if (objMeshCollider != null)
        {
            objMeshCollider.convex = enable;
        }
    }
}

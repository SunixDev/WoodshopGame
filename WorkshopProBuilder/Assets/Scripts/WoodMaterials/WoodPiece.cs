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
    public bool CanConnect
    {
        get
        {
            if (GlueBoxes.Count <= 0)
            {
                return true;
            }
            else
            {
                bool canConnect = true;
                for (int i = 0; i < GlueBoxes.Count && canConnect; i++)
                {
                    canConnect = GlueBoxes[i].ReadyToConnect;
                }
                return canConnect;
            }
        }
    }

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

    public List<GlueBox> ActivateGlueBoxes(int stepNumber)
    {
        List<GlueBox> activatedGlueBoxes = new List<GlueBox>();
        foreach (GlueBox glue in GlueBoxes)
        {
            StepID id = glue.GetComponent<StepID>();
            if (id.UsedInStep(stepNumber))
            {
                glue.gameObject.GetComponent<BoxCollider>().enabled = true;
                activatedGlueBoxes.Add(glue);
            }
        }
        return activatedGlueBoxes;
    }

    public List<SnapPoint> ActivateSnapPoints(int stepNumber)
    {
        List<SnapPoint> activatedSnapPoints = new List<SnapPoint>();
        foreach (SnapPoint snapPoint in SnapPoints)
        {
            StepID id = snapPoint.GetComponent<StepID>();
            if (id.UsedInStep(stepNumber))
            {
                //snapPoint.isActive = true;
                activatedSnapPoints.Add(snapPoint);
            }
        }
        return activatedSnapPoints;
    }
}

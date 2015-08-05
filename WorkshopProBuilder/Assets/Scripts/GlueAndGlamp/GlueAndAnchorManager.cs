using UnityEngine;
using System.Collections;

public class GlueAndAnchorManager : MonoBehaviour 
{
    public GlueBox GlueArea;
    public AnchorPoint Anchor;
	
	void Update () 
    {
        if (GlueArea.IsMinimumGlueAmountReached())
        {
            Anchor.CanConnect = true;
        }
	}
}

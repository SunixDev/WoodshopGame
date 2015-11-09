using UnityEngine;
using System.Collections;

public class BoundsRenderer : MonoBehaviour 
{
    private Renderer rend;
    void Start()
    {
        rend = GetComponent<Renderer>();
    }
    void OnDrawGizmosSelected()
    {
        if (rend != null)
        {
            Vector3 center = rend.bounds.center;
            float radius = rend.bounds.extents.magnitude;
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(center, radius);
        }
    }
}

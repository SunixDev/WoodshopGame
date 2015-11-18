using UnityEngine;
using System.Collections;

public class BoundsWithChildren : MonoBehaviour 
{
    public bool displayBounds = false;
    private Bounds objBounds;

	void Start () 
    {
        objBounds = new Bounds();
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            Renderer r = child.gameObject.GetComponent<Renderer>();
            if (r != null)
            {
                objBounds.Encapsulate(r.bounds);
            }
        }
	}

    void OnDrawGizmos()
    {
        if (displayBounds)
        {
            Vector3 center = objBounds.center;
            float radius = objBounds.extents.magnitude;
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(center, radius);
            float x = objBounds.center.x + objBounds.size.x;
            float y = objBounds.center.y + objBounds.size.y;
            float z = objBounds.center.z + objBounds.size.z;
            Gizmos.DrawWireCube(center, new Vector3(x, y, z));
        }
    }
}

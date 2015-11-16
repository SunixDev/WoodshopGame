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
            Gizmos.DrawWireCube(center, new Vector3(objBounds.center.x + objBounds.size.x, objBounds.center.y + objBounds.size.y, objBounds.center.z + objBounds.size.z));
            Debug.DrawRay(objBounds.center, Vector3.down, Color.blue);
        }
    }
}

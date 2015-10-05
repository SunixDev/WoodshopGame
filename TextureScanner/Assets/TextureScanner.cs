using UnityEngine;
using System.Collections;

public class TextureScanner : MonoBehaviour 
{
    public Texture2D texture;
    private Mesh objMesh;

	void Start () 
    {
        StartCoroutine(Scan());
	}

    void Update() 
    {
	
	}

    IEnumerator Scan()
    {
        Vector2[] objUVs = GetComponent<MeshFilter>().mesh.uv;
        Debug.Log("# of UV coordinates: " + objUVs.Length);
        for (int i = 0; i < objUVs.Length; i++)
        {
            //Debug.Log("UV Coordinate " + i + ": " + objUVs[i]);
            int texelX = Mathf.FloorToInt(objUVs[i].x * texture.width);
            int texelY = Mathf.FloorToInt(objUVs[i].y * texture.height);
            Debug.Log("Color: " + texture.GetPixel(texelX, texelY));
            yield return null;
        }
        Debug.Log("Total Time: " + Time.time);
    }
}

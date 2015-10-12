using UnityEngine;
using System.Collections;

public class TextureScanner : MonoBehaviour {

    public Texture2D texture { get; set; }
    public MeshFilter objMeshFilter { get; set; }
    public Color ColorToCompare { get; set; }

    private bool scanningComplete = false;
    private float TotalCorrectUVColors;

    public void StartScanning()
    {
        StartCoroutine(Scan());
    }

    public bool ScanIsComplete()
    {
        return scanningComplete;
    }

    public float GetPercentageCorrect()
    {
        Vector2[] objUVs = objMeshFilter.mesh.uv;
        int TotalUVs = objUVs.Length;
        float crossMultiple = TotalCorrectUVColors * 100.0f;
        float percentageResult = crossMultiple / TotalUVs;
        return percentageResult;
    }

    IEnumerator Scan()
    {
        Vector2[] objUVs = objMeshFilter.mesh.uv;
        for (int i = 0; i < objUVs.Length; i++)
        {
            int texelX = Mathf.FloorToInt(objUVs[i].x * texture.width);
            int texelY = Mathf.FloorToInt(objUVs[i].y * texture.height);
            Color color = texture.GetPixel(texelX, texelY);
            if (SameColor(color))
            {
                TotalCorrectUVColors++;
            }
            yield return null;
        }
        scanningComplete = true;
    }

    private bool SameColor(Color pixelColor)
    {
        return (ColorToCompare.r == pixelColor.r && 
                ColorToCompare.g == pixelColor.g && 
                ColorToCompare.b == pixelColor.b);
    }
}

using UnityEngine;
using System.Collections;

public class TextureScanner : MonoBehaviour {

    public Texture2D texture { get; set; }
    public MeshFilter objMeshFilter { get; set; }

    private Color ColorToCompare = new Color(1f, 1f, 1f);
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
            if (DifferentFromWhite(color))
            {
                TotalCorrectUVColors++;
            }
            if(i % 5 == 0)
            {
                yield return null;
            }
        }
        scanningComplete = true;
    }

    private bool DifferentFromWhite(Color pixelColor)
    {
        return (pixelColor.r < ColorToCompare.r &&
                pixelColor.g < ColorToCompare.g &&
                pixelColor.b < ColorToCompare.b);

        //return (ColorToCompare.r != pixelColor.r &&
        //        ColorToCompare.g != pixelColor.g &&
        //        ColorToCompare.b != pixelColor.b);
    }
}

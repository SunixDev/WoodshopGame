using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor;

public enum SwipeGameType
{
    Paint,
    Sanding,
    Lacquer
}

public class SwipeManager : MonoBehaviour 
{
    public SwipeGameType type = SwipeGameType.Paint;
    public Camera RenderCamera;
    public GameObject CurrentPiece;
    public Transform SwipeArea;
    public Sprite SwipeBrush;
    public RenderTexture SwipeRenderTexture;
    public Material SwipeBackgroundMaterial;
    public Color BrushColor;

    [Range(100, 1000)]
    public int NumberOfBrushes = 500;

    [Range(0.01f, 0.05f)]
    public float BrushSize = 0.03f;

    private List<GameObject> SwipeBrushStrokes = new List<GameObject>();
    private int BrushIndex = 0;
    private Vector3 PreviousHitLocation = new Vector3(-10.0f, -10.0f, -10.0f);
    private bool CorrectMaterial;

	void Start () 
    {
        CheckPieceMaterial();
        if (CorrectMaterial)
        {
            if (type != SwipeGameType.Paint)
            {
                BrushColor = Color.black;
            }

            for (int i = 0; i < NumberOfBrushes; i++)
            {
                GameObject brush = new GameObject("Brush_" + i);
                brush.transform.parent = SwipeArea;
                brush.transform.localPosition = Vector3.zero;
                brush.transform.localScale = Vector3.one * BrushSize;

                SpriteRenderer renderer = brush.AddComponent<SpriteRenderer>();
                renderer.sprite = SwipeBrush;
                renderer.color = BrushColor;
                renderer.enabled = false;
                SwipeBrushStrokes.Add(brush);
            }
        }
        else
        {
            Debug.LogError("CurrentPiece in SwipeManager has the wrong material");
        }
	}
	
	void Update () 
    {
        if (CorrectMaterial)
        {
            if (type != SwipeGameType.Paint)
            {
                BrushColor = Color.black;
            }

            if (Input.GetMouseButton(0))
            {
                RaycastHit hit;
                Vector3 cursor = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f);
                Ray ray = Camera.main.ScreenPointToRay(cursor);

                if (Physics.Raycast(ray, out hit) && hit.transform.name == CurrentPiece.name && PreviousHitLocation != hit.point)
                {
                    Vector2 uvs = hit.textureCoord;
                    Vector3 paintUVPosition = new Vector3(0.0f, uvs.y - RenderCamera.orthographicSize,
                                                                uvs.x - RenderCamera.orthographicSize - 0.05f);

                    BrushColor.a = BrushSize * 2.0f;
                    SwipeBrushStrokes[BrushIndex].GetComponent<SpriteRenderer>().enabled = true;
                    SwipeBrushStrokes[BrushIndex].GetComponent<SpriteRenderer>().color = BrushColor;
                    SwipeBrushStrokes[BrushIndex].transform.localPosition = paintUVPosition;
                    SwipeBrushStrokes[BrushIndex].transform.localScale = Vector3.one * BrushSize;
                    PreviousHitLocation = hit.point;
                    BrushIndex++;
                }

                if (BrushIndex >= NumberOfBrushes)
                {
                    BrushIndex = 0;
                    RenderTexture.active = SwipeRenderTexture;
                    Texture2D tex = new Texture2D(SwipeRenderTexture.width, SwipeRenderTexture.height, TextureFormat.RGB24, false);
                    tex.ReadPixels(new Rect(0, 0, SwipeRenderTexture.width, SwipeRenderTexture.height), 0, 0);
                    tex.Apply();
                    RenderTexture.active = null;
                    SwipeBackgroundMaterial.mainTexture = tex;
                    foreach (GameObject brush in SwipeBrushStrokes)
                    {
                        brush.GetComponent<SpriteRenderer>().enabled = false;
                    }
                }
            }
        }
	}

    private void CheckPieceMaterial()
    {
        string material = CurrentPiece.GetComponent<Renderer>().material.name;
        CorrectMaterial = false;
        if (type == SwipeGameType.Paint)
        {
            CorrectMaterial = (material.Contains("Painting"));
        }
        else if (type == SwipeGameType.Sanding)
        {
            CorrectMaterial = (material.Contains("Sanding"));
        }
        else if (type == SwipeGameType.Lacquer)
        {
            CorrectMaterial = (material.Contains("Shining"));
        }
    }
}

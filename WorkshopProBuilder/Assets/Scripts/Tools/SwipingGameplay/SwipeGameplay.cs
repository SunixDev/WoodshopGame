using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum SwipeGameType
{
    Paint,
    Sanding,
    Lacquer
}
public class SwipeGameplay : MonoBehaviour 
{
    public SwipeGameType type = SwipeGameType.Paint;
    public Camera RenderCamera;
    public GameObject CurrentPiece;
    public Transform SwipeArea;
    public Sprite SwipeBrush;
    public RenderTexture SwipeRenderTexture;
    public Material SwipeBackgroundMaterial;
    public Color BrushColor;
    public Texture2D WhiteBackground;
    public HSVPicker colorPicker;

    [Range(100, 1000)]
    public int NumberOfBrushes = 500;

    [Range(0.001f, 0.05f)]
    public float BrushSize = 0.03f;

    public bool SwipeEnabled { get; set; }

    private List<GameObject> SwipeBrushStrokes = new List<GameObject>();
    private int BrushIndex = 0;
    private Vector3 PreviousHitLocation = new Vector3(-10.0f, -10.0f, -10.0f);
    private bool CorrectMaterial = false;

    public void Setup()
    {
        if (type != SwipeGameType.Paint)
        {
            BrushColor = Color.black;
        }
        else
        {
            colorPicker.onValueChanged.AddListener(color =>
            {
                BrushColor = color;
            });
            BrushColor = colorPicker.currentColor;
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
        SwipeEnabled = true;
    }

    void Update()
    {
        if (Input.GetMouseButton(0) && SwipeEnabled)
        {
            RaycastHit hit;
            Vector3 cursor = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f);
            Ray ray = Camera.main.ScreenPointToRay(cursor);

            if (Physics.Raycast(ray, out hit) && hit.transform.name == CurrentPiece.name && PreviousHitLocation != hit.point)
            {
                Vector2 uvs = hit.textureCoord;
                Vector3 paintUVPosition = new Vector3(0.0f, uvs.y - RenderCamera.orthographicSize,
                                                            uvs.x - RenderCamera.orthographicSize - 0.05f);

                BrushColor.a = BrushSize * 10.0f; //1.0f;
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

    public Texture2D GetPaintedTexture()
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
        return tex;
    }

    public void ResetSwipeBackgroundTexture()
    {
        Texture2D resetTex = Instantiate(WhiteBackground);
        SwipeBackgroundMaterial.mainTexture = resetTex;
    }

    private void CheckPieceMaterial()
    {
        string material = CurrentPiece.GetComponent<Renderer>().material.name;
        CorrectMaterial = (type == SwipeGameType.Paint && material.Contains("Painting"));

        if (!CorrectMaterial)
        {
            CorrectMaterial = (type == SwipeGameType.Sanding && material.Contains("Sanding"));
        }

        if (!CorrectMaterial)
        {
            CorrectMaterial = (type == SwipeGameType.Lacquer && material.Contains("Shining"));
        }
    }
}

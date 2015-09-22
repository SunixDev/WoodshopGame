using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LinePrac : MonoBehaviour 
{
    public Color lineColor;
    public RectTransform lineImageRectTransform;
    public float lineWidth;

    private int numOfClicks;
    private Vector3 startPoint;
    private Vector3 endPoint;

	// Use this for initialization
	void Start () 
    {
        Image image = GetComponent<Image>();
        if (image == null)
        {
            image = gameObject.AddComponent<Image>();
        }
        image.color = lineColor;
        numOfClicks = 0;
	}
	
	void Update ()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (numOfClicks == 0)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    numOfClicks++;
                    startPoint = hit.point;
                    Debug.Log("Start Point Hit Normal: " + hit.normal);
                }
            }
            else if (numOfClicks == 1)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    numOfClicks = 0;
                    endPoint = hit.point;
                    Debug.Log("End Point Hit Normal: " + hit.normal);
                    PlaceLine(startPoint, endPoint);
                }
            }
        }
    }

    public void PlaceLine(Vector3 start, Vector3 end)
    {
        Vector3 differenceVector = end - start;

        lineImageRectTransform.sizeDelta = new Vector2(differenceVector.magnitude, lineWidth);
        lineImageRectTransform.pivot = new Vector2(0, 0.5f);
        lineImageRectTransform.position = start;
        float angle = Mathf.Atan2(differenceVector.y, differenceVector.x) * Mathf.Rad2Deg;
        lineImageRectTransform.rotation = Quaternion.Euler(0, 0, angle);
    }
}

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TableSawUI : MonoBehaviour 
{
    public CameraControl camera;
    public Color SelectedButtonColor;
    public GameObject SelectedButton;

    void Start()
    {
        SelectedButton.GetComponent<Image>().color = SelectedButtonColor;
        SelectedButton.GetComponent<Button>().enabled = false;
    }

    public void SwitchActiveButton(GameObject buttonToUse)
    {
        if (buttonToUse != SelectedButton)
        {
            SelectedButton.GetComponent<Image>().color = Color.white;
            SelectedButton.GetComponent<Button>().enabled = true;

            SelectedButton = buttonToUse;
            SelectedButton.GetComponent<Image>().color = SelectedButtonColor;
            SelectedButton.GetComponent<Button>().enabled = false;
        }
    }

    public void ResetCamera()
    {
        camera.MinDistance = 0.5f;
        camera.MaxDistance = 2.0f;
    }

    public void SwitchToMakeCut()
    {
        camera.MinDistance = 0.1f;
        camera.MaxDistance = 0.6f;
    }
}

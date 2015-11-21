using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ClampUI : MainUI
{
    public Button ClampSpawnButton;

	void Start () 
    {
        //Initialize();
	}

    public void EnableSpawnButton(bool enable)
    {
        ClampSpawnButton.interactable = enable;
    }
}

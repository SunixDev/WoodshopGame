using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ClampManager : MonoBehaviour 
{
    public List<ClampPoint> ClampPoints;
    public CameraControl GameCamera;
    public GameObject ClampObject;
    public Vector3 ClampSpawn;
    public float MinConnectDistance;
    public double dryTimeInMinutes = 5.0;
    public ClampUI UI_Manager;

    private ClampController currentClamp;
    private ClampHeadPoint currentClampHead;
    private int clampPointsRemaining;
    private bool inProgress = true;
    private DateTime dryingTimeEnd;
    private string keyForDryingTime = "DryTime";
    private bool saveDryTime = false;

	void Start () 
    {
        UI_Manager.DisplayPlans(true);
        GameCamera.MovementEnabled = false;
        if (PlayerPrefs.HasKey(keyForDryingTime))
        {
            inProgress = false;
            clampPointsRemaining = 0;
            long binaryTime = Convert.ToInt64(PlayerPrefs.GetString(keyForDryingTime));
            dryingTimeEnd = DateTime.FromBinary(binaryTime);
            PlayerPrefs.DeleteKey(keyForDryingTime);
            UI_Manager.InfoPanel.SetActive(true);
            UI_Manager.InfoPanelText.text = "Your project will be dry in " + dryTimeInMinutes + " minutes.\nCome back then to continue the project";
            UI_Manager.InfoPanelButton.gameObject.SetActive(false);
        }
        else
        {
            clampPointsRemaining = ClampPoints.Count;
            GameObject clamp = Instantiate(ClampObject, ClampSpawn, Quaternion.identity) as GameObject;
            currentClamp = clamp.GetComponent<ClampController>();
            currentClamp.Moveable = true;
            currentClampHead = currentClamp.GetComponent<ClampHeadPoint>();
        }
    }

    void Update()
    {
        if (inProgress)
        {
            if (clampPointsRemaining <= 0)
            {
                System.DateTime currentTime = System.DateTime.Now;
                if (currentTime.CompareTo(dryingTimeEnd) >= 0)
                {
                    UI_Manager.InfoPanelText.text = "Your project is completely dry now. On to the next step.";
                    UI_Manager.InfoPanelButton.gameObject.SetActive(true);
                    inProgress = false;
                    saveDryTime = false;
                }
            }
            else
            {
                for (int i = 0; i < ClampPoints.Count && clampPointsRemaining > 0; i++)
                {
                    if (!ClampPoints[0].Clamped)
                    {
                        float distance = Vector3.Distance(currentClampHead.Position, ClampPoints[i].Position);
                        if (distance <= MinConnectDistance)
                        {
                            currentClampHead.ClampAt(ClampPoints[i]);
                            currentClamp.ResetSelection();
                            clampPointsRemaining--;

                            if (clampPointsRemaining > 0)
                            {
                                GameObject clamp = Instantiate(ClampObject, ClampSpawn, Quaternion.identity) as GameObject;
                                currentClamp = clamp.GetComponent<ClampController>();
                                currentClamp.Moveable = true;
                                currentClampHead = currentClamp.GetComponent<ClampHeadPoint>();
                            }
                            else
                            {
                                currentClamp = null;
                                currentClampHead = null;
                                DateTime dryTimeStart = DateTime.Now;
                                dryingTimeEnd = dryTimeStart.AddMinutes(dryTimeInMinutes);
                                UI_Manager.InfoPanel.SetActive(true);
                                UI_Manager.InfoPanelText.text = "Your project will be dry in " + dryTimeInMinutes + " minutes.\nCome back then to continue the project";
                                UI_Manager.InfoPanelButton.gameObject.SetActive(false);
                                saveDryTime = true;
                            }
                        }
                    }
                }
            }
        }
    }

    //public void RotateClampOrientation()
    //{
    //    currentClamp.RotateAtPoint(90.0f);
    //}

    public void SetUpClampMovement()
    {
        GameCamera.MovementEnabled = false;
        if (currentClamp != null)
        {
            currentClamp.Moveable = true;
        }
    }

    public void SetUpCameraMovement()
    {
        GameCamera.MovementEnabled = true;
        if (currentClamp != null)
        {
            currentClamp.Moveable = false;
        }
    }

    public void GoToScene(string sceneName)
    {
        Application.LoadLevel(sceneName);
    }

    public void OnApplicationQuit()
    {
        if (saveDryTime)
        {
            PlayerPrefs.SetString(keyForDryingTime, System.DateTime.Now.ToBinary().ToString());
            PlayerPrefs.SetInt("GlueLevel", Application.loadedLevel);
        }
    }
}

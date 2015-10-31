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
    public double dryTimeInSeconds = 15.0;
    public ClampUI UI_Manager;
    public Transform WoodProject;

    private ClampControl currentClampController;
    private Clamp currentClamp;
    private GluedPieceController piece;
    private int clampPointsRemaining;
    private bool inProgress = true;
    private DateTime dryingTimeEnd;
    private bool saveDryTime = false;

	void Start ()
    {
        UI_Manager.DisplayPlans(true);
        GameCamera.MovementEnabled = false;
        bool levelLoaded = LoadLevel();
        
        if (!levelLoaded)
        {
            SetupGame();
        }

        if (WoodProject == null)
        {
            Debug.LogError("WoodProject variable not assigned");
            inProgress = false;
        }
        else
        {
            foreach (ClampPoint point in ClampPoints)
            {
                point.DisplayPoint();
            }
        }
    }

    private void SetupGame()
    {
        clampPointsRemaining = ClampPoints.Count;
        GameObject clamp = Instantiate(ClampObject, ClampSpawn, Quaternion.identity) as GameObject;
        currentClampController = clamp.GetComponent<ClampControl>();
        currentClampController.Moveable = true;
        currentClamp = currentClampController.GetComponent<Clamp>();
        piece = WoodProject.gameObject.GetComponent<GluedPieceController>();
        if (piece == null)
        {
            piece = WoodProject.gameObject.AddComponent<GluedPieceController>();
            piece.Moveable = true;
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
                int nearestClampPointIndex = -1;
                for (int i = 0; i < ClampPoints.Count && clampPointsRemaining > 0 && nearestClampPointIndex == -1; i++)
                {
                    if (!ClampPoints[i].Clamped)
                    {
                        float distance = Vector3.Distance(currentClamp.ClampHead.position, ClampPoints[i].Position);
                        if (distance <= MinConnectDistance)
                        {
                            nearestClampPointIndex = i;
                            currentClamp.ClampAt(ClampPoints[i], WoodProject);
                        }
                    }
                }
                if (nearestClampPointIndex == -1)
                {
                    currentClamp.ReleaseClamp();
                }
                else if ((Input.touchCount == 0 && nearestClampPointIndex > -1) || (Input.GetTouch(0).phase == TouchPhase.Ended && nearestClampPointIndex > -1))
                {
                    ClampPoints[nearestClampPointIndex].Clamped = true;
                    clampPointsRemaining--;
                    currentClampController.ResetSelection();

                    ClampControl previousController = currentClampController;
                    if (clampPointsRemaining > 0)
                    {
                        GameObject clamp = Instantiate(ClampObject, ClampSpawn, Quaternion.identity) as GameObject;
                        currentClampController = clamp.GetComponent<ClampControl>();
                        currentClampController.Moveable = true;
                        currentClamp = currentClamp.GetComponent<Clamp>();
                    }
                    else
                    {
                        currentClampController = null;
                        currentClamp = null;
                        DateTime dryTimeStart = DateTime.Now;
                        dryingTimeEnd = dryTimeStart.AddSeconds(dryTimeInSeconds);
                        UI_Manager.InfoPanel.SetActive(true);
                        UI_Manager.InfoPanelText.text = "Your project will be dry in " + dryTimeInSeconds + " seconds.\nCome back then to continue the project";
                        UI_Manager.InfoPanelButton.gameObject.SetActive(false);
                        saveDryTime = true;
                    }
                    Destroy(previousController);
                }
            }
        }
    }

    private bool LoadLevel()
    {
        bool loaded = false;
        if (GameManager.instance != null)
        {
            if (PlayerPrefs.HasKey(GameManager.instance.DryTimeKey))
            {
                clampPointsRemaining = -1;
                long binaryTime = Convert.ToInt64(PlayerPrefs.GetString(GameManager.instance.DryTimeKey));
                dryingTimeEnd = DateTime.FromBinary(binaryTime);
                PlayerPrefs.DeleteKey(GameManager.instance.DryTimeKey);
                UI_Manager.InfoPanel.SetActive(true);
                UI_Manager.InfoPanelText.text = "Your project will be dry in " + dryTimeInSeconds + " seconds.\nCome back then to continue the project";
                UI_Manager.InfoPanelButton.gameObject.SetActive(false);
                loaded = true;
            }
        }
        return loaded;
    }

    public void SetUpClampMovement()
    {
        GameCamera.MovementEnabled = false;
        piece.Moveable = true;
        if (currentClampController != null)
        {
            currentClampController.Moveable = true;
        }
    }

    public void SetUpCameraMovement()
    {
        GameCamera.MovementEnabled = true;
        piece.Moveable = false;
        if (currentClampController != null)
        {
            currentClampController.Moveable = false;
        }
    }

    public void GoToScene(string sceneName)
    {
        Application.LoadLevel(sceneName);
    }

    public void OnApplicationQuit()
    {
        if (saveDryTime && GameManager.instance != null)
        {
            PlayerPrefs.SetString(GameManager.instance.DryTimeKey, System.DateTime.Now.ToBinary().ToString());
            PlayerPrefs.SetInt(GameManager.instance.LevelKey, Application.loadedLevel);
            PlayerPrefs.SetFloat(GameManager.instance.ScoreKey, GameManager.instance.totalScorePercentage);
            PlayerPrefs.SetFloat(GameManager.instance.StepsKey, GameManager.instance.numberOfSteps);
            PlayerPrefs.Save();
        }
    }
}





//if (clampConnected)
//                        {
//                            currentClampHead.ClampAt(ClampPoints[i], WoodProject);
//                            currentClamp.ResetSelection();
//                            clampPointsRemaining--;

//                            if (clampPointsRemaining > 0)
//                            {
//                                GameObject clamp = Instantiate(ClampObject, ClampSpawn, Quaternion.identity) as GameObject;
//                                currentClamp = clamp.GetComponent<ClampControl>();
//                                currentClamp.Moveable = true;
//                                currentClampHead = currentClamp.GetComponent<ClampHeadPoint>();
//                            }
//                            else
//                            {
//                                currentClamp = null;
//                                currentClampHead = null;
//                                DateTime dryTimeStart = DateTime.Now;
//                                dryingTimeEnd = dryTimeStart.AddSeconds(dryTimeInSeconds);
//                                UI_Manager.InfoPanel.SetActive(true);
//                                UI_Manager.InfoPanelText.text = "Your project will be dry in " + dryTimeInSeconds + " seconds.\nCome back then to continue the project";
//                                UI_Manager.InfoPanelButton.gameObject.SetActive(false);
//                                saveDryTime = true;
//                            }
//                        }
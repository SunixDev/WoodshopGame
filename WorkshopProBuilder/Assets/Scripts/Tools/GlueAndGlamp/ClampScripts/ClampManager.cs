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
        currentClamp = clamp.GetComponent<Clamp>();
        currentClamp.controller.Moveable = true;
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
                if (Input.touchCount > 0 || Input.GetMouseButton(0))
                {
                    //if (Input.GetTouch(0).phase != TouchPhase.Ended || Input.GetMouseButton(0))
                    //{
                        
                    //}
                    //if (Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetMouseButtonUp(0))
                    //{

                    //}
                }
                //if (nearestClampPointIndex == -1)
                //{
                //    currentClamp.ReleaseClamp();
                //}
                //else if (Input.touchCount > 0 )
                //{
                //    if (nearestClampPointIndex >= 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
                //    {
                //        ClampPoints[nearestClampPointIndex].Clamped = true;
                //        clampPointsRemaining--;
                //        currentClamp.controller.ResetSelection();

                //        if (clampPointsRemaining > 0)
                //        {
                //            GameObject clamp = Instantiate(ClampObject, ClampSpawn, Quaternion.identity) as GameObject;
                //            currentClamp = clamp.GetComponent<Clamp>();
                //            currentClamp.controller.Moveable = true;
                //        }
                //        else
                //        {
                //            currentClamp = null;
                //            DateTime dryTimeStart = DateTime.Now;
                //            dryingTimeEnd = dryTimeStart.AddSeconds(dryTimeInSeconds);
                //            UI_Manager.InfoPanel.SetActive(true);
                //            UI_Manager.InfoPanelText.text = "Your project will be dry in " + dryTimeInSeconds + " seconds.\nCome back then to continue the project";
                //            UI_Manager.InfoPanelButton.gameObject.SetActive(false);
                //            saveDryTime = true;
                //        }
                //    }
                //}
            }
        }
    }

    private float DistanceFromDistance(ClampPoint point)
    {
        float distance = -1.0f;
        if (Input.touchCount > 0 || Input.GetMouseButton(0))
        {
            Vector2 touchPoint = (Input.touchCount > 0) ? Input.GetTouch(0).position : new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            Vector3 touchPosition = Camera.main.ScreenToWorldPoint(new Vector3(touchPoint.x, touchPoint.y, 10.0f));
            touchPosition.z = point.transform.position.z;
            distance = Vector3.Distance(touchPosition, point.transform.position);
        }
        return distance;
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
        currentClamp.controller.Moveable = true;
        
    }

    public void SetUpCameraMovement()
    {
        GameCamera.MovementEnabled = true;
        piece.Moveable = false;
        currentClamp.controller.Moveable = false;
    }

    public void GoToScene(string sceneName)
    {
        if (GameManager.instance != null)
        {
            Application.LoadLevel(sceneName);
        }
        else
        {
            Debug.Log("Moving on to " + sceneName + " scene.");
        }
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
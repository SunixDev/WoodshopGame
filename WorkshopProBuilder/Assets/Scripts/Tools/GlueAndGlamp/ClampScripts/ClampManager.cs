using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ClampManager : MonoBehaviour 
{
    public List<ClampPoint> ClampPoints;
    public GameObject Clamp;
    public Transform ClampSpawnPoint;
    public double dryTimeInSeconds = 15.0;
    public ClampUI UI_Manager;
    public Transform WoodProject;
    public LayerMask clampingLayerMask;

    private ClampControl currentClamp;
    private bool clampPlacementInProgress = true;
    private DateTime dryingTimeEnd;
    private bool saveDryTime = false;
    private bool updatingTouchPosition;
    private Vector2 currentFingerPosition;

	void Start ()
    {
        updatingTouchPosition = true;
        bool levelLoaded = LoadLevel();
        
        if (!levelLoaded)
        {
            SetupGame();
        }

        if (WoodProject == null)
        {
            Debug.LogError("WoodProject variable not assigned");
            clampPlacementInProgress = false;
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
        GameObject clamp = Instantiate(Clamp, ClampSpawnPoint.position, Quaternion.identity) as GameObject;
        currentClamp = clamp.GetComponent<ClampControl>();
    }

    void Update()
    {
        if (clampPlacementInProgress)
        {
            if (ClampPoints.Count <= 0 || ClampPoints == null)
            {
                System.DateTime currentTime = System.DateTime.Now;
                if (currentTime.CompareTo(dryingTimeEnd) >= 0)
                {
                    UI_Manager.DisplayResultsPanel("Your project is completely dry now. On to the next step.", displayNextSceneButton: true);
                    clampPlacementInProgress = false;
                    saveDryTime = false;
                }
            }
            else
            {
                
            }
        }
    }

    public void UpdateTouchPosition(Gesture gesture)
    {
        if (gesture.touchCount == 1)
        {
            currentFingerPosition = gesture.position;
            updatingTouchPosition = true;
        }
    }

    public void StopUpdatingTouchPosition(Gesture gesture)
    {
        if (gesture.touchCount == 1)
        {
            updatingTouchPosition = false;
        }
    }

    private bool LoadLevel()
    {
        bool loaded = false;
        if (GameManager.instance != null)
        {
            if (PlayerPrefs.HasKey(GameManager.instance.DryTimeKey))
            {
                long binaryTime = Convert.ToInt64(PlayerPrefs.GetString(GameManager.instance.DryTimeKey));
                dryingTimeEnd = DateTime.FromBinary(binaryTime);
                PlayerPrefs.DeleteKey(GameManager.instance.DryTimeKey);
                UI_Manager.DisplayResultsPanel("Your project will be dry in " + dryTimeInSeconds + " seconds.\nCome back then to continue the project");
                loaded = true;
            }
        }
        return loaded;
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



    private void EnableTouchEvents()
    {
        EasyTouch.On_TouchDown += UpdateTouchPosition;
        EasyTouch.On_TouchUp += StopUpdatingTouchPosition;
    }

    private void DisableTouchEvents()
    {
        EasyTouch.On_TouchDown -= UpdateTouchPosition;
        EasyTouch.On_TouchUp -= StopUpdatingTouchPosition;
    }

    void OnEnable()
    {
        EnableTouchEvents();
    }
    void OnDisable()
    {
        DisableTouchEvents();
    }
    void OnDestory()
    {
        DisableTouchEvents();
    }
}
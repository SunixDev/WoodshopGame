using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ClampManager : MonoBehaviour 
{
    public List<ClampPoint> ClampPoints;
    public double dryTimeInSeconds = 15.0;
    public MainUI UI_Manager;

    private bool clampPlacementInProgress = true;
    private DateTime dryingTimeEnd;
    private bool saveDryTime = false;

	void Start ()
    {
        bool levelLoaded = LoadLevel();
        
        if (!levelLoaded)
        {
            if (ClampPoints.Count <= 0)
            {
                clampPlacementInProgress = false;
                Debug.Log("No clamp points were found or assigned");
            }
            else
            {
                UI_Manager.Initialize();
                foreach (ClampPoint point in ClampPoints)
                {
                    point.gameObject.SetActive(true);
                    point.DisplayPoint();
                }
            }
        }
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
        }
    }

    public void UpdateClampedPoints(ClampPoint point)
    {
        ClampPoints.Remove(point);
        if (ClampPoints.Count <= 0 || ClampPoints == null)
        {
            dryingTimeEnd = System.DateTime.Now.AddSeconds(dryTimeInSeconds);
            UI_Manager.DisplayResultsPanel("Your project will be dry in " + dryTimeInSeconds + " seconds.\nCome back then to continue the project");
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
}
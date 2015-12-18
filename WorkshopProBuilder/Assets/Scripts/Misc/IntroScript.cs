using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class IntroScript : MonoBehaviour 
{
    public string FirstScene;
    public GameObject ModalBackground;
    public GameObject InstructionsPanel;
    public GameObject CameraControlsPanel;

    void Start()
    {
        if (PlayerPrefs.HasKey(GameManager.instance.LevelKey))
        {
            string level = PlayerPrefs.GetString(GameManager.instance.LevelKey);
            PlayerPrefs.DeleteKey(GameManager.instance.LevelKey);

            GameManager.instance.totalScorePercentage = PlayerPrefs.GetFloat(GameManager.instance.ScoreKey);
            PlayerPrefs.DeleteKey(GameManager.instance.ScoreKey);

            GameManager.instance.numberOfSteps = PlayerPrefs.GetFloat(GameManager.instance.StepsKey);
            PlayerPrefs.DeleteKey(GameManager.instance.StepsKey);
            Application.LoadLevel(level);
        }
    }

    public void StartProject()
    {
        GameManager.instance.ResetScore();
        Application.LoadLevel(FirstScene);
    }

    public void ShowInstructions(bool show)
    {
        ModalBackground.SetActive(show);
        InstructionsPanel.SetActive(show);
    }

    public void ShowCameraControls(bool show)
    {
        ModalBackground.SetActive(show);
        CameraControlsPanel.SetActive(show);
    }
}

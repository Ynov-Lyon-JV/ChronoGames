using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndRaceManager : MonoBehaviour
{
    #region Fields

    private string lastTime;

    private RaceManager raceManager;
    private APIController apiController;
    private GameManager gameManager;

    private GameObject endPanel;
    private TMP_Text txtLastTime;
    private TMP_Text txtPBTime;
    private TMP_Text txtWRTime;
    private TMP_Text txtMessage;

    #endregion

    #region Unity Methods

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        raceManager = GameObject.Find("RaceManager").GetComponent<RaceManager>();
        apiController = GameObject.Find("APIManager").GetComponent<APIController>();
        apiController.ApiResponseCodeUpdated += () => UpdateMessage();

        endPanel = GameObject.Find("EndPanel");
        txtWRTime = endPanel.GetComponentsInChildren<TMP_Text>()[1];
        txtPBTime = endPanel.GetComponentsInChildren<TMP_Text>()[2];
        txtLastTime = endPanel.GetComponentsInChildren<TMP_Text>()[3];
        txtMessage = endPanel.GetComponentsInChildren<TMP_Text>()[4];
    }

    #endregion

    #region Private Methods

    private void SetPBAndWR()
    {
        string wrTime = gameManager.SelectedMapId != -5 ? raceManager.WrTime.time : "no time";
        string pbTime = gameManager.SelectedMapId != -5 ? raceManager.PbTime.time : "no time";
        txtWRTime.text = $"World record : {wrTime}";
        txtPBTime.text = $"Best time : {pbTime}";
    }

    private void UpdateMessage()
    {
        if (apiController.ApiResponseCode == "201")
        {
            txtMessage.enabled = true;
            txtPBTime.text = $"Best time : {lastTime}";
            if (TimeSpan.ParseExact(lastTime, @"mm\:ss\:FFF", CultureInfo.InvariantCulture) < TimeSpan.ParseExact(raceManager.WrTime.time, @"mm\:ss\:FFF", CultureInfo.InvariantCulture))
            {
                txtMessage.text = "NEW WORLD RECORD";
            }
        }
    }

    #endregion

    #region Public Methods
    public void SetLastTime()
    {
        lastTime = GameObject.Find("GameHUD").GetComponent<DisplayData>().TimerText.text;
        txtLastTime.text = $"Race time : {lastTime}";

        SetPBAndWR();
    }

    public void RestartRace()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void BackToSelection()
    {
        SceneManager.LoadScene("SelectionScene");
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
    public void Quit()
    {
        Application.Quit();
    }

    #endregion
}

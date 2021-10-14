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

    private GameObject endPanel;
    private TMP_Text txtLastTime;
    private TMP_Text txtPBTime;
    private TMP_Text txtWRTime;
    private TMP_Text txtMessage;

    #endregion

    #region Unity Methods

    private void Start()
    {
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
        txtWRTime.text = $"World record : {raceManager.WrTime.time}";
        txtPBTime.text = $"Best time : {raceManager.PbTime.time}";
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
        txtLastTime.text = $"Last time : {lastTime}";

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

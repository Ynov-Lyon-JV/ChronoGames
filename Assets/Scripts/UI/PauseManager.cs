using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    #region Fields

    private RaceManager raceManager;

    private bool isPaused = false;
    private GameObject pausePanel;
    private PanelOpener pausePanelOpener;

    private TMP_Text txtPBTime;
    private TMP_Text txtWRTime;

    private bool isSet = false;

    #endregion

    #region Properties

    public bool IsPaused { get => isPaused; set => isPaused = value; }

    #endregion

    #region Unity Methods
    // Start is called before the first frame update
    void Start()
    {
        raceManager = GameObject.Find("RaceManager").GetComponent<RaceManager>();

        pausePanel = GameObject.Find("PausePanel");
        pausePanelOpener = pausePanel.GetComponent<PanelOpener>();

        txtPBTime = pausePanel.GetComponentsInChildren<TMP_Text>()[1];
        txtWRTime = pausePanel.GetComponentsInChildren<TMP_Text>()[2];
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            SetPBAndWR();
            isPaused = !isPaused;
            pausePanelOpener.OpenClosePanel();
        }
    }

    #endregion

    #region Private Methods
    private void SetPBAndWR()
    {
        if (!isSet)
        {
            txtWRTime.text = $"World record : {raceManager.WrTime.time}";
            txtPBTime.text = $"Best time : {raceManager.PbTime.time}";
            isSet = true;
        }
    }
    #endregion

    #region Public Methods

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

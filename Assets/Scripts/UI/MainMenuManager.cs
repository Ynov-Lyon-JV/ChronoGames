using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MainMenuManager : MonoBehaviour
{
    #region Fields

    private GameObject optionMenuPanel;
    private GameObject mainMenuPanel;
    private PanelOpener optionPanelOpener;
    private PanelOpener mainMenuPanelOpener;

    #endregion

    #region Unity Methods

    private void Start()
    {
        optionMenuPanel = GameObject.Find("OptionMenuPanel");
        optionPanelOpener = optionMenuPanel.GetComponent<PanelOpener>();

        mainMenuPanel = GameObject.Find("MainMenuPanel");
        mainMenuPanelOpener = mainMenuPanel.GetComponent<PanelOpener>();
    }

    #endregion

    #region Public Methods
    public void Play()
    {
        SceneManager.LoadScene("SelectionScene");
    }

    public void Edit()
    {
        SceneManager.LoadScene("MapEditorScene");
    }

    public void OpenCloseOptionPanel()
    {
        mainMenuPanelOpener.OpenClosePanel();
        optionPanelOpener.OpenClosePanel();
    }

    public void Quit()
    {
        Application.Quit();
    }

    #endregion
}

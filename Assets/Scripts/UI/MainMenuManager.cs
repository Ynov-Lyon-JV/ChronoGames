using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MainMenuManager : MonoBehaviour
{
    #region Fields

    private GameObject _optionMenuPanel;
    private GameObject _mainMenuPanel;
    private PanelOpener _optionPanelOpener;
    private PanelOpener _mainMenuPanelOpener;

    public GameObject newLoadPanel;
    public GameObject fileSelectionPanel;

    #endregion

    #region Unity Methods

    private void Start()
    {
        _optionMenuPanel = GameObject.Find("OptionMenuPanel");
        _optionPanelOpener = _optionMenuPanel.GetComponent<PanelOpener>();

        _mainMenuPanel = GameObject.Find("MainMenuPanel");
        _mainMenuPanelOpener = _mainMenuPanel.GetComponent<PanelOpener>();
    }

    #endregion

    #region Public Methods
    public void Play()
    {
        SceneManager.LoadScene("SelectionScene");
    }

    public void DisplayPrompt(bool isOpen)
    {
        newLoadPanel.SetActive(isOpen);
    }

    public void DisplayFileSelection(bool isOpen)
    {
        if (newLoadPanel.activeInHierarchy)
            newLoadPanel.SetActive(false);
        fileSelectionPanel.SetActive(isOpen);
    }

    public void LoadEditor(int environment)
    {
        switch (environment)
        {
            case 0:
                //Grass environment
                break;
            case 1:
                //Desert environment
                break;
            default:
                //Grass environment
                break;
        }
    }

    public void LoadEditor(string mapName)
    {
        SceneManager.LoadScene("MapEditorScene");
    }

    public void OpenCloseOptionPanel()
    {
        _mainMenuPanelOpener.OpenClosePanel();
        _optionPanelOpener.OpenClosePanel();
    }

    public void Quit()
    {
        Application.Quit();
    }

    #endregion
}

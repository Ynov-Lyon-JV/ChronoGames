using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MainMenuManager : MonoBehaviour
{
    #region Constantes
    
    private const string EDITED_CHRONOGAME_FOLDER = "chronogame";
    private const string EDITED_MAP_FOLDER = "maps";
    private const string EDITED_EDITED_FOLDER = "edited"; 

    #endregion

    #region Fields

    private GameManager _gameManager;

    private string _documentPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
    private string _editedMapPath;

    private GameObject _optionMenuPanel;
    private GameObject _mainMenuPanel;
    private PanelOpener _optionPanelOpener;
    private PanelOpener _mainMenuPanelOpener;

    public GameObject newLoadPanel;
    public GameObject fileSelectionPanel;

    public Transform contentPanel;
    public GameObject mapItem;

    #endregion

    #region Unity Methods

    private void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();

        _optionMenuPanel = GameObject.Find("OptionMenuPanel");
        _optionPanelOpener = _optionMenuPanel.GetComponent<PanelOpener>();

        _mainMenuPanel = GameObject.Find("MainMenuPanel");
        _mainMenuPanelOpener = _mainMenuPanel.GetComponent<PanelOpener>();

        _editedMapPath = Path.Combine(_documentPath, EDITED_CHRONOGAME_FOLDER, EDITED_MAP_FOLDER, EDITED_EDITED_FOLDER);
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

        if (isOpen)
        {
            //Remove all existing files ?
            foreach (Transform child in contentPanel)
            {
                Destroy(child.gameObject);
            }

            //Gather all files
            foreach (string filePath in Directory.GetFiles(_editedMapPath, "*.json"))
            {
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                GameObject go = Instantiate(mapItem, contentPanel);
                go.GetComponentInChildren<TMP_Text>().text = fileName;
            } 
        }

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

        _gameManager.SelectedMapName = string.Empty;
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

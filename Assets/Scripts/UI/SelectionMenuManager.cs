using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.IO;
using System;

public class SelectionMenuManager : MonoBehaviour
{
    #region Constantes

    private const string EDITED_CHRONOGAME_FOLDER = "chronogame";
    private const string EDITED_MAP_FOLDER = "maps";
    private const string EDITED_EDITED_FOLDER = "edited";

    #endregion

    #region Fields
    private TMP_Dropdown ddVehicles;
    private TMP_Dropdown ddMaps;
    private TMP_Dropdown ddGhosts;

    private GameManager gameManager;
    private GameObject spawnedVehicle = null;
    private Transform spawnVehicleTransform;

    private APIController apiController;
    private bool isPBNotNull;
    private bool isWRNotNull;
    private Toggle cbIsGhost;

    private string _documentPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
    private string _mapPath;

    public GameObject mapSelectionPanel;
    public Transform contentPanel;
    public GameObject mapItem;

    public GameObject carSelectionPanel;

    #endregion

    #region Unity Methods
    // Start is called before the first frame update
    void Start()
    {
        _mapPath = Path.Combine(_documentPath, EDITED_CHRONOGAME_FOLDER, EDITED_MAP_FOLDER, EDITED_EDITED_FOLDER);

        cbIsGhost = FindObjectOfType<Toggle>();

        spawnVehicleTransform = GameObject.Find("SpawnVehicle").transform;
        gameManager = FindObjectOfType<GameManager>();

        apiController = FindObjectOfType<APIController>();
        apiController.PBUpdated += () => PBIsUpdated();
        apiController.WRUpdated += () => WRIsUpdated();

        //ddGhosts = ddList[0];
        //InitialiseDDGhosts();

        //apiController.GetTimeFromAPI(gameManager.SelectedMapIndex + 1, gameManager.SelectedVehicleIndex + 1, null);
        //apiController.GetTimeFromAPI(gameManager.SelectedMapIndex + 1, gameManager.SelectedVehicleIndex + 1, gameManager.UserName);

    }
    #endregion

    #region Private Methods

    private void PBIsUpdated()
    {
        isPBNotNull = apiController.Pb.rank != "-1";
        //InitialiseDDGhosts();
    }

    private void WRIsUpdated()
    {
        isWRNotNull = apiController.Wr.rank != "-1";
        //InitialiseDDGhosts();
    }

    private void InitialiseDDGhosts()
    {
        //ddGhosts.ClearOptions();
        List<string> listNames = new List<string>();
        if (isPBNotNull)
        {
            listNames.Add("Personnal Best");
        }
        if (isWRNotNull)
        {
            listNames.Add("World Record");
        }

        if (listNames.Count > 0)
        {
            //ddGhosts.AddOptions(listNames);
            gameManager.GhostType = ddGhosts.options[ddGhosts.value].text;
        }

        //cbIsGhost.interactable = listNames.Count > 0;
        //ddGhosts.interactable = listNames.Count > 0;
        //cbIsGhost.isOn = false;
    }

    #endregion

    #region Public Methods

    public void GhostDropdownValueChanged()
    {
        gameManager.GhostType = ddGhosts.options[ddGhosts.value].text;
    }

    public void SelectCar(int carId)
    {
        //Faire des sortes de cartes pour les différentes voitures
        //Appeler cette fonction au clic de la carte et afficher la sélection de map

        gameManager.SelectedVehicleIndex = carId;

        DisplayMapCarSelection(true);
    }

    public void DisplayMapCarSelection(bool isDisplayed)
    {
        carSelectionPanel.SetActive(!isDisplayed);

        if (isDisplayed)
        {
            //Remove all existing files ?
            foreach (Transform child in contentPanel)
            {
                Destroy(child.gameObject);
            }

            //Gather all files
            foreach (string filePath in Directory.GetFiles(_mapPath, "*.json"))
            {
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                GameObject go = Instantiate(mapItem, contentPanel);
                go.GetComponentInChildren<TMP_Text>().text = fileName;
            }
        }

        mapSelectionPanel.SetActive(isDisplayed);
    }

    //public void StartGame()
    //{
    //    gameManager.IsGhostSelected = GameObject.Find("CbIsGhost").GetComponent<Toggle>().isOn;
    //    SceneManager.LoadScene("GameScene");
    //}

    #endregion
}
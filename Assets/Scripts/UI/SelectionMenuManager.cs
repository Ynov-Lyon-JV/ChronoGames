using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectionMenuManager : MonoBehaviour
{
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
    #endregion

    #region Unity Methods
    // Start is called before the first frame update
    void Start()
    {
        cbIsGhost = FindObjectOfType<Toggle>();

        spawnVehicleTransform = GameObject.Find("SpawnVehicle").transform;
        gameManager = FindObjectOfType<GameManager>();

        apiController = FindObjectOfType<APIController>();
        apiController.PBUpdated += () => PBIsUpdated();
        apiController.WRUpdated += () => WRIsUpdated();

        List<string> listName = new List<string>();
        foreach (GameObject vehicle in gameManager.Vehicles)
        {
            listName.Add(vehicle.name);
        }

        List<string> listMapNames = new List<string>();
        foreach (GameObject map in gameManager.Maps)
        {
            listMapNames.Add(map.name);
        }

        TMP_Dropdown[] ddList = GetComponentsInChildren<TMP_Dropdown>();
        ddVehicles = ddList[1];
        ddVehicles.ClearOptions();
        ddVehicles.AddOptions(listName);

        ddMaps = ddList[2];
        ddMaps.ClearOptions();
        ddMaps.AddOptions(listMapNames);
        gameManager.SelectedMapIndex = ddMaps.value;

        ddGhosts = ddList[0];
        InitialiseDDGhosts();

        apiController.GetTimeFromAPI(gameManager.SelectedMapIndex + 1, gameManager.SelectedVehicleIndex + 1, null);
        apiController.GetTimeFromAPI(gameManager.SelectedMapIndex + 1, gameManager.SelectedVehicleIndex + 1, gameManager.UserName);

        SpawnVehicle();
    }
    #endregion

    #region Private Methods

    private void SpawnVehicle()
    {
        int vehicleIndex = ddVehicles.value; 
        gameManager.SelectedVehicleIndex = vehicleIndex;

        if (spawnedVehicle != null)
        {
            Destroy(spawnedVehicle);
        }
        spawnedVehicle = Instantiate(this.gameManager.SelectedVehiclePrefab, spawnVehicleTransform.position, spawnVehicleTransform.rotation);
    }

    private void PBIsUpdated()
    {
        isPBNotNull = apiController.Pb.rank != "-1";
        InitialiseDDGhosts();
    }

    private void WRIsUpdated()
    {
        isWRNotNull = apiController.Wr.rank != "-1";
        InitialiseDDGhosts();
    }

    private void InitialiseDDGhosts()
    {
        ddGhosts.ClearOptions();
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
            ddGhosts.AddOptions(listNames);
            gameManager.GhostType = ddGhosts.options[ddGhosts.value].text;
        }

        cbIsGhost.interactable = listNames.Count > 0;
        ddGhosts.interactable = listNames.Count > 0;
        cbIsGhost.isOn = false;
    }

    #endregion

    #region Public Methods

    public void VehicleDropdownValueChanged()
    {
        SpawnVehicle();
        apiController.GetTimeFromAPI(gameManager.SelectedMapIndex + 1, gameManager.SelectedVehicleIndex + 1, null);
        apiController.GetTimeFromAPI(gameManager.SelectedMapIndex + 1, gameManager.SelectedVehicleIndex + 1, gameManager.UserName);
    }

    public void MapDropdownValueChanged()
    {
        gameManager.SelectedMapIndex = ddMaps.value;
        apiController.GetTimeFromAPI(gameManager.SelectedMapIndex + 1, gameManager.SelectedVehicleIndex + 1, null);
        apiController.GetTimeFromAPI(gameManager.SelectedMapIndex + 1, gameManager.SelectedVehicleIndex + 1, gameManager.UserName);
    }

    public void GhostDropdownValueChanged()
    {
        gameManager.GhostType = ddGhosts.options[ddGhosts.value].text;
    }

    public void StartGame()
    {
        gameManager.IsGhostSelected = GameObject.Find("CbIsGhost").GetComponent<Toggle>().isOn;
        SceneManager.LoadScene("GameScene");
    }

    #endregion
}

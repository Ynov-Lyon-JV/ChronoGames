using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardManager : MonoBehaviour
{
    #region Fields

    private TMP_Dropdown ddVehicles;
    private TMP_Dropdown ddMaps;

    private Transform entryContainer;
    private Transform entryTemplate;

    private APIController apiController;
    private GameManager gameManager;

    private List<APITime> listTimes;

    #endregion

    #region Properties


    #endregion

    #region Unity Methods

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();

        apiController = FindObjectOfType<APIController>();
        apiController.ListUpdated += () => UpdateLeaderboard();

        entryContainer = GameObject.Find("LbEntryContainer").transform;
        entryTemplate = GameObject.Find("LbEntryTemplate").transform;

        entryTemplate.gameObject.SetActive(false);

        List<string> listVehicleNames = new List<string>();
        foreach (GameObject vehicle in gameManager.Vehicles)
        {
            listVehicleNames.Add(vehicle.name);
        }

        TMP_Dropdown[] ddList = GetComponentsInChildren<TMP_Dropdown>();
        ddVehicles = ddList[1];
        ddVehicles.ClearOptions();
        ddVehicles.AddOptions(listVehicleNames);
    }

    #endregion

    #region Private Methods

    private void ResetTable()
    {
        GameObject[] entries = GameObject.FindGameObjectsWithTag("Entry");
        for (int i = 0; i < entries.Length; i++)
        {
            Destroy(entries[i]);
        }
    }

    private void CheckList()
    {
        if (apiController.ListAPITimes.Count > 0)
        {
            UpdateLeaderboard();
        }    
    }

    private void UpdateLeaderboard()
    {
        ResetTable();

        listTimes = apiController.ListAPITimes;

        float templateHeight = 50.0f;
        int i = 0;
        foreach (APITime time in listTimes)
        {
            Transform entryTransform = Instantiate(entryTemplate, entryContainer);
            RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
            entryRectTransform.anchoredPosition = new Vector2(0, -templateHeight * i);
            entryTransform.gameObject.SetActive(true);

            //Gérer l'alternance des couleurs
            if (i % 2 == 0)
                entryTransform.gameObject.GetComponent<Image>().color = new Color(0.53f, 0.74f, 1.0f);

            entryTransform.Find("TxtRank").GetComponent<TMP_Text>().text = time.rank;
            entryTransform.Find("TxtTime").GetComponent<TMP_Text>().text = time.time;
            entryTransform.Find("TxtName").GetComponent<TMP_Text>().text = time.username;

            i++;
        }
    }

    #endregion

    #region Public Methods

    public void PopulateTable()
    {
        //Récupérer les données de l'API
        apiController.GetTimesFromAPI(ddMaps.value + 1, ddVehicles.value + 1, null, 10, 0);
    }

    public void DropdownValueChanged()
    {
        apiController.GetTimesFromAPI(ddMaps.value + 1, ddVehicles.value + 1, null, 10, 0);
    }

    #endregion
}

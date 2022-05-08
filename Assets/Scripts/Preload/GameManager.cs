using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Fields

    //Selection
    [SerializeField] private List<GameObject> vehicles;
    [SerializeField] private List<GameObject> modules;

    private int selectedVehicleIndex;
    private GameObject selectedVehiclePrefab;
    private string selectedMapName;
    private int selectedMapId;

    //Connectivity
    private bool isConnectedToInternet = false;
    private string userToken = string.Empty;
    private string userName = string.Empty;

    //Ghost
    private bool isGhostSelected = false;
    private string ghostType;
    private float frequence = 15;

    //Language
    private string language = "FR";

    #endregion

    #region Properties

    //Selection
    public List<GameObject> Vehicles { get => vehicles; }
    public List<GameObject> Modules { get => modules; }
    public GameObject SelectedVehiclePrefab { get => selectedVehiclePrefab; }
    public int SelectedVehicleIndex { get => selectedVehicleIndex; set => SetVehicleIndexAndPrefab(value); }
    public string SelectedMapName { get => selectedMapName; set => selectedMapName = value; }
    public int SelectedMapId { get => selectedMapId; set => selectedMapId = value; }

    //Connectivity
    public bool IsConnectedToInternet { get => isConnectedToInternet; set => isConnectedToInternet = value; }
    public string UserToken { get => userToken; set => userToken = value; }
    public string UserName { get => userName; set => userName = value; }

    //Ghost
    public bool IsGhostSelected { get => isGhostSelected; set => isGhostSelected = value; }
    public string GhostType { get => ghostType; set => ghostType = value; }
    public float Frequence { get => frequence; set => frequence = value; }

    //Language
    public string Language { get => language; set => language = value; }

    #endregion

    #region Unity Methods
    private void Start()
    {
        //Connectivity
        UnityWebRequest www = new UnityWebRequest("http://google.com");
        if (www.error != null)
        {
            isConnectedToInternet = false;
        }
        else
        {
            isConnectedToInternet = true;
        }

        // Language
        //Language = "FR";

        if (CheckToken())
        {
            SceneManager.LoadScene("LoginMenuScene");
        }
    }
    #endregion

    #region Private Methods

    private void SetVehicleIndexAndPrefab(int _value)
    {
        selectedVehicleIndex = _value;
        selectedVehiclePrefab = vehicles[_value];
    }

    private bool CheckToken()
    {
        //Get return code from API class
        return true;
    }

#if UNITY_EDITOR
    private string GetTokenFromFile()
    {
        string token = string.Empty;
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Assets\\Scripts\\token.txt");

        token = File.ReadAllText(filePath);

        return token;
    }
#endif

#endregion
}

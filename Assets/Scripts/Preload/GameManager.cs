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
    [SerializeField] private List<GameObject> maps;
    [SerializeField] private List<GameObject> modules;

    private int selectedVehicleIndex;
    private GameObject selectedVehiclePrefab;
    private int selectedMapIndex;
    private GameObject selectedMapPrefab;
    private string selectedMapName;

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
    public List<GameObject> Maps { get => maps; }
    public List<GameObject> Modules { get => modules; }
    public GameObject SelectedVehiclePrefab { get => selectedVehiclePrefab; }
    public GameObject SelectedMapPrefab { get => selectedMapPrefab; }
    public int SelectedVehicleIndex { get => selectedVehicleIndex; set => SetVehicleIndexAndPrefab(value); }
    public int SelectedMapIndex { get => selectedMapIndex; set => SetMapIndexAndPrefab(value); }
    public string SelectedMapName { get => selectedMapName; set => selectedMapName = value; }

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

#if UNITY_EDITOR
        //Username
        UserName = "Test";
        // Token
        UserToken = GetTokenFromFile();
        // Language
        Language = "FR";
#else
        UserToken = "270c4c85d9d239f66a563deac1c65318ce2f9862e1576583d70069bff178c030";//System.Array.IndexOf(args, "-usertoken") != -1 ? args[System.Array.IndexOf(args, "-usertoken") + 1] : string.Empty;

        // Test once launcher can pass parameters
        // Retrieve arguments data
        string[] args = System.Environment.GetCommandLineArgs();

        if (args.Length > 0)
	    {
		    // Username
            UserName = System.Array.IndexOf(args, "-username") != -1 ? args[System.Array.IndexOf(args, "-username") + 1] : string.Empty;
            // Token
            UserToken = "270c4c85d9d239f66a563deac1c65318ce2f9862e1576583d70069bff178c030";//System.Array.IndexOf(args, "-usertoken") != -1 ? args[System.Array.IndexOf(args, "-usertoken") + 1] : string.Empty;
            // Language
            Language = System.Array.IndexOf(args, "-language") != -1 ? args[System.Array.IndexOf(args, "-language") + 1] : string.Empty; 
	    }


#endif



        if (CheckToken())
        {
            SceneManager.LoadScene("MainMenuScene");
        }
    }
    #endregion

    #region Private Methods

    private void SetVehicleIndexAndPrefab(int _value)
    {
        selectedVehicleIndex = _value;
        selectedVehiclePrefab = vehicles[_value];
    }

    private void SetMapIndexAndPrefab(int _value)
    {
        selectedMapIndex = _value;
        selectedMapPrefab = maps[_value];
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

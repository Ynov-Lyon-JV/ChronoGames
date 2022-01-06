using UnityEngine;
using UnityEngine.SceneManagement;

public class UIMenu : MonoBehaviour
{
    private SaveLoadManager _slmanager;
    private ModuleManager _moduleManager;

    private void Start()
    {
        _slmanager = FindObjectOfType<SaveLoadManager>();
        _moduleManager = GetComponent<ModuleManager>();
    }

    public void BackToMainMenu()
    {
        //If map is edited prompt to save the map
        SceneManager.LoadScene("MainMenuScene");
    }

    public void SaveMap()
    {
        //Initiate function in save/load script
        _moduleManager.ResetPrefab();
        _slmanager.SaveMap();
    }

    public void LoadMap()
    {
        //Discard current modules and load new ones based on the selected file
        //_moduleManager.ResetPrefab();
        _slmanager.LoadMap();
    }
}

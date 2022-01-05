using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIMenu : MonoBehaviour
{
    public Button btnSaveMap;

    private MapValidator _validator;
    private MapValidator.MapState _mapState;

    private void Start()
    {
        _validator = FindObjectOfType<MapValidator>();
    }

    void Update()
    {
        //Check if map is validated
        _mapState = _validator.MapStateValue;
        btnSaveMap.interactable = _mapState == MapValidator.MapState.Validated;
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    public void SaveMap()
    {
        //Save current modules infos in JSON file
        Debug.Log("Map saved");
    }

    public void LoadMap()
    {
        //Discard current modules and load new ones based on the selected file
        Debug.Log("Map loaded");
    }
}

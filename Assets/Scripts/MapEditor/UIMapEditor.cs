using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMapEditor : MonoBehaviour
{
    public Button validateButton;
    public ModuleManager moduleManager;
    private GameManager _gameManager;
    private MapValidator _mapValidator;

    // Start is called before the first frame update
    void Start()
    {
        _mapValidator = FindObjectOfType<MapValidator>();
        _gameManager = FindObjectOfType<GameManager>();
    }

    public void SelectModule(int moduleIndex)
    {
        if (moduleManager.SelectedPrefab != null)
        {
            moduleManager.ResetPrefab();
        }
        if (moduleManager.IsDeleteMode)
        {
            moduleManager.IsDeleteMode = false;
        }
        moduleManager.SelectedPrefab = _gameManager.Modules[moduleIndex];
    }

    public void DeleteMode()
    {
        if (moduleManager.SelectedPrefab != null)
        {
            moduleManager.ResetPrefab();
        }
        moduleManager.IsDeleteMode = !moduleManager.IsDeleteMode;
    }

    public void ValidateMap()
    {
        _mapValidator.MapStateValue = MapValidator.MapState.Validated;
        if (moduleManager.IsDeleteMode)
        {
            moduleManager.IsDeleteMode = false;
        }

        _mapValidator.StartValidation();
    }

    public void DisplayHelp()
    {
        Debug.Log("HELP");
    }
}

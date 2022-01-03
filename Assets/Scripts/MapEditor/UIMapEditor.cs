using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMapEditor : MonoBehaviour
{
    public Button validateButton;
    public ModuleManager moduleManager;
    private GameManager _gameManager;



    // Start is called before the first frame update
    void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
    }

    void Update()
    {
        if (moduleManager.NbStartModule > 0 && moduleManager.NbFinishModule > 0)
        {
            validateButton.interactable = true;
        }
        else
        {
            validateButton.interactable = false;
        }
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
}

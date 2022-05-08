using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LoadMap : MonoBehaviour
{
    public bool isEditor = true;

    private GameManager _gameManager;
    private string _mapName;

    private void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        _mapName = this.GetComponentInChildren<TMP_Text>().text;
    }

    public void Load()
    {
        _gameManager.SelectedMapName = _mapName;
        _gameManager.SelectedMapId = 500; //Change with real map ID 
        if (isEditor)
        {
            SceneManager.LoadScene("MapEditorScene"); 
        }
        else
        {
            SceneManager.LoadScene("GameScene");
        }
    }
}

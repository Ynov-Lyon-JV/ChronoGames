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
        Debug.Log(_mapName);
    }

    public void Load()
    {
        _gameManager.SelectedMapName = _mapName;
        if (isEditor)
        {
            SceneManager.LoadScene("MapEditorScene"); 
        }
        else
        {
            // Charger la scene de jeu
            SceneManager.LoadScene("GameScene");
        }

    }
}

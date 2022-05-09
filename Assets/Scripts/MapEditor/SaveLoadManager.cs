using Newtonsoft.Json;
using System;
using System.IO;
using TMPro;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    private const string EDITED_CHRONOGAME_FOLDER = "chronogame";
    private const string EDITED_MAP_FOLDER = "maps";
    private const string EDITED_EDITED_FOLDER = "edited";
    private const string JSON_EXTENSION = ".json";

    private string _documentPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
    private string _editedMapPath;
    private string _mapName;
    private string _finalPath;

    private ModuleManager _moduleManager;

    private bool _isMapValidated;

    public Transform savePanel;
    public Transform fileAlreadyExistsPanel;
    public TMP_InputField mapNameSave;

    public bool IsMapValidated { get => _isMapValidated; set => _isMapValidated = value; }

    private void Start()
    {
        savePanel.gameObject.SetActive(false);
        fileAlreadyExistsPanel.gameObject.SetActive(false);
        _editedMapPath = Path.Combine(_documentPath, EDITED_CHRONOGAME_FOLDER, EDITED_MAP_FOLDER, EDITED_EDITED_FOLDER);

        _moduleManager = FindObjectOfType<ModuleManager>();

        _mapName = FindObjectOfType<GameManager>().SelectedMapName;
        if (!string.IsNullOrEmpty(_mapName))
        {
            LoadMap();
        }
    }

    public void SaveMap()
    {
        //Disable blocks placements etc...
        _moduleManager.enabled = false;

        //Ask for map name
        savePanel.gameObject.SetActive(true);
    }

    public void CheckIfFileExists()
    {
        _mapName = mapNameSave.text.ToLower();
        _finalPath = Path.Combine(_editedMapPath, _mapName + JSON_EXTENSION);

        //Check if file already exists
        if (File.Exists(_finalPath))
        {
            //Prompt "File already existing"
            fileAlreadyExistsPanel.gameObject.SetActive(true);
        }
        else
        {
            ValidateSave();
        }
    }

    public void ValidateSave()
    { 
        //Gather all ObjectData objects
        ObjectData[] objects = FindObjectsOfType<ObjectData>();
        MapData mapData = new MapData();

        for (int i = 0; i < objects.Length; i++)
        {
            ObjectInfo info = new ObjectInfo();
            info.Id = objects[i].ObjectId;
            info.RankX = objects[i].RankX;
            info.RankZ = objects[i].RankZ;
            info.Rotation = objects[i].Rotation;
            mapData.ListObjectInfos.Add(info);
        }

        mapData.Index = -5;
        mapData.Validated = _isMapValidated;

        string jsonString = JsonConvert.SerializeObject(mapData);
        File.WriteAllText(_finalPath, jsonString);

        _moduleManager.enabled = true;
        savePanel.gameObject.SetActive(false);
        fileAlreadyExistsPanel.gameObject.SetActive(false);
    }

    public void CancelFileAlreadyExists()
    {
        fileAlreadyExistsPanel.gameObject.SetActive(false);
    }

    public void CancelSave()
    {
        savePanel.gameObject.SetActive(false);
        _moduleManager.enabled = true;
    }

    public void LoadMap()
    {
        _moduleManager.enabled = false;

        _finalPath = Path.Combine(_editedMapPath, _mapName + JSON_EXTENSION);

        string json = File.ReadAllText(_finalPath);
        MapData data = JsonConvert.DeserializeObject<MapData>(json);

        IsMapValidated = data.Validated;

        _moduleManager.InstantiateMap(data.ListObjectInfos);
        _moduleManager.enabled = true;
    }
}

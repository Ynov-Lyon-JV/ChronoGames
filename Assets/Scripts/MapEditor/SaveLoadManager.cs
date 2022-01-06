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

    public Transform savePanel;
    public Transform loadPanel;
    public TMP_InputField mapNameSave;
    public TMP_InputField mapNameLoad;

    private void Start()
    {
        savePanel.gameObject.SetActive(false);
        loadPanel.gameObject.SetActive(false);
        _editedMapPath = Path.Combine(_documentPath, EDITED_CHRONOGAME_FOLDER, EDITED_MAP_FOLDER, EDITED_EDITED_FOLDER);

        _moduleManager = FindObjectOfType<ModuleManager>();
    }

    public void SaveMap()
    {
        //Disable blocks placements etc...
        _moduleManager.enabled = false;

        //Ask for map name
        savePanel.gameObject.SetActive(true);
    }

    public void ValidateSave()
    {
        _mapName = mapNameSave.text;
        _finalPath = Path.Combine(_editedMapPath, _mapName + JSON_EXTENSION);

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

        string jsonString = JsonConvert.SerializeObject(mapData);
        File.WriteAllText(_finalPath, jsonString);

        _moduleManager.enabled = true;
        savePanel.gameObject.SetActive(false);
    }


    public void CancelSave()
    {
        mapNameLoad.text = String.Empty;
        savePanel.gameObject.SetActive(false);
        _moduleManager.enabled = true;
    }

    public void LoadMap()
    {
        //Disable blocks placements etc...
        _moduleManager.enabled = false;

        //Ask for map name
        loadPanel.gameObject.SetActive(true);
    }

    public void ValidateLoad()
    {
        _mapName = mapNameLoad.text;
        _finalPath = Path.Combine(_editedMapPath, _mapName + JSON_EXTENSION);

        string json = File.ReadAllText(_finalPath);
        MapData data = JsonConvert.DeserializeObject<MapData>(json);

        _moduleManager.InstantiateMap(data.ListObjectInfos);
        _moduleManager.enabled = true;
        loadPanel.gameObject.SetActive(false);
    }

    public void CancelLoad()
    {
        _moduleManager.enabled = true;
        loadPanel.gameObject.SetActive(false);
    }
}

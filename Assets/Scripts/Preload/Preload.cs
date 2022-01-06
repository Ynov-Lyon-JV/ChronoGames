using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Preload : MonoBehaviour
{
    // IMPORTANT CE SCRIPT EST A SUPPRIMER UNE FOIS QUE LE LAUNCHER S'OCCUPE DE CREER LES DOSSIERS NECESSAIRES
    private const string EDITED_CHRONOGAME_FOLDER = "chronogame";
    private const string EDITED_MAP_FOLDER = "maps";
    private const string EDITED_EDITED_FOLDER = "edited";

    private string _documentPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
    private string _editedMapPath;

    // Start is called before the first frame update
    void Start()
    {
        _editedMapPath = Path.Combine(_documentPath, EDITED_CHRONOGAME_FOLDER);
        
        if (!Directory.Exists(_editedMapPath))
        {
            Directory.CreateDirectory(_editedMapPath);
        }

        _editedMapPath = Path.Combine(_editedMapPath, EDITED_MAP_FOLDER);

        if (!Directory.Exists(_editedMapPath))
        {
            Directory.CreateDirectory(_editedMapPath);
        }

        _editedMapPath = Path.Combine(_editedMapPath, EDITED_EDITED_FOLDER);

        if (!Directory.Exists(_editedMapPath))
        {
            Directory.CreateDirectory(_editedMapPath);
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleManager : MonoBehaviour
{
    private GameObject _selectedPrefab;

    private Quaternion _rotation;

    private int _moduleSize = 50;
    private int _gridSize = 10;
    private Module _module;
    private Module _lastModule;
    private Module[,] _isPosTaken;

    private int _nbStartModule = 0;
    private int _nbFinishModule = 0;

    private bool _isDeleteMode = false;

    private MapValidator _validator;

    [SerializeField]
    private LayerMask _layerMask;

    public GameObject SelectedPrefab { get => _selectedPrefab; set => _selectedPrefab = value; }
    public bool IsDeleteMode { get => _isDeleteMode; set => _isDeleteMode = value; }
    public int NbStartModule { get => _nbStartModule; set => _nbStartModule = value; }
    public int NbFinishModule { get => _nbFinishModule; set => _nbFinishModule = value; }

    // Start is called before the first frame update
    void Start()
    {
        _validator = FindObjectOfType<MapValidator>();
        _rotation = Quaternion.identity;

        _isPosTaken = new Module[_gridSize, _gridSize];
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isDeleteMode)
        {
            if (_selectedPrefab != null && _selectedPrefab.name == "depart" && _nbStartModule > 0)
            {
                _selectedPrefab = null;
            }

            if (_selectedPrefab != null)
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, _layerMask))
                {
                    Vector3 position = hit.point;

                    int rankX = (int)Mathf.Floor(position.x / _moduleSize);
                    int rankZ = (int)Mathf.Floor(position.z / _moduleSize);

                    position.x = (rankX * _moduleSize) + (_moduleSize / 2);
                    position.z = (rankZ * _moduleSize) + (_moduleSize / 2);

                    if (_module == null)
                    {
                        _module = Instantiate(_selectedPrefab, position, _rotation).GetComponent<Module>();
                    }

                    _module.transform.position = new Vector3(position.x, 0, position.z);
                    _module.transform.rotation = _rotation;

                    bool isPlaceable = _isPosTaken[rankX, rankZ] == null;

                    if (_module.IsPlaceable != isPlaceable)
                        _module.IsPlaceable = isPlaceable;

                    if (isPlaceable)
                    {
                        if (Input.GetKeyDown(KeyCode.Mouse0))
                        {
                            _isPosTaken[rankX, rankZ] = _module;

                            _module.enabled = false;
                            _module = null;


                            if (_selectedPrefab.name == "depart")
                            {
                                _nbStartModule++;
                            }
                            else if (_selectedPrefab.name == "arrivee")
                            {
                                _nbFinishModule++;
                            }

                            _validator.MapStateValue = MapValidator.MapState.Edited;
                        }
                    }

                    if (Input.GetKeyDown(KeyCode.R))
                    {
                        RotateModule();
                    }

                    if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Mouse1))
                    {
                        ResetPrefab();
                    }
                }
            }
        }
        else
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, _layerMask))
            {
                Vector3 position = hit.point;

                int rankX = (int)Mathf.Floor(position.x / _moduleSize);
                int rankZ = (int)Mathf.Floor(position.z / _moduleSize);

                Module currModule = _isPosTaken[rankX, rankZ];

                bool isModulePresent = currModule != null;

                if (currModule != _lastModule)
                {
                    if (_lastModule != null)
                        _lastModule.RestoreMaterial();

                    _lastModule = currModule;
                }

                if (currModule != null)
                    currModule.IsPlaceable = !isModulePresent;

                if (isModulePresent)
                {

                    if (Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        Module module = _isPosTaken[rankX, rankZ];

                        if (module.gameObject.name == "depart(Clone)")
                            _nbStartModule--;
                        else if (module.gameObject.name == "arrivee(Clone)")
                            _nbFinishModule--;

                        _isPosTaken[rankX, rankZ] = null;
                        Destroy(module.gameObject);

                        _validator.MapStateValue = MapValidator.MapState.Edited;
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Mouse1))
            {
                _isDeleteMode = false;
            }
        }
    }

    private void RotateModule()
    {
        _rotation *= Quaternion.Euler(0, 90, 0);
    }

    public void ResetPrefab()
    {
        if (_module != null)
        {
            Destroy(_module.gameObject);
            _module = null;
        }

        if (_selectedPrefab != null)
        {
            _selectedPrefab = null;
        }
    }

    public void ResetMap()
    {
        for (int i = 0; i < _isPosTaken.Length / _gridSize; i++)
        {
            for (int j = 0; j < _isPosTaken.Length / _gridSize; j++)
            {
                if (_isPosTaken[i, j] != null)
                {
                    Destroy(_isPosTaken[i, j].gameObject);
                    _isPosTaken[i, j] = null;
                }
            }
        }

        _nbFinishModule = 0;
        _nbStartModule = 0;

        _validator.MapStateValue = MapValidator.MapState.Edited;
    }
}

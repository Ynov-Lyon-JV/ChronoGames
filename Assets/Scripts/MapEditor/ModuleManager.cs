using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleManager : MonoBehaviour
{
    private GameObject _selectedPrefab;

    [SerializeField]
    private int _moduleSize;

    private Quaternion _rotation;
    private Vector3 _position;

    private Module _module;

    private int _nbStartModule = 0;
    private int _nbFinishModule = 0;

    private bool _isDeleteMode = false;



    public GameObject SelectedPrefab { get => _selectedPrefab; set => _selectedPrefab = value; }
    public bool IsDeleteMode { get => _isDeleteMode; set => _isDeleteMode = value; }
    public int NbStartModule { get => _nbStartModule; set => _nbStartModule = value; }
    public int NbFinishModule { get => _nbFinishModule; set => _nbFinishModule = value; }

    // Start is called before the first frame update
    void Start()
    {
        _rotation = Quaternion.identity;
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

                if (Physics.Raycast(ray, out hit))
                {
                    if (_module == null)
                    {
                        _module = Instantiate(_selectedPrefab, _position, _rotation).GetComponent<Module>();
                    }

                    _position = hit.point;
                    _position.x = (Mathf.Floor(_position.x / _moduleSize) * _moduleSize) + (_moduleSize / 2);
                    _position.z = (Mathf.Floor(_position.z / _moduleSize) * _moduleSize) + (_moduleSize / 2);

                    _module.transform.position = new Vector3(_position.x, 0, _position.z);
                    _module.transform.rotation = _rotation;

                    if (_module.IsPlaceable)
                    {
                        if (Input.GetKeyDown(KeyCode.Mouse0))
                        {
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
            if (Physics.Raycast(ray, out hit))
            {
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    if (hit.collider.CompareTag("Module"))
                    {
                        if (hit.collider.transform.parent != null)
                        {
                            if (hit.collider.transform.parent.name == "depart(Clone)")
                            {
                                _nbStartModule--;
                            }
                            else if (hit.collider.transform.parent.name == "arrivee(Clone)")
                            {
                                _nbFinishModule--;
                            }
                            Destroy(hit.collider.transform.parent.gameObject); 
                        }
                        else
                        {
                            Destroy(hit.collider.gameObject);
                        }
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
}

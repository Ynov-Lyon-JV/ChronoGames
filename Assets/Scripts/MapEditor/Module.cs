using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Module : MonoBehaviour
{
    [SerializeField]
    private Material _matPlaceable;
    [SerializeField]
    private Material _matUnplaceable;

    private Renderer[] _children;

    private List<Material[]> _materials;

    private bool _isPlaceable = true;

    public bool IsPlaceable { get => _isPlaceable; set => SetIsPlaceable(value); }

    // Start is called before the first frame update
    void Awake()
    {
        _children = this.GetComponentsInChildren<Renderer>();
        _materials = new List<Material[]>();
        SaveMaterials();
        ChangeMaterial(_isPlaceable);
    }

    private void SaveMaterials()
    {
        foreach (Renderer rend in _children)
        {
            _materials.Add(rend.materials);
        }
    }

    public void RestoreMaterial()
    {
        for (int i = 0; i < _children.Length; i++)
        {
            _children[i].materials = _materials[i];
        }
    }

    private void ChangeMaterial(bool isPlaceable)
    {
        Material mat = isPlaceable ? _matPlaceable : _matUnplaceable;
        
        foreach(Renderer rend in _children)
        {
            var mats = new Material[rend.materials.Length];
            for (var j = 0; j < rend.materials.Length; j++)
            {
                mats[j] = mat;
            }
            rend.materials = mats;
        }
    }

    private void SetIsPlaceable(bool isPlaceable)
    {
        _isPlaceable = isPlaceable;
        ChangeMaterial(_isPlaceable);
    }
   
    private void OnDisable()
    {
        RestoreMaterial();
    }
}

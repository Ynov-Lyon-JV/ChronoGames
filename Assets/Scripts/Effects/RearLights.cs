using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RearLights : MonoBehaviour
{
    MeshRenderer myMeshRenderer;

    [SerializeField] private Material FeuxArri�re_mat;
    private Material FeuxArri�reEteints_mat;
    [Tooltip("Position du material de feux arri�res au sein de la liste des mat�riaux dans le mesh renderer")]
    public int FeuxArri�re_Index;

    public void ChangeLights(bool LightOn = true)
    {
        if (!FeuxArri�reEteints_mat)
        {
            myMeshRenderer = GetComponent<MeshRenderer>();
            FeuxArri�reEteints_mat = myMeshRenderer.materials[FeuxArri�re_Index];
        }

        Material[] NewMaterials = myMeshRenderer.materials;
        Material NewLight = null;
        if (LightOn) NewLight = FeuxArri�re_mat;
        else NewLight = FeuxArri�reEteints_mat;
        NewMaterials[FeuxArri�re_Index] = NewLight;
        myMeshRenderer.materials = NewMaterials;
    }
}

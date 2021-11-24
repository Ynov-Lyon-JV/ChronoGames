using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RearLights : MonoBehaviour
{
    MeshRenderer myMeshRenderer;

    [SerializeField] private Material FeuxArrière_mat;
    private Material FeuxArrièreEteints_mat;
    [Tooltip("Position du material de feux arrières au sein de la liste des matériaux dans le mesh renderer")]
    public int FeuxArrière_Index;

    public void ChangeLights(bool LightOn = true)
    {
        if (!FeuxArrièreEteints_mat)
        {
            myMeshRenderer = GetComponent<MeshRenderer>();
            FeuxArrièreEteints_mat = myMeshRenderer.materials[FeuxArrière_Index];
        }

        Material[] NewMaterials = myMeshRenderer.materials;
        Material NewLight = null;
        if (LightOn) NewLight = FeuxArrière_mat;
        else NewLight = FeuxArrièreEteints_mat;
        NewMaterials[FeuxArrière_Index] = NewLight;
        myMeshRenderer.materials = NewMaterials;
    }
}

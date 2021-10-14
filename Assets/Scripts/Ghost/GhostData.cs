using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GhostData
{
    #region Fields

    private List<SerializableVector3> positions;
    private List<SerializableQuaternion> rotations;

    #endregion

    #region Properties

    public List<SerializableVector3> Positions { get => positions; set => positions = value; }
    public List<SerializableQuaternion> Rotations { get => rotations; set => rotations = value; }

    #endregion


    public GhostData(List<SerializableVector3> _positions, List<SerializableQuaternion> _rotations)
    {
        positions = _positions;
        rotations = _rotations;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectData : MonoBehaviour
{
    [SerializeField]
    private int _objectId;
    private int _rankX;
    private int _rankZ;
    private float _rotation;

    public int ObjectId { get => _objectId; }
    public int RankX { get => _rankX; set => _rankX = value; }
    public int RankZ { get => _rankZ; set => _rankZ = value; }
    public float Rotation { get => _rotation; set => _rotation = value; }
}

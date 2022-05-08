using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapData
{
    private List<ObjectInfo> _listObjectInfos = new List<ObjectInfo>();
    private bool _validated;
    private int _index;

    public List<ObjectInfo> ListObjectInfos { get => _listObjectInfos; set => _listObjectInfos = value; }
    public bool Validated { get => _validated; set => _validated = value; } 
    public int Index { get => _index; set => _index = value;}
}

[System.Serializable]
public class ObjectInfo
{
    private int _id;
    private int _rankX;
    private int _rankZ;
    private float _rotation;

    public int Id { get => _id; set => _id = value; }
    public int RankX { get => _rankX; set => _rankX = value; }
    public int RankZ { get => _rankZ; set => _rankZ = value; }
    public float Rotation { get => _rotation; set => _rotation = value; }
}

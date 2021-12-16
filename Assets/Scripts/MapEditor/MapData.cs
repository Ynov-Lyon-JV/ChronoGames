using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapData
{
    private List<ObjectInfo> _listObjectInfos = new List<ObjectInfo>();
}

[System.Serializable]
public class ObjectInfo
{
    private int _id;
    private Vector3 _position;
    private Vector3 _rotation;
}

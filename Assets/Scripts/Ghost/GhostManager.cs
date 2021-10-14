using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class GhostManager : MonoBehaviour
{
    #region Fields
    private List<SerializableVector3> savedPositions = new List<SerializableVector3>();
    private List<SerializableQuaternion> savedRotations = new List<SerializableQuaternion>();
    private Transform playerTransform;
    
    private float timer = 0.0f;

    private bool isPlayerSpawned = false;

    private GameManager gm;

    #endregion

    #region Properties

    public bool IsPlayerSpawned { get => isPlayerSpawned; set => isPlayerSpawned = value; }
    public Transform PlayerTransform { get => playerTransform; set => playerTransform = value; }

    #endregion

    #region Unity Methods
    // Start is called before the first frame update
    void Start()
    {
        gm = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlayerSpawned)
        {
            timer += Time.deltaTime;
            if (timer >= (1 / gm.Frequence))
            {
                timer = 0.0f;
                SavePositions();
            }
        }
    }
    #endregion

    #region Private Methods

    private void SavePositions()
    {
        savedPositions.Add(playerTransform.position);
        savedRotations.Add(playerTransform.rotation);
    }

    #endregion

    #region Public Methods

    public byte[] SaveToByteArray()
    {
        byte[] bytes;

        GhostData ghData = new GhostData(savedPositions, savedRotations);
        BinaryFormatter bf = new BinaryFormatter();
        using (MemoryStream stream = new MemoryStream())
        {
            bf.Serialize(stream, ghData);
            bytes = stream.ToArray();
        }

        return bytes;
    }

    public GhostData LoadFromByteArray(byte[] data)
    {
        if (data == null)
            return default(GhostData);
        BinaryFormatter bf = new BinaryFormatter();
        using (MemoryStream ms = new MemoryStream(data))
        {
            object ghData = bf.Deserialize(ms);
            return (GhostData)ghData;
        }
    }

    #endregion
}

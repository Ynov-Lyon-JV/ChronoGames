using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockCP : Block
{
    #region Fields
    private bool isPassed = false; 
    #endregion

    #region Properties
    public Vector3 SpawnPosition { get => base.spawnPosition; set => base.spawnPosition = value; }
    public Quaternion SpawnRotation { get => base.spawnRotation; set => base.spawnRotation = value; }
    public bool IsPassed { get => isPassed; set => isPassed = value; } 
    #endregion

    #region Events
    public delegate void EventHandler();
    public event EventHandler IsLastCP;
    #endregion

    #region Unity Methods
    // Start is called before the first frame update
    void Start()
    {
        base.SetPositionAndRotation();
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            this.isPassed = true;

            IsLastCP();
        }
    } 
    #endregion
}

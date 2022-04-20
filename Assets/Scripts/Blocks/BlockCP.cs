using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockCP : Block
{
    #region Fields
    private int id = 0;
    private bool isPassed = false;
    private string checkPointTimer = "";
    #endregion

    #region Properties
    public Vector3 SpawnPosition { get => base.spawnPosition; set => base.spawnPosition = value; }
    public Quaternion SpawnRotation { get => base.spawnRotation; set => base.spawnRotation = value; }
    public bool IsPassed { get => isPassed; set => isPassed = value; }
    public string CheckPointTimer { get => checkPointTimer; set => checkPointTimer = value; }
    public int Id { get => id; set => id = value; }
    #endregion

    #region Events
    public delegate void EventHandler();
    public event EventHandler IsLastCP;
    public event EventHandler FinishLineWorkflow;
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
            IsLastCP();
            this.isPassed = true;
            FinishLineWorkflow();
        }
    } 
    #endregion
}

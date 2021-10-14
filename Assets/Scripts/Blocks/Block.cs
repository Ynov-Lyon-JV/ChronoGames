using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    #region Fields
    protected Vector3 spawnPosition;
    protected Quaternion spawnRotation;
    #endregion

    #region Protected Methods
    /// <summary>
    /// Sets the base position and rotation of the block
    /// </summary>
    protected void SetPositionAndRotation()
    {
        spawnPosition = this.transform.TransformPoint(this.transform.localPosition); // offset to set the car above ground
        spawnRotation = this.transform.rotation;
    } 
    #endregion
}

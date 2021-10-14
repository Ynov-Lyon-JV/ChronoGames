using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockFinish : Block
{
    #region Events
    public delegate void EventHandler();
    public event EventHandler FinishLinePassed; 
    #endregion

    #region Unity Methods
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            FinishLinePassed();
        }
    } 
    #endregion
}

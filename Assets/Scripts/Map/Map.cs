using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    #region Fields
    [SerializeField] private BlockStart start;
    [SerializeField] private BlockFinish finish;
    private GameObject[] cpList;
    [SerializeField] private bool isMultilap = false;
    [SerializeField] private int totLap;
    private BlockCP lastCP;
    private int currentLap = 1;
    #endregion

    #region Properties
    public BlockStart StartBlock { get => start; }
    public BlockFinish Finish { get => finish; }
    public BlockCP LastCP { get => lastCP; }
    public int TotLap { get => totLap; }
    public int CurrentLap { get => currentLap; }
    #endregion

    #region Events
    public delegate void EventHandler();
    public event EventHandler EndRace;
    public event EventHandler EndLap;
    #endregion

    #region Unity Methods
    void Awake()
    {
        cpList = GameObject.FindGameObjectsWithTag("Checkpoint");
     /* if(lastCP != null)
        {
            lastCP.LinePassed += () => FinishLineWorkflow();
        }*/

        foreach (GameObject cp in cpList)
        {
            BlockCP blockCp = (BlockCP) cp.GetComponent<BlockCP>();
            blockCp.IsLastCP += () => CheckEndRace(blockCp);
        }
        
        if (!isMultilap)
            totLap = 1;
        else
        {
            if (totLap < 2)
                totLap = 2;
        }
    } 
    #endregion

    #region Private Methods
    /// <summary>
    /// Function called when the player gets accros the finish line
    /// </summary>
    private void FinishLineWorkflow()
    {
        if (CheckCPs())
        {
           // if (currentLap == totLap)
          //  {
                EndRace();
           /* }
            else
            {
                currentLap++;
                ResetCheckpoints();
                EndLap();
            }*/
        }
    }

    /// <summary>
    /// Function called when the player gets accros a checkpoint
    /// </summary>
    /// <param name="_cp">The checkpoint passed</param>
    private void CheckEndRace(BlockCP _cp)
    {
        lastCP = _cp;
        FinishLineWorkflow();
    }

    /// <summary>
    /// Checks if the property IsPassed is set to "false" for all the checkpoints in the list
    /// </summary>
    /// <returns></returns>
    private bool CheckCPs()
    {
        foreach (GameObject cp in cpList)
        {
            BlockCP blockCp = (BlockCP)cp.GetComponent<BlockCP>();
             
            if (blockCp.IsPassed == false)
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Resets the property IsPassed for all the checkpoints 
    /// </summary>
    private void ResetCheckpoints()
    {
        for (int i = 0; i < cpList.Length; i++)
        {
            BlockCP blockCp = (BlockCP)cpList[i].GetComponent<BlockCP>();
            blockCp.IsPassed = false;
        }
    }

    #endregion
}

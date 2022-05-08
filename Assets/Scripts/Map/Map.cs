using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    #region Fields
    [SerializeField] private BlockStart start;
    private GameObject[] cpList;
    [SerializeField] private bool isMultilap = false;
    [SerializeField] private int totLap;
    private BlockCP lastCP;
    private int currentLap = 1;

    public bool isValidating = false;
    #endregion

    #region Properties
    public BlockStart StartBlock { get => start; }
    public BlockCP LastCP { get => lastCP; }
    public int TotLap { get => totLap; }
    public int CurrentLap { get => currentLap; }
    #endregion

    #region Events
    public delegate void EventHandler();
    public event EventHandler EndRace;
    public event EventHandler CheckPointTimer;
    #endregion

    #region Unity Methods
    public void SetMapDatas()
    {
        cpList = GameObject.FindGameObjectsWithTag("Checkpoint");
        start = GameObject.FindGameObjectsWithTag("DepartSpawn")[0].GetComponent<BlockStart>();

        int i = 0;
        foreach (GameObject cp in cpList)
        {
            BlockCP blockCp = (BlockCP) cp.GetComponent<BlockCP>();
            blockCp.Id = i;
            blockCp.IsLastCP += () => CheckEndRace(blockCp);
            blockCp.FinishLineWorkflow += () => FinishLineWorkflow();

            i++;
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
            if (isValidating)
            {
                FindObjectOfType<MapValidator>().ValidateMap();
            }
            else
            {
                EndRace();
            }
        }
    }

    /// <summary>
    /// Function called when the player gets accros a checkpoint
    /// </summary>
    /// <param name="_cp">The checkpoint passed</param>
    private void CheckEndRace(BlockCP _cp)
    {
        lastCP = _cp;
        BlockCP? blockCp = (BlockCP)cpList[_cp.Id].GetComponent<BlockCP>();

        if (blockCp != null && !blockCp.IsPassed && !isValidating)
        {
            // Update time checkpoint
            CheckPointTimer();
        }

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

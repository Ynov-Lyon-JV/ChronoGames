using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class APITime
{
    #region Fields
    public string rank;
    public string username;
    public string time;
    public string ghostUrl;
    #endregion

    #region Constructors
    public APITime(string _rank, string _username, double _time, string _ghostUrl)
    {
        rank = _rank;
        username = _username;
        time = FormatTime(_time);
        ghostUrl = _ghostUrl;
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Formats the given float into a readable time
    /// </summary>
    /// <param name="_time">Float given by the DB</param>
    /// <returns></returns>
    private string FormatTime(double _time)
    {
        TimeSpan t = TimeSpan.FromMilliseconds(_time);
        string formatedTime = t.ToString(@"mm\:ss\:fff");
        return formatedTime;
    } 
    #endregion
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayData : MonoBehaviour
{
    #region Fields
    [SerializeField] private Text timerText;
    [SerializeField] private Text countdownText;
    [SerializeField] private Text lapDataText;
    [SerializeField] private Text lastLapTimeText;
    [SerializeField] private Text speedText;
    [SerializeField] private Text gearText;
    [SerializeField] private Image RPMBar;

    private double currTime;
    private float currCountdown;
    [SerializeField] private float maxCountdown;
    [SerializeField] private float countdownTextThreshold;

    [SerializeField] private bool isRunning = false;
    [SerializeField] private bool isCountdown = false;
    
    #endregion

    #region Properties
    public bool IsRunning { get => isRunning; set => isRunning = value; }
    public bool IsCountdown { get => isCountdown; set => isCountdown = value; }
    public Text TimerText { get => timerText; set => timerText = value; }
    public double CurrTime { get => currTime; set => currTime = value; }
    #endregion

    #region Events
    public delegate void EventHandler();
    public event EventHandler CountdownFinished;
    #endregion

    #region Unity Methods
    // Start is called before the first frame update
    void Start()
    {
        IsCountdown = false;
        countdownText.text = maxCountdown.ToString();
        currCountdown = maxCountdown;
    }

    // Update is called once per frame
    void Update()
    {
        if (isCountdown)
        {
            UpdateCountdown();
        }

        if (isRunning)
        {
            UpdateTimer();
        }
    } 
    #endregion

    #region Private Methods

    /// <summary>
    /// Formats the given float into a time string 
    /// </summary>
    /// <param name="_time">The time to format into a string</param>
    /// <returns></returns>
    private string FormatTimeToString(double _time)
    {
        int minutes = (int)_time / 60;
        int seconds = (int)_time % 60;
        float fraction = (float)_time * 1000;
        fraction %= 1000;
        return String.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, fraction);
    }

    /// <summary>
    /// Formats the given float into a string with two numbers after the coma
    /// </summary>
    /// <param name="_speed"></param>
    /// <returns></returns>
    private string FormatTwoDigits(float _speed)
    {
        return _speed.ToString("0.00");
    }

    /// <summary>
    /// Updates the countdown and its display
    /// </summary>
    private void UpdateCountdown()
    {
        if (currCountdown > 0)
        {
            currCountdown -= Time.deltaTime;
            countdownText.text = Mathf.Ceil(currCountdown).ToString();
        }
        else
        {
            CountdownFinished();
            isRunning = true;
            isCountdown = false;
        }
    }

    /// <summary>
    /// Update the timer and its display
    /// </summary>
    private void UpdateTimer()
    {
        currTime += Time.deltaTime;
        if (currTime >= countdownTextThreshold)
        {
            countdownText.text = string.Empty;
        }
        timerText.text = FormatTimeToString(currTime);
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Updates the display of the current lap
    /// </summary>
    /// <param name="_currLap">Current lap the player is on</param>
    /// <param name="_totLap">Total number of laps</param>
    public void UpdateLapData(string _currLap, string _totLap)
    {
        lapDataText.text = $"{_currLap} / {_totLap}";
    }

    /// <summary>
    /// Updates the display of last lap's time
    /// </summary>
    public void UpdateLastLapTime()
    {
        lastLapTimeText.text = $"Last lap : {FormatTimeToString(currTime)}";
    }

    /// <summary>
    /// Updates the display of the speed
    /// </summary>
    /// <param name="_speed">The current speed the player has</param>
    public void UpdateSpeed(int _speed)
    {
        speedText.text = $"{_speed} km/h";
    }

    public void UpdateGear(bool isReverse, int gearNum)
    {
        gearText.text = isReverse ? "R" : (gearNum + 1).ToString();
    }

    public void UpdateRPM(float rpm)
    {
        RPMBar.fillAmount = rpm / 5000f;
    }
    #endregion
}
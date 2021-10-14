using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using System;

public class APIController : MonoBehaviour
{
    private const string _URL_TIMES_API = "https://chronogame.ydayslyon.fr/times/";

    #region Fields

    private JSONNode jsonObj;
    List<APITime> listAPITimes = new List<APITime>();
    APITime wr = null;
    APITime pb = null;

    private string apiResponseCode = string.Empty;

    private GameManager gm;

    private byte[] ghostData;

    #endregion

    #region Properties

    public List<APITime> ListAPITimes { get => listAPITimes; set => listAPITimes = value; }
    public APITime Wr { get => wr; set => wr = value; }
    public APITime Pb { get => pb; set => pb = value; }
    public string ApiResponseCode { get => apiResponseCode; set => apiResponseCode = value; }
    public byte[] GhostData { get => ghostData; set => ghostData = value; }

    #endregion

    #region Events
    public delegate void EventHandler();
    public event EventHandler ListUpdated;
    public event EventHandler WRUpdated;
    public event EventHandler PBUpdated;
    public event EventHandler ApiResponseCodeUpdated;
    public event EventHandler GhostDataUpdated;
    #endregion

    #region Coroutines
    private IEnumerator GetGhost(string url)
    {
        url = url.Replace("http", "https");
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            // Show results as text
            Debug.Log(www.downloadHandler.text);

            // Or retrieve results as binary data
            ghostData = www.downloadHandler.data;
            GhostDataUpdated();
        }
    }

    private IEnumerator GetTimes(int mapId, int carId, string username, int limit, int offset)
    {
        string apiURL = _URL_TIMES_API + $"?map={mapId}&car={carId}";

        if (username != null)
        {
            apiURL += $"&username={username}";
        }

        apiURL += $"&limit={limit}&offset={offset}";


        UnityWebRequest apiRequest = UnityWebRequest.Get(apiURL);
        apiRequest.SetRequestHeader("Authorization", $"Token {gm.UserToken}");
        yield return apiRequest.SendWebRequest();

        if (apiRequest.isNetworkError || apiRequest.isHttpError)
        {
            Debug.LogError(apiRequest.error);
            yield break;
        }

        jsonObj = JSON.Parse(apiRequest.downloadHandler.text);
        ParseAPIJSON();
    }

    private IEnumerator GetTime(int mapId, int carId, string username)
    {
        string apiURL = _URL_TIMES_API + $"?map={mapId}&car={carId}";

        if (username != null)
        {
            apiURL += $"&username={username}";
        }
        apiURL += $"&limit=1&offset=0";

        UnityWebRequest apiRequest = UnityWebRequest.Get(apiURL);
        apiRequest.SetRequestHeader("Authorization", $"Token {gm.UserToken}");
        yield return apiRequest.SendWebRequest();

        if (apiRequest.isNetworkError || apiRequest.isHttpError)
        {
            Debug.LogError(apiRequest.error);
            yield break;
        }

        jsonObj = JSON.Parse(apiRequest.downloadHandler.text);
        ParseAPIJSON(username != null ? "PB" : "WR");
    }

    private IEnumerator PostTime(int mapId, int carId, double time, byte[] file)
    {
        string apiURL = _URL_TIMES_API;
        float timeFloat = Convert.ToSingle(time * 1000);
        int timeInt = Mathf.FloorToInt(timeFloat);

        string token = $"Token {gm.UserToken}";

        WWWForm form = new WWWForm();
        form.AddField("map", mapId);
        form.AddField("car", carId);
        form.AddField("time", timeInt.ToString());
        form.AddBinaryData("ghost", file, $"{mapId}_{carId}_{timeInt}_{DateTime.Today}");

        UnityWebRequest apiRequest = UnityWebRequest.Post(apiURL, form);
        apiRequest.SetRequestHeader("Authorization", token);
        yield return apiRequest.SendWebRequest();

        if (apiRequest.isNetworkError || apiRequest.isHttpError)
        {
            Debug.LogError(apiRequest.error);
        }
        else
        {
            apiResponseCode = apiRequest.responseCode.ToString();
            ApiResponseCodeUpdated();
        }
    }

    #endregion

    #region Private Methods
    private void ParseAPIJSON()
    {
        foreach (JSONNode node in jsonObj["results"])
        {
            APITime newTime = new APITime(node["rank"], node["username"], double.Parse(node["time"]), node["ghost"]);
            listAPITimes.Add(newTime);
        }
        ListUpdated();
    }

    private void ParseAPIJSON(string type)
    {
        foreach (JSONNode node in jsonObj["results"])
        {
            if (type == "WR")
            {
                wr = new APITime(node["rank"], node["username"], double.Parse(node["time"]), node["ghost"]);
                WRUpdated();
            }
            else if (type == "PB")
            {
                pb = new APITime(node["rank"], node["username"], double.Parse(node["time"]), node["ghost"]);
                PBUpdated();
            }
        }
        if (jsonObj["count"] == 0)
        {
            if (type == "WR")
            {
                wr = new APITime("-1", "", 0.0f, "");
                WRUpdated();
            }
            else if (type == "PB")
            {
                pb = wr = new APITime("-1", "", 0.0f, "");
                PBUpdated();
            }
        }
    }
    #endregion

    #region Unity Methods

    private void Start()
    {
        gm = FindObjectOfType<GameManager>();
    }

    #endregion

    #region Public Methods

    public void SendTimeToApi(int mapId, int carId, double time, byte[] file)
    {
        StartCoroutine(PostTime(mapId, carId, time, file));
    }

    public void GetTimesFromAPI(int mapId, int carId, string username, int limit, int offset)
    {
        listAPITimes.Clear();
        StartCoroutine(GetTimes(mapId, carId, username, limit, offset));
    }

    public void GetTimeFromAPI(int mapId, int carId, string username)
    {
        StartCoroutine(GetTime(mapId, carId, username));
    }

    public void GetGhostFromAPI(string url)
    {
        StartCoroutine(GetGhost(url));
    }

    #endregion
}
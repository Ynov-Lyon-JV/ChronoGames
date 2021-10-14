using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System;
using System.Text.RegularExpressions;
using SimpleJSON;
using UnityEngine.SceneManagement;

public class Login : MonoBehaviour
{
    #region Fields
    public GameObject username;
    public GameObject password;
    public Text ErrorMessage;
    private string Username;
    private string Password;
    private const string _AUTH_API = "https://chronogame.ydayslyon.fr/auth/login/";

    private GameManager gm;
    #endregion

    #region Unity Methods

    void Start()
    {
        gm = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (username.GetComponent<InputField>().isFocused)
            {
                password.GetComponent<InputField>().Select();
            }
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (Password != string.Empty && Password != string.Empty)
            {
                LoginButton();
            }
        }
        Username = username.GetComponent<InputField>().text;
        Password = password.GetComponent<InputField>().text;
    }

    #endregion

    #region Public Methods

    public void LoginButton()
    {
        if (Username == "")
        {
            Debug.LogWarning("Username Field Empty");
        }
        else if (Password == "")
        {
            Debug.LogWarning("Password Field Empty");
        }
        else
        {
            //Faire la requete de connexion
            StartCoroutine(Authenticate());
        }
    }

    public IEnumerator Authenticate()
    {
        string apiURL = _AUTH_API;
        WWWForm form = new WWWForm();
        form.AddField("username", Username);
        form.AddField("password", Password);

        UnityWebRequest apiRequest = UnityWebRequest.Post(apiURL, form);
        yield return apiRequest.SendWebRequest();


        if (apiRequest.isNetworkError || apiRequest.isHttpError)
        {
            Debug.LogError(apiRequest.error);
            ErrorMessage.text = "Identifants incorrects";
        }
        else
        {
            if (apiRequest.responseCode == 200)
            {
                JSONNode jsonObj = JSON.Parse(apiRequest.downloadHandler.text); ;
                string userToken = jsonObj["token"];
                gm.UserToken = userToken;
                Debug.Log(userToken);
                gm.UserName = Username;
                SceneManager.LoadScene("MainMenuScene");
            }
        }
    }

    #endregion

}

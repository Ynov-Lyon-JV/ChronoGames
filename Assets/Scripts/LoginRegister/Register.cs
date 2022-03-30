using SimpleJSON;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class Register : MonoBehaviour
{
    public GameObject username;
    public GameObject email;
    public GameObject password;
    public GameObject confPassword;
    private string Username;
    private string Email;
    private string Password;
    private string ConfPassword;
    private string form;
    private bool EmailValid = false;
    private string[] Characters = {"a","b","c","d","e","f","g","h","i","j","k","l","m","n","o","p","q","r","s","t","u","v","w","x","y","z",
                                   "A","B","C","D","E","F","G","H","I","J","K","L","M","N","O","P","Q","R","S","T","U","V","W","X","Y","Z",
                                   "1","2","3","4","5","6","7","8","9","0","_","-"};
    private const string _AUTH_API = "https://chronogame.ydayslyon.fr/auth/register/";

    public void RegisterButton()
    {
        bool UN = false;
        bool EM = false;
        bool PW = false;
        bool CPW = false;

        if (Username == "")
        {
            Debug.LogWarning("Username field Empty");
        }
        else
        {
            UN = true;
        }
        if (Email != "")
        {
            EmailValidation();
            if (EmailValid)
            {
                if (Email.Contains("@"))
                {
                    if (Email.Contains("."))
                    {
                        EM = true;
                    }
                    else
                    {
                        Debug.LogWarning("Email is Incorrect");
                    }
                }
                else
                {
                    Debug.LogWarning("Email is Incorrect");
                }
            }
            else
            {
                Debug.LogWarning("Email is Incorrect");
            }
        }
        else
        {
            Debug.LogWarning("Email Field Empty");
        }
        if (Password != "")
        {
            if (Password.Length > 5)
            {
                PW = true;
            }
            else
            {
                Debug.LogWarning("Password Must Be atleast 6 Characters long");
            }
        }
        else
        {
            Debug.LogWarning("Password Field Empty");
        }
        if (ConfPassword != "")
        {
            if (ConfPassword == Password)
            {
                CPW = true;
            }
            else
            {
                Debug.LogWarning("Passwords Don't Match");
            }
        }
        else
        {
            Debug.LogWarning("Confirm Password Field Empty");
        }
        if (UN == true && EM == true && PW == true && CPW == true)
        {

            //Requete inscription

            StartCoroutine(CreateAccount());


            print("Registration Complete");
        }

    }

    public IEnumerator CreateAccount()
    {
        string apiURL = _AUTH_API;
        WWWForm form = new WWWForm();
        form.AddField("username", Username);
        form.AddField("email", Email);
        form.AddField("password", Password);
        form.AddField("password2", ConfPassword);

        UnityWebRequest apiRequest = UnityWebRequest.Post(apiURL, form);
        yield return apiRequest.SendWebRequest();


        if (apiRequest.isNetworkError || apiRequest.isHttpError)
        {
            Debug.LogError(apiRequest.error);
        }
        else
        {
            Debug.Log(apiRequest.responseCode);
            JSONNode jsonObj = JSON.Parse(apiRequest.downloadHandler.text); ;
            string userToken = jsonObj["token"];
            //TODO : Enregistrer le token pour la session de jeu
            Debug.Log(userToken);
            username.GetComponent<TMP_InputField>().text = "";
            email.GetComponent<TMP_InputField>().text = "";
            password.GetComponent<TMP_InputField>().text = "";
            confPassword.GetComponent<TMP_InputField>().text = "";
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (username.GetComponent<TMP_InputField>().isFocused)
            {
                email.GetComponent<TMP_InputField>().Select();
            }
            if (email.GetComponent<TMP_InputField>().isFocused)
            {
                password.GetComponent<TMP_InputField>().Select();
            }
            if (password.GetComponent<TMP_InputField>().isFocused)
            {
                confPassword.GetComponent<TMP_InputField>().Select();
            }
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (Password != "" && Email != "" && Password != "" && ConfPassword != "")
            {
                RegisterButton();
            }
        }
        Username = username.GetComponent<TMP_InputField>().text;
        Email = email.GetComponent<TMP_InputField>().text;
        Password = password.GetComponent<TMP_InputField>().text;
        ConfPassword = confPassword.GetComponent<TMP_InputField>().text;
    }

    void EmailValidation()
    {
        bool SW = false;
        bool EW = false;
        for (int i = 0; i < Characters.Length; i++)
        {
            if (Email.StartsWith(Characters[i]))
            {
                SW = true;
            }
        }
        for (int i = 0; i < Characters.Length; i++)
        {
            if (Email.EndsWith(Characters[i]))
            {
                EW = true;
            }
        }
        if (SW == true && EW == true)
        {
            EmailValid = true;
        }
        else
        {
            EmailValid = false;
        }

    }
}

using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class ResetPassword : MonoBehaviour
{
    public GameObject email;
    private string Email;
    public TMP_Text ErrorMessage;
    private bool EmailValid = false;
    private string[] Characters = {"a","b","c","d","e","f","g","h","i","j","k","l","m","n","o","p","q","r","s","t","u","v","w","x","y","z",
                                   "A","B","C","D","E","F","G","H","I","J","K","L","M","N","O","P","Q","R","S","T","U","V","W","X","Y","Z",
                                   "1","2","3","4","5","6","7","8","9","0","_","-"};
    private const string _AUTH_API = "https://chronogame.ydayslyon.fr/api/auth/password_reset/";

    public void ResetPasswordButton()
    {
        bool EM = false;
        Email = email.GetComponent<TMP_InputField>().text;

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

        if (EM == true)
        {

            //Requete inscription
            StartCoroutine(ResetUserPassword());
        }

    }

    public IEnumerator ResetUserPassword()
    {
        string apiURL = _AUTH_API;
        WWWForm form = new WWWForm();
        form.AddField("email", Email);

        UnityWebRequest apiRequest = UnityWebRequest.Post(apiURL, form);

        if (apiRequest.result == UnityWebRequest.Result.ConnectionError || apiRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            ErrorMessage.color = new Color(0.905f, 0.243f, 0.125f); //Orange
            ErrorMessage.text = apiRequest.error;
        }
        else
        {
            ErrorMessage.color = new Color(0.412f, 0.906f, 0.125f); //Vert
            ErrorMessage.text = "Reset mail sent";
            email.GetComponent<TMP_InputField>().text = "";
        }

        yield return apiRequest.SendWebRequest();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (Email != "")
            {
                ResetPasswordButton();
            }
        }
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

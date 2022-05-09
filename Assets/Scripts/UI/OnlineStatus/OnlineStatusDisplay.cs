using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class OnlineStatusDisplay : MonoBehaviour
{
    #region Fields

    private GameManager gameManager;
    private Image OnlineStatusImage;
    private TextMeshProUGUI OnlineStatusText;
    //private Button OnlineStatusButton;

    #endregion
    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        OnlineStatusImage = this.GetComponentInChildren<Image>();
        OnlineStatusText = this.GetComponentInChildren<TextMeshProUGUI>();
        //OnlineStatusButton = this.GetComponentInChildren<Button>();

        UpdateOnlineStatus();
    }

    #region Private Methods

    private void UpdateOnlineStatus()
    {
        if (gameManager.IsConnectedToInternet)
        {
            OnlineStatusImage.color = new Color(0.1137255f, 0.6313726f, 0);
            OnlineStatusText.text = $"Online ({gameManager.UserName})";
            //OnlineStatusButton.interactable = false;
        }
        else
        {
            //OnlineStatusImage.color = new Color(0.6313726f, 0, 0);
            //OnlineStatusText.text = "Offline";
            //OnlineStatusButton.interactable = true;
        }
    }
    
    #endregion

    #region Public Methods

    public void TestConnectivity()
    {
        UnityWebRequest www = new UnityWebRequest("http://google.com");
        if (www.error != null)
        {
            gameManager.IsConnectedToInternet = false;
        }
        else
        {
            gameManager.IsConnectedToInternet = true;
        }

        UpdateOnlineStatus();
    }

    #endregion
}

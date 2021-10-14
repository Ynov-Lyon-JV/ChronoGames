using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIUtilityScript : MonoBehaviour
{
    #region Public methods
    
    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    #endregion
}

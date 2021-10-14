using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginAnimManager : MonoBehaviour
{
    private GameObject panLogin;
    private GameObject panRegister;
    private GameObject panResetPassword;
    private PanelOpener loginPanelOpener;
    private PanelOpener registerPanelOpener;
    private PanelOpener resetPasswordPanelOpener;

    private void Start()
    {
        panLogin = GameObject.Find("Login");
        loginPanelOpener = panLogin.GetComponent<PanelOpener>();

        panRegister = GameObject.Find("Register");
        registerPanelOpener = panRegister.GetComponent<PanelOpener>();

        panResetPassword = GameObject.Find("ResetPassword");
        resetPasswordPanelOpener = panResetPassword.GetComponent<PanelOpener>();
    }

    public void OpenCloseRegisterPanel()
    {
        if (loginPanelOpener.IsOpen)
        {
            loginPanelOpener.OpenClosePanel();
        }
        registerPanelOpener.OpenClosePanel();
    }

    public void OpenCloseResetPasswordPanel()
    {
        if (loginPanelOpener.IsOpen)
        {
            loginPanelOpener.OpenClosePanel();
        }
        resetPasswordPanelOpener.OpenClosePanel();
    }

    public void OpenCloseLoginPanel()
    {
        if (registerPanelOpener.IsOpen)
        {
            registerPanelOpener.OpenClosePanel();
        }
        if (resetPasswordPanelOpener.IsOpen)
        {
            resetPasswordPanelOpener.OpenClosePanel();
        }
        loginPanelOpener.OpenClosePanel();
    }

    public void Quit()
    {
        Application.Quit();
    }
}

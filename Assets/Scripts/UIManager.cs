using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    // UI Objects
    [Header ("Login")]
    public TMP_InputField emailInput;
    public TMP_InputField pwdInput;
    public GameObject errorBoard;
    public TMP_Text errorText;
    public Toggle loginSaver;
    
    [Header("Register")]
    public GameObject registerPanel;
    public TMP_InputField nicknameInput;
    public TMP_InputField registerEmailInput;
    public TMP_InputField registerPwdInput;
    public GameObject registerSuccessPanel;
    
    public GameObject progressPanel;
    public bool progressPanelActived = false;
    
    public int mode = 0;

    private AccountManager accountManager;
    
    void Start()
    {
        accountManager = FindObjectOfType<AccountManager>();
        LoadLoginInfo();
    }

    // 타이틀 화면

    private void SaveLoginInfo()
    {
        PlayerPrefs.SetString("Email", emailInput.text);
        PlayerPrefs.SetString("Pwd", pwdInput.text);
    }

    public void DeleteLoginInfo()
    {
        PlayerPrefs.DeleteAll();
    }

    private void LoadLoginInfo()
    {
        if (PlayerPrefs.HasKey("Email"))
        {
            emailInput.text = PlayerPrefs.GetString("Email").ToString();
            pwdInput.text = PlayerPrefs.GetString("Pwd").ToString();
            loginSaver.isOn = true;
        }
    }
    
    public void SendLoginInfo()
    {
        if(loginSaver.isOn)
            SaveLoginInfo();
        accountManager.TryLogin(emailInput.text, pwdInput.text);
    }

    public void SendRegisterInfo()
    {
        accountManager.TryRegister(registerEmailInput.text, registerPwdInput.text, nicknameInput.text);
    }

    public void SetErrorBoard(string error)
    {
        errorBoard.SetActive(true);
        errorText.text = error;
    }
    
    public void SetProgressActive()
    {
        progressPanelActived = !progressPanelActived;
        progressPanel.SetActive(progressPanelActived);
    }

    public void SetRegisterSuccess()
    {
        registerPanel.SetActive(false);
        registerSuccessPanel.SetActive(true);
    }
    
    public void SetProgressText(string target)
    {
        
    }
    
    // 로비 화면
    
    public void setModeNormal()
    {
        mode = 0;
    }

    public void setModeRank()
    {
        mode = 1;
    }
}

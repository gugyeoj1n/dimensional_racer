using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UiManager : MonoBehaviour
{
    // UI Objects
    public TMP_InputField emailInput;
    public TMP_InputField pwdInput;
    
    public GameObject progressPanel;
    public bool progressPanelActived = false;
    
    public int mode = 0;

    private AccountManager accountManager;
    
    void Start()
    {
        accountManager = FindObjectOfType<AccountManager>();
    }

    public void SendLoginInfo()
    {
        accountManager.TryLogin(emailInput.text, pwdInput.text);
    }

    public void SetProgressActive()
    {
        progressPanelActived = !progressPanelActived;
        progressPanel.SetActive(progressPanelActived);
    }
    
    public void setModeNormal()
    {
        mode = 0;
    }

    public void setModeRank()
    {
        mode = 1;
    }
}

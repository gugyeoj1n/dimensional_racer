using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;
using PlayFab.ProfilesModels;
using EntityKey = PlayFab.ProfilesModels.EntityKey;

public class ServerManager : MonoBehaviour
{
    void Start()
    {
        if(string.IsNullOrEmpty(PlayFabSettings.TitleId))
            PlayFabSettings.TitleId = "B4F2E";
        
        //TryRegister("kwooj2788@gmail.com", "woojin9821", "gugyeoj1n");
        TryLogin("kwooj2788@gmail.com", "woojin9821");
    }
    
    /// <summary>
    /// 로그인
    /// </summary>

    public void TryLogin(string email, string password)
    {
        var request = new LoginWithEmailAddressRequest
        {
            Email = email,
            Password = password
        };
        
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnError);
    }

    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("LOGIN SUCCESS");
    }
    
    
    /// <summary>
    /// 회원가입
    /// </summary>
    
    public void TryRegister(string email, string password, string username)
    {
        var request = new RegisterPlayFabUserRequest
        {
            Email = email,
            Password = password,
            Username = username
        };
        
        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnError);
    }

    private void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        Debug.Log("REGISTER SUCCESS");
    }

    private void OnError(PlayFabError error)
    {
        Debug.Log(error);
    }
}
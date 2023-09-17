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
    [HideInInspector]
    public string entityId;
    [HideInInspector]
    public string entityType;

    private FriendManager friendManager;
    
    void Start()
    {
        friendManager = FindObjectOfType<FriendManager>();
        
        if(string.IsNullOrEmpty(PlayFabSettings.TitleId))
            PlayFabSettings.TitleId = "B4F2E";
        
        //TryRegister("abc@gmail.com", "woojin9821", "test");
        TryLogin("abc@gmail.com", "woojin9821");
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

    private void GetUserData(string playfabId)
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest()
        {
            PlayFabId = playfabId,
            Keys = null
        }, result =>
        {
            Debug.Log(result.Data);
            foreach (var kvp in result.Data)
            {
                Debug.Log("Key : " + kvp.Key + " / Value : " + kvp.Value.Value);
            }
        }, (error) =>
        {
            OnError(error);
        });
    }

    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("LOGIN SUCCESS");
        GetUserData(result.PlayFabId);
        GetPlayerCurrency();
        friendManager.GetFriends();
        // 로그인 정보로 엔티티 키와 타입 저장
        entityId = result.EntityToken.Entity.Id;
        entityType = result.EntityToken.Entity.Type;
    }

    private void GetPlayerCurrency()
    {
        var request = new GetUserInventoryRequest();
        PlayFabClientAPI.GetUserInventory(request, OnGetPlayerCurrencySuccess, OnError);
    }

    private string currencyCode = "BT";
    
    private void OnGetPlayerCurrencySuccess(GetUserInventoryResult result)
    {
        int virtualCurrencyBalance = result.VirtualCurrency[currencyCode];
        Debug.Log("Player's " + currencyCode + " balance: " + virtualCurrencyBalance);
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
            Username = username,
            DisplayName = username
        };
        
        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnError);
    }

    private void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        Debug.Log("REGISTER SUCCESS");
        InitiateUserData();
    }

    private void InitiateUserData()
    {
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>()
            {
                {"rating", "0"}
            }
        }, result =>
        {
            Debug.Log("INITIATED USER DATA");
        }, error =>
        {
            OnError(error);
        });
    }

    private void OnError(PlayFabError error)
    {
        Debug.Log(error);
    }
}
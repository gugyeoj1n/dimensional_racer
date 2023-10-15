using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;
using PlayFab.ProfilesModels;
using EntityKey = PlayFab.ProfilesModels.EntityKey;
using Photon.Pun;
using Photon.Realtime;
using Photon;
using UnityEngine.SceneManagement;

public class AccountManager : MonoBehaviourPunCallbacks
{
    [HideInInspector]
    public string entityId;
    [HideInInspector]
    public string entityType;
    [HideInInspector]
    public string currentUserId;

    private UiManager uiManager;
    private FriendManager friendManager;
    [SerializeField]
    private string photonVer = "1.0.0";

    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = photonVer;
    }

    void Start()
    {
        //friendManager = FindObjectOfType<FriendManager>();
        uiManager = FindObjectOfType<UiManager>();
        
        if(string.IsNullOrEmpty(PlayFabSettings.TitleId))
            PlayFabSettings.TitleId = "B4F2E";
        
        //TryRegister("abc@gmail.com", "woojin9821", "test");
        //TryLogin("abc@gmail.com", "woojin9821");
        //TryLogin("kwooj2788@gmail.com", "woojin9821");

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
        
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginError);
        uiManager.SetProgressActive();
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
            OnLoginError(error);
        });
    }

    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("LOGIN SUCCESS");
        currentUserId = result.PlayFabId;
        UpdateLoginTime();
        GetUserData(result.PlayFabId);
        GetPlayerCurrency();
        GetPlayerRating();
        //friendManager.GetFriends();
        // 로그인 정보로 엔티티 키와 타입 저장
        entityId = result.EntityToken.Entity.Id;
        entityType = result.EntityToken.Entity.Type;
        // 포톤 서버에도 접속
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("CONNECTED TO PHOTON MASTER SERVER");
        PhotonNetwork.NickName = entityId;
        uiManager.SetProgressActive();
        SceneManager.LoadScene(1);
    }
    
    private void UpdateLoginTime()
    {
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>()
            {
                {"lastLogin", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}
            },
            Permission = UserDataPermission.Public
        }, result =>
        {
            Debug.Log("LOGIN TIME UPDATED!");
        }, OnLoginError);
    }

    public void GetPlayerCurrency()
    {
        var request = new GetUserInventoryRequest();
        PlayFabClientAPI.GetUserInventory(request, OnGetPlayerCurrencySuccess, OnLoginError);
    }

    private string currencyCode = "BT";
    
    private void OnGetPlayerCurrencySuccess(GetUserInventoryResult result)
    {
        int virtualCurrencyBalance = result.VirtualCurrency[currencyCode];
        Debug.Log("Player's " + currencyCode + " balance: " + virtualCurrencyBalance);
    }

    public void GetPlayerRating()
    {
        var request = new GetUserDataRequest();
        PlayFabClientAPI.GetUserData(request, OnGetPlayerRatingSuccess, OnLoginError);
    }

    private void OnGetPlayerRatingSuccess(GetUserDataResult result)
    {
        foreach (KeyValuePair<string, UserDataRecord> record in result.Data)
        {
            if (record.Key == "rating")
            {
                Debug.Log(record.Value.Value);
                return;
            }
        }
        
        Debug.Log("FAILED TO GET USER RATING");
    }

    public void OnLoginError(PlayFabError error)
    {
        switch (error.Error)
        {
            case PlayFabErrorCode.AccountNotFound:
                Debug.Log("해당 유저를 찾을 수 없습니다.");
                break;
            case PlayFabErrorCode.InvalidEmailOrPassword:
                Debug.Log("잘못된 계정 정보입니다.");
                break;
            case PlayFabErrorCode.InvalidEmailAddress:
                Debug.Log("잘못된 이메일 형식입니다.");
                break;
            case PlayFabErrorCode.AccountAlreadyLinked:
                Debug.Log("해당 계정으로 로그인 중인 기기가 있습니다.");
                break;
            default:
                Debug.Log("통신 중 오류가 발생했습니다. 다시 시도해 주세요.");
                break;
        }
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
                {"rating", "0"},
                {"lastLogin", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")}
            },
            Permission = UserDataPermission.Public
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
        switch (error.Error)
        {
        }
        Debug.Log(error);
    }
}
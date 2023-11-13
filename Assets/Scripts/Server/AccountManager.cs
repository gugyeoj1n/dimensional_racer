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
using PlayFab.EconomyModels;
using UnityEngine.SceneManagement;

public class AccountManager : MonoBehaviourPunCallbacks
{
    [HideInInspector] public Dictionary<int, string> playerInfoInQueue;
    
    [HideInInspector]
    public string entityId;
    [HideInInspector]
    public string entityType;
    [HideInInspector]
    public string currentUserId;
    public string userName;
    public int coin;
    public int diamond;
    public string playerIconId;

    private UIManager uiManager;
    private FriendManager friendManager;
    [SerializeField]
    private string photonVer = "1.0.0";

    private bool loginTimeResponse = false;
    private bool userNameResponse = false;
    private bool userDataResponse = false;
    private bool currencyResponse = false;
    private bool ratingResponse = false;
    private bool photonResponse = false;

    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = photonVer;
    }

    void Start()
    {
        //friendManager = FindObjectOfType<FriendManager>();
        uiManager = FindObjectOfType<UIManager>();
        
        if(string.IsNullOrEmpty(PlayFabSettings.TitleId))
            PlayFabSettings.TitleId = "B4F2E";
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
            foreach (var kvp in result.Data)
            {
                if (kvp.Key == "playerIconId")
                    playerIconId = kvp.Value.Value;
            }

            userDataResponse = true;
        }, (error) =>
        {
            OnLoginError(error);
        });
    }

    private void OnLoginSuccess(LoginResult result)
    {
        StartCoroutine(WaitForResponse());
        
        Debug.Log("LOGIN SUCCESS");
        currentUserId = result.PlayFabId;
        UpdateLoginTime();
        GetUserName(result.PlayFabId);
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

    private void GetUserName(string targetId)
    {
        PlayFabClientAPI.GetPlayerProfile(new GetPlayerProfileRequest()
        {
            PlayFabId = targetId,
            ProfileConstraints = new PlayerProfileViewConstraints()
            {
                ShowDisplayName = true
            }
        }, (result) =>
        {
            userName = result.PlayerProfile.DisplayName;
            userNameResponse = true;
        }, OnError);
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("CONNECTED TO PHOTON MASTER SERVER");
        PhotonNetwork.NickName = userName;
        photonResponse = true;
    }

    private IEnumerator WaitForResponse()
    {
        while (!(loginTimeResponse && userNameResponse && userDataResponse && currencyResponse && ratingResponse && photonResponse))
        {
            yield return null;
        }
        
        uiManager.SetProgressActive();
        SceneManager.LoadScene(2);
    }

    public void UpdatePlayerIcon(string iconId)
    {
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>()
            {
                {"playerIconId", iconId}
            },
            Permission = UserDataPermission.Public
        }, result =>
        {
            LobbyUIManager ui = FindObjectOfType<LobbyUIManager>();
            ui.SetPlayerIcon(iconId);
        }, OnError);
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
            loginTimeResponse = true;
        }, OnLoginError);
    }

    public void GetPlayerCurrency()
    {
        var request = new GetUserInventoryRequest();
        PlayFabClientAPI.GetUserInventory(request, OnGetPlayerCurrencySuccess, OnLoginError);
    }

    private void OnGetPlayerCurrencySuccess(GetUserInventoryResult result)
    {
        // 여기서 currencyCode가 CN이면 일반 코인, DM이면 캐시
        coin = result.VirtualCurrency["CN"];
        diamond = result.VirtualCurrency["DM"];
        currencyResponse = true;

        if (SceneManager.GetActiveScene().name == "Lobby")
        {
            FindObjectOfType<LobbyUIManager>().RefreshLobbyMoney();
        }
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
                ratingResponse = true;
                return;
            }
        }
        
        Debug.Log("FAILED TO GET USER RATING");
    }

    public void OnLoginError(PlayFabError error)
    {
        Debug.Log(error.ErrorMessage);
        uiManager.SetProgressActive();
        uiManager.SetErrorBoard("잘못된 계정 정보입니다.");
    }

    public void GetPlayerInventory()
    {
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), result =>
        {
            LobbyUIManager ui = FindObjectOfType<LobbyUIManager>();
            ui.SetGaragePanel(result.Inventory);
        }, error =>
        {
            Debug.Log(error.ErrorMessage);
        });
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
        uiManager.SetProgressActive();
    }

    private void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        uiManager.SetProgressActive();
        uiManager.SetRegisterSuccess();
        InitiateUserData(result.Username);
    }

    private void InitiateUserData(string name)
    {
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>()
            {
                {"rating", "0"},
                {"lastLogin", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")},
                {"username", name},
                {"playerIconId", "default"}
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
        uiManager.SetProgressActive();
        uiManager.SetErrorBoard("오류가 발생했습니다. 다시 시도해 주세요.");
    }

    public string GetWinnerNickname(int viewId)
    {
        foreach (KeyValuePair<int, string> a in playerInfoInQueue)
        {
            if (a.Key == viewId % 1000)
                return a.Value;
        }

        return null;
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.MultiplayerModels;
using Unity.VisualScripting;

public class FriendManager : MonoBehaviour
{
    private LobbyUIManager lobbyUIManager;

    public Dictionary<string, GameObject> friendPrefabList;

    void Start()
    {
        lobbyUIManager = FindObjectOfType<LobbyUIManager>();
        friendPrefabList = new Dictionary<string, GameObject>();
    }
    
    public void GetFriends()
    {
        friendPrefabList.Clear();
        lobbyUIManager.FriendPanelManage();
        PlayFabClientAPI.GetFriendsList(new GetFriendsListRequest(),
            result =>
            {
                if (result.Friends.Count != 0)
                {
                    foreach (FriendInfo friend in result.Friends)
                    {
                        friendPrefabList.Add(friend.Username, lobbyUIManager.InstantiateFriend(friend));
                        GetLastLoginOfFriend(friend.FriendPlayFabId);
                    }
                }
                else
                {
                    Debug.Log("NO FRIENDS");
                }
            }, OnError);
    }

    public void GetLastLoginOfFriend(string targetId)
    {
        var request = new GetUserDataRequest
        {
            PlayFabId = targetId,
            Keys = null
        };
        PlayFabClientAPI.GetUserData(request, OnGetUserData, OnError);
    }

    private void OnGetUserData(GetUserDataResult result)
    {
        Debug.Log("ONGETUSERDATA 실행됨");
        string name = "";
        int time = 0;
        string text = "";
        foreach (var kvp in result.Data)
        {
            if (kvp.Key == "username")
                name = kvp.Value.Value;
            
            if (kvp.Key == "lastLogin")
            {
                (time, text) = CalculateTime(GetTimeDiff(kvp.Value.Value));
            }
        }
        
        SetLastLoginText(name, string.Format("마지막 로그인 : {0}{1} 전", time, text));
    }

    private void SetLastLoginText(string name, string text)
    {
        friendPrefabList[name].transform.GetChild(1).GetComponent<TMP_Text>().text = text;
    }

    private int GetTimeDiff(string targetTime)
    {
        DateTime targetDateTime = DateTime.ParseExact(targetTime, "yyyy-MM-dd HH:mm:ss", null);
        DateTime currentDateTime = DateTime.Now;

        TimeSpan timeDifference = currentDateTime - targetDateTime;
        return (int)timeDifference.TotalMinutes;
    }

    private (int, string) CalculateTime(int target)
    {
        if (target >= 1440)
            return (target / 1440, "일");
        else if (target >= 60)
            return (target / 60, "시간");
        else
            return (target, "분");
    }

    public void SearchFriend(string target)
    {
        var request = new GetAccountInfoRequest()
        {
            Username = target
        };
        PlayFabClientAPI.GetAccountInfo(request, result =>
        {
            AddFriend(result.AccountInfo.Username);
        }, OnError);
    }

    public void AddFriend(string targetUsername)
    {
        var request = new AddFriendRequest
        {
            FriendUsername = targetUsername
        };
        
        PlayFabClientAPI.AddFriend(request, result =>
            {
                lobbyUIManager.RefreshFriend();
            },
            OnError);
    }

    public void RemoveFriend(FriendInfo target)
    {
        PlayFabClientAPI.RemoveFriend(new RemoveFriendRequest
        {
            FriendPlayFabId = target.FriendPlayFabId
        }, result =>
        {
            lobbyUIManager.RefreshFriend();
        }, OnError);
    }

    public void OnError(PlayFabError error)
    {
        lobbyUIManager.ShowError(error);
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using Unity.VisualScripting;

public class FriendManager : MonoBehaviour
{
    public void GetFriends()
    {
        PlayFabClientAPI.GetFriendsList(new GetFriendsListRequest(),
            result =>
            {
                if (result.Friends.Count != 0)
                {
                    foreach (FriendInfo friend in result.Friends)
                    {
                        // *** FriendPlayFabId -> PlayFabId 로 변환하는 과정이 필요함 ***
                        GetLastLoginOfFriend(friend.FriendPlayFabId);
                        Debug.Log(friend.Username + " : " + friend.FriendPlayFabId);
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
        foreach (var kvp in result.Data)
        {
            Debug.Log(kvp.Key);
            if (kvp.Key == "lastLogin")
            {
                Debug.Log(GetTimeDiff(kvp.Value.Value) + "분 전 로그인했습니다.");
            }
        }
    }

    private int GetTimeDiff(string targetTime)
    {
        DateTime targetDateTime = DateTime.ParseExact(targetTime, "yyyy-MM-dd HH:mm:ss", null);
        DateTime currentDateTime = DateTime.Now;

        TimeSpan timeDifference = currentDateTime - targetDateTime;
        return (int)timeDifference.TotalMinutes;
    }

    public void AddFriend(string targetUsername)
    {
        var request = new AddFriendRequest
        {
            FriendUsername = targetUsername
        };
        
        PlayFabClientAPI.AddFriend(request, result =>
            {
                Debug.Log("NEW FRIEND");
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
            Debug.Log("FRIEND REMOVED");
        }, OnError);
    }

    public void OnError(PlayFabError error)
    {
        Debug.Log(error);
    }
}
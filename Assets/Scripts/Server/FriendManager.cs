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
                (var time, string text) = CalculateTime(GetTimeDiff(kvp.Value.Value));
                Debug.LogFormat("{0}{1} 전 로그인했습니다.", time, text);
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

    private (int, string) CalculateTime(int target)
    {
        if (target >= 1440)
            return (target / 1440, "일");
        else if (target >= 60)
            return (target / 60, "시간");
        else
            return (target, "분");
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
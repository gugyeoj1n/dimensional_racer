using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using Unity.VisualScripting;

public class FriendManager : MonoBehaviour
{
    public TMP_Text friendName;
    
    public void GetFriends()
    {
        PlayFabClientAPI.GetFriendsList(new GetFriendsListRequest(),
            result =>
            {
                if (result.Friends.Count != 0)
                {
                    foreach (FriendInfo friend in result.Friends)
                    {
                        Debug.Log(friend.Username);
                    }
                }
                else
                {
                    Debug.Log("NO FRIENDS");
                }
            }, OnError);
    }

    public void Test()
    {
        GetLastLoginOfFriend(friendName.text);
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
        foreach (var kvp in result.Data)
        {
            if (kvp.Key == "lastLogin")
            {
                Debug.Log("LAST LOGIN : " + kvp.Value.Value);
            }
        }
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
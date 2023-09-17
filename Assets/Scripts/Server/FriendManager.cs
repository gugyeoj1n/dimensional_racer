using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

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
                        Debug.Log(friend.Username);
                    }
                }
                else
                {
                    Debug.Log("NO FRIENDS");
                }
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
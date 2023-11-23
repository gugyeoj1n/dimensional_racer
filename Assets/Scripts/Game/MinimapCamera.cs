using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class MinimapCamera : MonoBehaviourPunCallbacks
{
    void Start()
    {
        PhotonView[] players = FindObjectsOfType<PhotonView>();
        foreach (PhotonView player in players)
        {
            if (player.Controller == PhotonNetwork.LocalPlayer)
            {
                player.transform.GetChild(0).gameObject.SetActive(true);
            }
        }
    }
}
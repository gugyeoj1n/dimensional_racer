using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    private GameManager gameManager;
    public string playerId;
    
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    public void StartCamera()
    {
        CameraFollow cam = GetComponent<CameraFollow>();
        if (photonView.IsMine)
        {
            cam.StartFollowing();
        }
    }

    
}

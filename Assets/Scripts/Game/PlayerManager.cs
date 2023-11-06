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
        
        CameraFollow cam = GetComponent<CameraFollow>();
        if (photonView.IsMine)
        {
            cam.StartFollowing();
        }
        
        StartCoroutine(CheckRoomFull());
    }

    private IEnumerator CheckRoomFull()
    {
        while(!gameManager.isStarted)
        {
            yield return new WaitForSeconds(0.1f);
            if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
                gameManager.StartGame();
        }
    }
}

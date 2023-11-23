using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class MiniMapCamera : MonoBehaviourPunCallbacks
{
    public Transform target;
    private GameManager gameManager;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    public void SetCam()
    {
        PhotonView[] players = FindObjectsOfType<PhotonView>();
        foreach (PhotonView player in players)
        {
            if (player.Controller == PhotonNetwork.LocalPlayer)
            {
                target = player.transform;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameManager.isStarted) return;
        Vector3 position = new Vector3(target.transform.position.x, 900, target.transform.position.z);
        transform.position = position;
    }
}

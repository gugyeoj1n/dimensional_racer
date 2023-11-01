using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    void Start()
    {
        CameraFollow cam = GetComponent<CameraFollow>();
        if (photonView.IsMine)
        {
            cam.StartFollowing();
        }
    }
}

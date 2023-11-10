using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class GoalLine : MonoBehaviour
{
    private GameManager gameManager;
    private AccountManager accountManager;
    
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        accountManager = FindObjectOfType<AccountManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            int winner = other.gameObject.GetComponent<PhotonView>().ViewID;
            Debug.LogFormat("WINNER VIEWID : {0}", winner);
            //string winnerNickname = accountManager.GetWinnerNickname(winner);
            gameManager.EndGame(winner % 1000);
        }
    }
}
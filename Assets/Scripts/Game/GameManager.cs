using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;
using Photon;
using Photon.Pun;
using Photon.Realtime;
using Unity.VisualScripting;

public class GameManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    // 게임 씬에서 필요한 기능
    // 위치 재설정, 포톤 동기화, 플레이팹 카탈로그에서 자동차 아이템 가져오기, 게임 종료 후 점수 정산

    // 방에 다 들어오면 (캐릭터들 다 생성되면) 3 2 1 하고 시작해
    // 1등이 들어오면 10초 세고 끝 못들어오면 리타이어
    // 정산하고 로비로 복귀

    public bool isStarted = false;
    public float time = 300f;

    public GameObject playerPrefab;

    public void Start()
    {
        Debug.Log("현재 룸 인원수 : " + PhotonNetwork.CurrentRoom.PlayerCount);
        StartCoroutine(CheckRoomFull());
        //Vector3 startPos = Vector3.up + Vector3.right * PhotonNetwork.CurrentRoom.PlayerCount;
        //PhotonNetwork.Instantiate(playerPrefab.name, startPos, Quaternion.identity, 0);
    }

    private IEnumerator CheckRoomFull()
    {
        while (!isStarted)
        {
            Debug.Log("전원 입장 기다리는 중");
            if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    SpawnPlayers();
                }
            }

            yield return new WaitForSecondsRealtime(0.1f);
        }
    }
    
    // 여기 수정하기. 지금 두 플레이어가 반대로 움직임

    private void SpawnPlayers()
    {
        foreach (KeyValuePair<int, Player> pl in PhotonNetwork.CurrentRoom.Players)
        {
            GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, Vector3.up, Quaternion.identity, 0);
            int targetPos = (player.GetComponent<PhotonView>().ViewID % 1000 - 1) * 30;
            Debug.Log("Target Pos : " + targetPos);
            player.transform.position = Vector3.up + Vector3.right * targetPos;
            
            player.GetComponent<PhotonView>().TransferOwnership(pl.Value);
        }
        
        RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        SendOptions sendOptions = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(1, null, options, sendOptions);
    }

    public void StartGame()
    {
        isStarted = true;
        PhotonView[] players = FindObjectsOfType<PhotonView>();
        foreach (PhotonView player in players)
        {
            Debug.LogFormat("카메라 확인 : {0}", player.ViewID);
            if (player.Controller == PhotonNetwork.LocalPlayer)
            {
                player.gameObject.GetComponent<PlayerManager>().StartCamera();
            }
        }
        
        StartCoroutine(CountStart());
    }

    IEnumerator CountStart()
    {
        Debug.Log("3");
        yield return new WaitForSeconds(1f);
        Debug.Log("2");
        yield return new WaitForSeconds(1f);
        Debug.Log("1");
        yield return new WaitForSeconds(1f);
        Debug.Log("GAME START");
        UnlockInput(true);
    }

    private void UnlockInput(bool locked)
    {
        PlayerManager[] players = FindObjectsOfType<PlayerManager>();
        foreach (PlayerManager player in players)
        {
            if (player.GameObject().GetPhotonView().Controller == PhotonNetwork.LocalPlayer)
            {
                player.GameObject().GetComponent<AirplaneController>().enabled = locked;
                if(!locked)
                    Destroy(player.GameObject().GetComponent<Rigidbody>());
            }
        }
    }

    public void EndGame(int winnerId)
    {
        Debug.LogFormat("{0}번 플레이어의 승리!", winnerId);
        isStarted = false;
        UnlockInput(false);
    }

    void Update()
    {
        //CountTime();
    }

    public void CountTime()
    
    {
        if(time >= 0f)
        {
            time -= Time.deltaTime;
            Debug.Log(time);
            // 여기는 UI Manager로 넘기기
            //timeText.text = Mathf.Floor(time / 60) + ":" + Mathf.Floor(time % 60);
        }
    }
    
    IEnumerator FinishCount()
    {
        int cnt = 10;
        for (int i = 0; i < 10; i++)
        {
            yield return new WaitForSeconds(1f);
            Debug.Log(cnt);
            cnt--;
        }
        yield return new WaitForSeconds(1f);
        Debug.Log("FINISHED");
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        
        // 게임 시작
        if (eventCode == 1)
        {
            Debug.Log("EVENT CODE 1 RECEIVED");
            StartGame();
        }
    }
}

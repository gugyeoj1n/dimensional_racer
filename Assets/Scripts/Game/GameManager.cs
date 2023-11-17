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

    public bool isReady = false;
    public bool isStarted = false;
    public bool firstClear = false;
    public float time = 300f;

    public GameObject playerPrefab;

    private IngameUIManager ui;

    public void Start()
    {
        ui = FindObjectOfType<IngameUIManager>();
        
        Debug.Log("현재 룸 인원수 : " + PhotonNetwork.CurrentRoom.PlayerCount);
        StartCoroutine(CheckRoomFull());
        //Vector3 startPos = Vector3.up + Vector3.right * PhotonNetwork.CurrentRoom.PlayerCount;
        //PhotonNetwork.Instantiate(playerPrefab.name, startPos, Quaternion.identity, 0);
    }

    private IEnumerator CheckRoomFull()
    {
        while (!isReady)
        {
            Debug.Log("전원 입장 기다리는 중");
            if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    SpawnPlayers();
                    isReady = true;
                }
            }

            yield return new WaitForSecondsRealtime(0.1f);
        }
    }
    
    private void SpawnPlayers()
    {
        int cnt = 0;
        foreach (KeyValuePair<int, Player> pl in PhotonNetwork.CurrentRoom.Players)
        {
            Debug.Log("플레이어 확인 : " + pl.Value.NickName);
            
            string cartId = (string) pl.Value.CustomProperties["cartId"];
            GameObject player = PhotonNetwork.Instantiate(cartId, Vector3.up * 5f + Vector3.right * (cnt * 30f), Quaternion.identity, 0);
            //int targetPos = (player.GetComponent<PhotonView>().ViewID % 1000 - 1) * 50;
            
            player.GetComponent<PhotonView>().TransferOwnership(pl.Value);
            cnt++;
        }
        
        RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        SendOptions sendOptions = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(1, null, options, sendOptions);
    }

    public void StartGame()
    {
        PhotonView[] players = FindObjectsOfType<PhotonView>();
        foreach (PhotonView player in players)
        {
            Debug.LogFormat("카메라 확인 : {0}", player.ViewID);
            if (player.Controller == PhotonNetwork.LocalPlayer)
            {
                player.gameObject.GetComponent<PlayerManager>().StartCamera();

                ui.airplaneController = player.gameObject.GetComponent<AirplaneController>();
                ui.playerManager = player.gameObject.GetComponent<PlayerManager>();
            }
        }
        StartCoroutine(CountStart());
    }

    IEnumerator CountStart()
    {
        ui.countText.gameObject.SetActive(true);
        ui.countText.text = "3";
        yield return new WaitForSeconds(1f);
        ui.countText.text = "2";
        yield return new WaitForSeconds(1f);
        ui.countText.text = "1";
        yield return new WaitForSeconds(1f);
        ui.countText.text = "Go!";
        isStarted = true;
        UnlockInput(true);
        yield return new WaitForSeconds(1f);
        ui.countText.gameObject.SetActive(false);
    }

    private void UnlockInput(bool locked)
    {
        PlayerManager[] players = FindObjectsOfType<PlayerManager>();
        foreach (PlayerManager player in players)
        {
            if (player.GameObject().GetPhotonView().Controller == PhotonNetwork.LocalPlayer)
            {
                player.GameObject().GetComponent<AirplaneController>().enabled = locked;
                if (!locked)
                    Destroy(player.GameObject().GetComponent<Rigidbody>());
                else
                    player.GameObject().GetComponent<CameraFollow>().smoothSpeed = 10f;
            }
            player.InitParticles();
        }
    }

    public void EndGame()
    {
        //Debug.LogFormat("{0}번 플레이어의 승리!", winnerId);
        //isStarted = false;
        //UnlockInput(false);

        RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        SendOptions sendOptions = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(2, null, options, sendOptions);
    }

    IEnumerator EndCount()
    {
        ui.countText.gameObject.SetActive(true);
        for (int i = 10; i > 0; i--)
        {
            ui.countText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }
        ui.countText.text = "소코마데!";
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
        
        // 게임 종료
        else if (eventCode == 2)
        {
            StartCoroutine(EndCount());
        }
    }
}

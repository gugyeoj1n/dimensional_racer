using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using UnityEngine;
using Photon;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using Unity.VisualScripting;

public class PlayerProperty
{
    public string name;
    public string client;
    public int currentStage = 1;
}

public class GameManager : MonoBehaviourPunCallbacks, IOnEventCallback {
    public bool isReady = false;
    public bool isStarted = false;
    public bool firstClear = false;
    public float time = 0f;
    public bool isRank = false;

    public Dictionary<PlayerProperty, int> playerProperties;
    public List<int> endSequence;

    public int myId;
    
    public GameObject playerPrefab;

    public Transform spawnPoint;

    private IngameUIManager ui;
    private AccountManager accountManager;

    public void Start()
    {
        endSequence = new List<int>();
        ui = FindObjectOfType<IngameUIManager>();
        accountManager = FindObjectOfType<AccountManager>();
        
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
                }
                
                isReady = true;
            }

            yield return new WaitForSecondsRealtime(0.1f);
        }
    }

    private void SpawnPlayers()
    {
        //playerProperties = new Dictionary<PlayerProperty, GameObject>();

        string[] names = new string[PhotonNetwork.CurrentRoom.Players.Count];
        string[] clients = new string[PhotonNetwork.CurrentRoom.Players.Count];
        int[] photonViews = new int[PhotonNetwork.CurrentRoom.Players.Count];

        int cnt = 0;
        foreach (KeyValuePair<int, Player> pl in PhotonNetwork.CurrentRoom.Players)
        {
            Debug.Log("플레이어 확인 : " + pl.Value.NickName);
            
            string cartId = (string) pl.Value.CustomProperties["cartId"];
            GameObject player = PhotonNetwork.Instantiate(cartId, spawnPoint.position + Vector3.right * (cnt * 10f), Quaternion.identity, 0);
            
            player.GetComponent<PhotonView>().TransferOwnership(pl.Value);
            
            /*playerProperties.Add(new PlayerProperty
            {
                name = pl.Value.NickName,
                client = (string)pl.Value.CustomProperties["cartId"]
            }, player);*/

            names[cnt] = pl.Value.NickName;
            clients[cnt] = (string)pl.Value.CustomProperties["cartId"];
            photonViews[cnt] = player.GetComponent<PhotonView>().ViewID;
            cnt++;
        }
        
        for(int i = 0; i < names.Length; i++)
        {
            Debug.Log("정보 확인 : " + names[i] + " / " + clients[i] + " / " + photonViews[i].ToString());
        }

        object[] content = new object[] { names, clients, photonViews };
        
        RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        SendOptions sendOptions = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(1, content, options, sendOptions);
        Debug.Log("RaiseEvent 실행함");
    }

    public void StartGame()
    {
        ui.InitPlayers(playerProperties.Keys);
        
        PhotonView[] players = FindObjectsOfType<PhotonView>();
        foreach (PhotonView player in players)
        {
            Debug.LogFormat("카메라 확인 : {0}", player.ViewID);
            if (player.Controller == PhotonNetwork.LocalPlayer)
            {
                player.gameObject.GetComponent<PlayerManager>().StartCamera();
                myId = player.ViewID;
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

    public void EndGame(int winnerId)
    {
        string winnerName = "";
        foreach (KeyValuePair<PlayerProperty, int> kvp in playerProperties)
        {
            if (kvp.Value == winnerId)
            {
                winnerName = string.Format("{0} ({1})", kvp.Key.name, kvp.Key.client);
            }
        }

        object[] data = new object[] { winnerName };

        RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        SendOptions sendOptions = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(2, data, options, sendOptions);
    }

    IEnumerator EndCount(string winner)
    {
        firstClear = true;  
        ui.countText.gameObject.SetActive(true);
        for (int i = 10; i > 0; i--)
        {
            ui.countText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }
        ui.countText.text = "Over!";
        isStarted = false;
        UnlockInput(false);
        yield return new WaitForSeconds(3f);
        
        int money = 300;
        int rate = 50;
        if (endSequence.First() == myId)
        {
            money = 800;
            rate = 200;
        } else if (endSequence.Last() == myId)
        {
            rate = -100;
        }
        
        PlayFabClientAPI.AddUserVirtualCurrency(new AddUserVirtualCurrencyRequest()
        {
            Amount = money,
            VirtualCurrency = "CN"
        }, result =>
        {
            if (isRank)
            {
                int prevRating = 0;
                PlayFabClientAPI.GetUserData(new GetUserDataRequest()
                {
                    PlayFabId = accountManager.currentUserId,
                    Keys = null
                }, result =>
                {
                    foreach (var kvp in result.Data)
                    {
                        if (kvp.Key == "rating")
                        {
                            prevRating = int.Parse(kvp.Value.Value);
                            PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest
                            {
                                Data = new Dictionary<string, string>()
                                {
                                    {"rating", (rate + prevRating).ToString()}
                                },
                                Permission = UserDataPermission.Public
                            }, result =>
                            {
                                ui.SetSequencePanel(endSequence, playerProperties);
                                ui.SetSettlePanel(winner, time.ToString(), money, rate);
                            }, error => {});
                        }
                    }
                }, (error) => {});
            }
        }, error => {});
    }

    void Update()
    {
        if(isStarted)
            CountTime();
    }

    public void CountTime()
    {
        time += Time.fixedDeltaTime; 
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        
        // 게임 시작
        if (eventCode == 1)
        {
            object[] data = (object[]) photonEvent.CustomData;

            string[] names = (string[])data[0];
            string[] clients = (string[])data[1];
            int[] views = (int[])data[2];

            playerProperties = new Dictionary<PlayerProperty, int>();
            for (int i = 0; i < names.Length; i++)
            {
                playerProperties.Add(new PlayerProperty { name = names[i], client = clients[i] }, views[i]);
                Debug.Log("Property 추가 : " + names[i] + " / " + views[i]);
            }
            
            StartGame();
        }
        
        // 게임 종료
        else if (eventCode == 2)
        {
            object[] data = (object[])photonEvent.CustomData;
            StartCoroutine(EndCount((string)data[0]));
        }
    }
}

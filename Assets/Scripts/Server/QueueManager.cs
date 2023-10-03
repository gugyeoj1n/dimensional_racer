using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using PlayFab;
using PlayFab.AuthenticationModels;
using PlayFab.ClientModels;
using PlayFab.CloudScriptModels;
using PlayFab.EventsModels;
using PlayFab.MultiplayerModels;
using UnityEngine.UI;
using TMPro;
using EntityKey = PlayFab.MultiplayerModels.EntityKey;
using Photon.Pun;
using Photon;
using Photon.Realtime;
using UnityEngine.SceneManagement;


public class QueueManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public class TicketInfo
    {
        public int queueCancelTime;
        public string queueType;
    }

    public TMP_Dropdown queueTypeDropdown;
    //public TMP_Text timeText;
    
    private AccountManager accountManager;
    public TicketInfo info;
    private string currentTicketId;
    private GetMatchResult currentMatchResult;

    void Start()
    {
        accountManager = FindObjectOfType<AccountManager>();
        info = new TicketInfo();
        SetTicketInfo();
    }

    public void SetTicketInfo()
    {
        info.queueCancelTime = 60;
        info.queueType = queueTypeDropdown.options[queueTypeDropdown.value].text;
    }
    
    /// <summary>
    /// UI에서 입력을 받아와 만들어진 TicketInfo를 갖고 CreateTicket() 으로
    /// 티켓을 생성 후 매치메이킹에 참가
    /// </summary>
    
    /// <summary>
    /// 매치메이킹 (참가)
    /// </summary>

    public void CreateTicket()
    {
        SetTicketInfo();
        if (info.queueType == null || info.queueCancelTime <= 0)
        {
            Debug.Log("잘못된 티켓 정보입니다.");
            return;
        }
        
        PlayFabMultiplayerAPI.CreateMatchmakingTicket(
        new CreateMatchmakingTicketRequest
        {
            Creator = new MatchmakingPlayer
            {
                Entity = new PlayFab.MultiplayerModels.EntityKey
                {
                    Id = accountManager.entityId,
                    Type = accountManager.entityType
                },
                Attributes = new MatchmakingPlayerAttributes
                {
                    DataObject = new
                    {
                        
                    }
                }
            },
            GiveUpAfterSeconds = info.queueCancelTime,
            QueueName = info.queueType
        },
        this.OnMatchmakingTicketCreated,
        this.OnMatchmakingError);
    }

    private void OnMatchmakingTicketCreated(CreateMatchmakingTicketResult result)
    {
        currentTicketId = result.TicketId;
        Debug.Log("티켓이 생성되었습니다. 매치메이킹을 시작합니다.");
        StartCoroutine(WaitForMatch());
    }

    private void OnMatchmakingError(PlayFabError error)
    {
        Debug.Log(error);
    }

    /// <summary>
    /// 매치메이킹 (큐 잡는 중)
    /// </summary>
    private IEnumerator WaitForMatch()
    {
        float waitTime = 0;
        float checkTime = 0;
        float checkInterval = 6f;
        while (waitTime < info.queueCancelTime)
        {
            if (checkTime >= checkInterval)
            {
                CheckTicketStatus();
                checkTime = 0;
            }
            
            waitTime += Time.deltaTime;
            checkTime += Time.deltaTime;
            yield return null;
        }
    }

    public void CheckTicketStatus()
    {
        PlayFabMultiplayerAPI.GetMatchmakingTicket(
            new GetMatchmakingTicketRequest
            {
                TicketId = currentTicketId,
                QueueName = info.queueType
            },
            this.OnGetMatchmakingTicket,
            this.OnMatchmakingError);
    }

    public void RequestJoinTicket(string targetTicketId)
    {
        PlayFabMultiplayerAPI.JoinMatchmakingTicket(new JoinMatchmakingTicketRequest
        {
            TicketId = targetTicketId
        }, OnJoinMatchMakingTicket, OnJoinError);
    }

    private void OnJoinMatchMakingTicket(JoinMatchmakingTicketResult result)
    {
        
    }

    private void OnJoinError(PlayFabError error)
    {
        Debug.Log(error);
    }

    private void OnGetMatchmakingTicket(GetMatchmakingTicketResult result)
    {
        if (result.Status == "Matched")
        {
            Debug.Log("게임을 찾았습니다.");
            PlayFabMultiplayerAPI.GetMatch(
                new GetMatchRequest
                {
                    MatchId = result.MatchId,
                    QueueName = info.queueType
                },
                this.OnGetMatch,
                this.OnMatchmakingError);
        } else if (result.Status == "Canceled")
        {
            Debug.Log("매치메이킹이 취소되었습니다.");
        }
        else
        {
            Debug.Log("매치 찾는 중 ..");
        }
    }

    private void OnGetMatch(GetMatchResult result)
    {
        Debug.Log("매치메이킹에 성공했습니다.");
        if (result.Members.Count == 2)
        {
            currentMatchResult = result;
            CreatePhotonRoom();
        }
    }

    public void CreatePhotonRoom()
    {
        PhotonNetwork.CreateRoom(
            accountManager.entityId
        );
    }

    public override void OnCreatedRoom()
    {
        object[] content = new object[] { accountManager.entityId };
        RaiseEventOptions options = new RaiseEventOptions
        {
            TargetActors = GetPhotonPlayers(),
            CachingOption = EventCaching.AddToRoomCache
        };
        PhotonNetwork.RaiseEvent(1, content, options, SendOptions.SendReliable);
        SceneManager.LoadScene(1);
    }

    private int[] GetPhotonPlayers()
    {
        int[] result = new int[7];
        int idx = 0;
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            foreach (MatchmakingPlayerWithTeamAssignment matchedPlayer in currentMatchResult.Members)
            {
                if (matchedPlayer.Entity.Id == player.NickName)
                {
                    result[idx] = player.ActorNumber;
                    idx++;
                }
            }
        }

        return result;
    }
    
    public void CancelMatchMaking()
    {
        PlayFabMultiplayerAPI.CancelMatchmakingTicket(
        new CancelMatchmakingTicketRequest
        {
            QueueName = info.queueType,
            TicketId = currentTicketId
        },
        this.OnTicketCanceled,
        this.OnMatchmakingError);
    }

    public void OnTicketCanceled(CancelMatchmakingTicketResult result)
    {
        Debug.Log("매치메이킹이 취소되었습니다.");
    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == 1)
        {
            object[] data = (object[])photonEvent.CustomData;
            if (data != null)
            {
                PhotonNetwork.JoinRoom((string)data[0]);
            }
        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("ROOM JOINED");
        SceneManager.LoadScene(1);
    }
}
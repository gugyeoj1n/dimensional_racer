using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LobbyUIManager : MonoBehaviour
{
    public TMP_Text coinText;
    public TMP_Text diamondText;
    public TMP_Text userNameText;
    public TMP_Text levelText;

    public GameObject ModeSetPanel;

    public bool isFriendOpened = false;
    public GameObject friendPanel;
    public GameObject friendContent;
    public GameObject friendPrefab;
    public List<GameObject> friendList;

    public bool isMatching = false;
    public TMP_Text startButtonText;
    public GameObject matchedPanel;

    private QueueManager queueManager;
    private AccountManager accountManager;
    private FriendManager friendManager;

    void Start()
    {
        queueManager = FindObjectOfType<QueueManager>();
        accountManager = FindObjectOfType<AccountManager>();
        
        friendManager = FindObjectOfType<FriendManager>();
        friendList = new List<GameObject>();
        
        InitiateUI();
    }

    public void InitiateUI()
    {
        userNameText.text = accountManager.userName;
        coinText.text = accountManager.coin.ToString();
        diamondText.text = accountManager.diamond.ToString();
    }

    public void SetNormal()
    {
        queueManager.SetTicketInfo("normal");
        ModeSetPanel.SetActive(false);
    }

    public void SetRank()
    {
        queueManager.SetTicketInfo("rank");
        ModeSetPanel.SetActive(false);
    }

    public GameObject InstantiateFriend(string userName)
    {
        GameObject friend = Instantiate(friendPrefab, friendContent.transform);
        friend.transform.GetChild(0).GetComponent<TMP_Text>().text = userName;
        
        friendList.Add(friend);
        return friend;
    }

    private void ClearFriendList()
    {
        foreach(GameObject item in friendList)
            Destroy(item);
        friendList.Clear();
    }

    public void StartMatching()
    {
        if (!isMatching)
        {
            queueManager.CreateTicket();
            SetButtonMatching();
        }
        else
        {
            queueManager.CancelMatchMaking();
            SetButtonStart();
        }
    }

    public void SetMatchedPanel()
    {
        matchedPanel.SetActive(true);
    }

    public void SetButtonMatching()
    {
        isMatching = true;
        startButtonText.text = "매칭 취소";
    }

    public void SetButtonStart()
    {
        isMatching = false;
        startButtonText.text = "게임 찾기";
    }

    public void FriendPanelManage()
    {
        isFriendOpened = !isFriendOpened;
        
        if(isFriendOpened) ClearFriendList();
        
        friendPanel.SetActive(isFriendOpened);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}

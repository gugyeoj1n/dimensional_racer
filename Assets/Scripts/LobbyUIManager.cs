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
    public TMP_Text currentModeText;

    public bool isFriendOpened = false;
    public GameObject friendPanel;
    public GameObject friendContent;
    public GameObject friendPrefab;
    public List<GameObject> friendList;

    public List<GameObject> shopItemList;
    public GameObject shopItemPrefab;
    public GameObject shopContent;
    public bool isShopOpened = false;
    public GameObject shopPanel;

    public bool isMatching = false;
    public TMP_Text startButtonText;
    public GameObject matchedPanel;

    private QueueManager queueManager;
    private AccountManager accountManager;
    private FriendManager friendManager;
    private ShopManager shopManager;

    void Start()
    {
        queueManager = FindObjectOfType<QueueManager>();
        accountManager = FindObjectOfType<AccountManager>();
        shopManager = FindObjectOfType<ShopManager>();
        
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
        currentModeText.text = "현재 모드 : 일반";
        ModeSetPanel.SetActive(false);
    }

    public void SetRank()
    {
        queueManager.SetTicketInfo("rank");
        currentModeText.text = "현재 모드 : 랭크";
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

    public void ShopPanelManage()
    {
        isShopOpened = !isShopOpened;
        if (isShopOpened)
        {
            ClearShop();
            shopManager.RequestInventory();
        }
        shopPanel.SetActive(isShopOpened);
    }

    public void InstantiateShopItem(List<CartProduct> products)
    {
        foreach (CartProduct product in products)
        {
            GameObject shopItem = Instantiate(shopItemPrefab, shopContent.transform);
            Transform init = shopItem.transform.GetChild(0);
            init.GetChild(0).GetComponent<TMP_Text>().text = product.name;
            init.GetChild(2).GetComponent<TMP_Text>().text = product.coinPrice.ToString();
            init.GetChild(4).GetComponent<TMP_Text>().text = product.diamondPrice.ToString();
            if(product.isPurchased)
                shopItem.transform.GetChild(2).gameObject.SetActive(true);
            else
                shopItem.transform.GetChild(1).gameObject.SetActive(true);
            
            shopItemList.Add(shopItem);
        }
    }

    private void ClearShop()
    {
        foreach(GameObject item in shopItemList)
            Destroy(item);
        shopItemList.Clear();
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}

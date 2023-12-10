using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;

public class LobbyUIManager : MonoBehaviour
{
    public TMP_Text coinText;
    public TMP_Text diamondText;
    public TMP_Text userNameText;
    public TMP_Text levelText;
    public Image playerIcon;

    public Image TierImage;

    public GameObject ModeSetPanel;
    public TMP_Text currentModeText;

    public bool isFriendOpened = false;
    public GameObject friendPanel;
    public GameObject friendContent;
    public GameObject friendPrefab;
    public List<GameObject> friendList;
    public TMP_InputField friendSearchField;

    public GameObject garagePanel;
    public bool isGarageOpened = false;
    public List<GameObject> garageItemList;
    public GameObject garagePrefab;
    public Transform garageContent;
    public GameObject currentCartDisplay;
    public string currentCartId;
    public Transform displayPosition;

    public Dictionary<string, GameObject> shopItemList;
    public GameObject shopItemPrefab;
    public GameObject shopContent;
    public TMP_Text shopCoinText;
    public TMP_Text shopDiamondText;
    public bool isShopOpened = false;
    public GameObject shopPanel;
    public GameObject purchaseCheckPanel;

    public bool isMatching = false;
    public TMP_Text startButtonText;
    public GameObject matchedPanel;

    public GameObject partyPanel;
    public bool isPartyOpened;
    public Transform partyContent;
    public GameObject partyPlayer;
    public Dictionary<string, GameObject> partyPlayers;
    public TMP_Text partyMemberText;
    public TMP_Text partyCreatorText;

    public GameObject settingPanel;
    public Slider masterVolume;
    public TMP_Dropdown windowed;

    public GameObject errorPanel;

    private QueueManager queueManager;
    private AccountManager accountManager;
    private FriendManager friendManager;
    private ShopManager shopManager;
    private SettingManager settingManager;

    void Start()
    {
        queueManager = FindObjectOfType<QueueManager>();
        accountManager = FindObjectOfType<AccountManager>();
        shopManager = FindObjectOfType<ShopManager>();
        settingManager = FindObjectOfType<SettingManager>();
        
        friendManager = FindObjectOfType<FriendManager>();
        friendList = new List<GameObject>();
        shopItemList = new Dictionary<string, GameObject>();
        garageItemList = new List<GameObject>();
        partyPlayers = new Dictionary<string, GameObject>();
        
        InitiateUI();

        GarageManage();
        GarageManage();
    }

    public void InitiateUI()
    {
        userNameText.text = accountManager.userName;
        RefreshLobbyMoney();
        SetPlayerIcon(accountManager.playerIconId);
        SetTier(accountManager.rating);
    }

    public void SetPlayerIcon(string id)
    {
        playerIcon.sprite = Resources.Load<Sprite>("PlayerIcons/" + id);
    }

    public void SetTier(string rating)
    {
        Debug.Log("레이팅 : " + rating);
        Color targetColor;
        float tier = float.Parse(rating);
        if (tier <= 500) targetColor = new Color(205/255f, 127/255f, 50/255f);
        else if (tier > 500 && tier <= 1500) targetColor = new Color(192 / 255f, 192 / 255f, 192 / 255f);
        else if (tier > 1500 && tier <= 2500) targetColor = new Color(1f, 215 / 255f, 0);
        else if (tier > 2500 && tier <= 3500) targetColor = new Color(80 / 255f, 200 / 255f, 120 / 255f);
        else targetColor = new Color(185 / 255f, 242 / 255f, 1f);

        TierImage.color = targetColor;
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

    public GameObject InstantiateFriend(FriendInfo friendInfo)
    {
        GameObject friend = Instantiate(friendPrefab, friendContent.transform);
        friend.transform.GetChild(0).GetComponent<TMP_Text>().text = friendInfo.Username;
        friend.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(() =>
        {
            friendManager.RemoveFriend(friendInfo);
        });
        
        friendList.Add(friend);
        return friend;
    }

    public void SearchFriend()
    {
        if (friendSearchField.text == "") return;
        friendManager.SearchFriend(friendSearchField.text);
    }
    
    public void RefreshFriend()
    {
        FriendPanelManage();
        friendManager.GetFriends();
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
            RefreshShop();
        }
        else
        {
            accountManager.GetPlayerCurrency();
        }
        
        shopPanel.SetActive(isShopOpened);
    }

    public void RefreshLobbyMoney()
    {
        coinText.text = accountManager.coin.ToString();
        diamondText.text = accountManager.diamond.ToString();
    }

    public void RefreshShop()
    {
        ClearShop();
        shopDiamondText.text = accountManager.diamond.ToString();
        shopCoinText.text = accountManager.coin.ToString();
        shopManager.RequestInventory();
    }

    public void InstantiateShopItem(List<CartProduct> products)
    {
        foreach (CartProduct product in products)
        {
            GameObject shopItem = Instantiate(shopItemPrefab, shopContent.transform);
            Transform init = shopItem.transform.GetChild(0);
            init.GetComponent<Image>().sprite = product.capture;
            init.GetChild(0).GetComponent<TMP_Text>().text = product.name;
            init.GetChild(2).GetComponent<TMP_Text>().text = product.coinPrice.ToString();
            init.GetChild(4).GetComponent<TMP_Text>().text = product.diamondPrice.ToString();
            if(product.isPurchased)
                shopItem.transform.GetChild(2).gameObject.SetActive(true);
            else
            {
                shopItem.transform.GetChild(1).gameObject.SetActive(true);
                shopItem.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() =>
                {
                    shopManager.PurchaseItem(product.itemId, product.coinPrice);
                });
            }
            
            shopItemList.Add(product.itemId, shopItem);
        }
    }

    public void SetPurchaseCheckPanel(string text)
    {
        purchaseCheckPanel.SetActive(true);
        purchaseCheckPanel.transform.GetChild(0).GetComponent<TMP_Text>().text = text;
    }

    private void ClearShop()
    {
        foreach(GameObject item in shopItemList.Values)
            Destroy(item);
        shopItemList.Clear();
    }

    public void GarageManage()
    {
        isGarageOpened = !isGarageOpened;
        garagePanel.SetActive(isGarageOpened);
        if (isGarageOpened)
        {
            accountManager.GetPlayerInventory();
        }
    }

    public void ClearGarage()
    {
        foreach(GameObject item in garageItemList)
            Destroy(item);

        garageItemList.Clear();
    }

    public void SetGaragePanel(List<ItemInstance> items)
    {
        ClearGarage();

        foreach (ItemInstance item in items)
            InstantiateItem(item);
    }

    public void InstantiateItem(ItemInstance item)
    {
        Debug.Log("아이템 생성 : + " + item.DisplayName);
        GameObject itemPrefab = Instantiate(garagePrefab, garageContent);
        string resourcePath = "CartCaptures/" + item.ItemId + "_capture";
        itemPrefab.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>(resourcePath);

        GameObject target = Resources.Load<GameObject>(item.ItemId);

        itemPrefab.GetComponent<OnMouseWindow>().window = itemPrefab.transform.parent.parent.parent.parent.GetChild(3).gameObject;
        itemPrefab.GetComponent<OnMouseWindow>().name = item.DisplayName;
        itemPrefab.GetComponent<OnMouseWindow>().speed = target.GetComponent<AirplaneController>().speed;
        itemPrefab.GetComponent<OnMouseWindow>().accel = target.GetComponent<PlayerManager>().acceleration;
        itemPrefab.GetComponent<OnMouseWindow>().fuel = target.GetComponent<PlayerManager>().fuel;
        
        itemPrefab.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() =>
        {
            queueManager.SetCart(item.ItemId);
            SetCurrentCart(itemPrefab);
            SetCurrentDisplay(item.ItemId);
            currentCartId = item.ItemId;
        });
        
        garageItemList.Add(itemPrefab);
    }

    public void SetCurrentCart(GameObject target)
    {
        foreach (GameObject item in garageItemList)
        {
            if (item == target)
                item.GetComponent<Image>().color = new Color(1f, 78 / 255f, 78 / 255f);
            else
                item.GetComponent<Image>().color = new Color(160/255f, 160 / 255f, 160 / 255f);
        }
    }

    public void SetCurrentDisplay(string target)
    {
        Destroy(currentCartDisplay);
        GameObject display = Resources.Load<GameObject>(target + "_display");
        currentCartDisplay = Instantiate(display, displayPosition);
    }

    public void PartyManage()
    {
        isPartyOpened = !isPartyOpened;
        partyPanel.SetActive(isPartyOpened);
        if (isPartyOpened)
            InitParty();
    }

    public void InitParty()
    {
        
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void ApplySettings()
    {
        bool window = windowed.options[windowed.value].text == "창 모드";
        settingManager.SaveSettings(masterVolume.value / 100f, window);
        settingPanel.SetActive(false);
    }

    public void ShowError(PlayFabError error)
    {
        errorPanel.SetActive(true);
        string errorMessage = "";

        switch (error.Error)
        {
            case PlayFabErrorCode.UserNotFriend:
                errorMessage = "해당 유저를 찾을 수 없습니다.";
                break;
            case PlayFabErrorCode.UsersAlreadyFriends:
                errorMessage = "이미 팔로우 중입니다.";
                break;
            default:
                errorMessage = "오류가 발생했습니다. 다시 시도해 주세요.";
                break;
        }

        errorPanel.transform.GetChild(0).GetComponent<TMP_Text>().text = errorMessage;
    }
}

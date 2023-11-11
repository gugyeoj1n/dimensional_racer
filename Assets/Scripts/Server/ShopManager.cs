using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public class CartProduct
{
    public string itemId;
    public string name;
    public int coinPrice;
    public int diamondPrice;
    public GameObject model;
    public bool isPurchased;
}

public class ShopManager : MonoBehaviour
{
    private List<ItemInstance> userInven;

    private LobbyUIManager lobbyUIManager;

    void Start()
    {
        lobbyUIManager = FindObjectOfType<LobbyUIManager>();
    }
    
    public void RequestInventory()
    {
        var request = new GetUserInventoryRequest();
        PlayFabClientAPI.GetUserInventory(request, GetInventorySuccess, OnError);
    }

    public void GetInventorySuccess(GetUserInventoryResult result)
    {
        userInven = result.Inventory;
        RequestDataToServer();
    }
    
    public void RequestDataToServer()
    {
        var request = new GetCatalogItemsRequest { CatalogVersion = "cart" };
        PlayFabClientAPI.GetCatalogItems(request, GetCatalogSuccess, OnError);
    }

    private void GetCatalogSuccess(GetCatalogItemsResult result)
    {
        List<CartProduct> products = new List<CartProduct>();
        
        foreach (CatalogItem item in result.Catalog)
        {
            var customData = JsonUtility.FromJson<Dictionary<string, string>>(item.CustomData);
            
            CartProduct product = new CartProduct()
            {
                itemId = item.ItemId,
                name = item.DisplayName,
                coinPrice = Convert.ToInt32(item.VirtualCurrencyPrices["CN"]),
                diamondPrice = Convert.ToInt32(item.VirtualCurrencyPrices["DM"]),
                model = Resources.Load<GameObject>(""),
                isPurchased = CheckIsPurchased(item.DisplayName)
            };
            
            products.Add(product);
        }

        lobbyUIManager.InstantiateShopItem(products);
    }

    private bool CheckIsPurchased(string itemName)
    {
        // 인벤 리스트에서 검색 후 반환
        foreach (ItemInstance item in userInven)
            if (item.DisplayName == itemName) return true;

        return false;
    }

    public void PurchaseItem(string targetId, int price)
    {
        var request = new PurchaseItemRequest()
        {
            CatalogVersion = "cart",
            ItemId = targetId,
            Price = price,
            VirtualCurrency = "CN"
        };
        
        PlayFabClientAPI.PurchaseItem(request, result =>
        {
            lobbyUIManager.SetPurchaseCheckPanel("구매에 성공했습니다.");
            lobbyUIManager.RefreshShop();
        }, OnPurchaseError);
    }

    private void OnPurchaseError(PlayFabError error)
    {
        if (error.Error == PlayFabErrorCode.InsufficientFunds)
        {
            lobbyUIManager.SetPurchaseCheckPanel("구매에 실패했습니다.\n자금이 부족합니다.");
        }
        else
        {
            lobbyUIManager.SetPurchaseCheckPanel("구매에 실패했습니다.\n다시 시도해 주세요.");
        }
    }

    private void OnError(PlayFabError error)
    {
        Debug.Log(error.ErrorMessage);
    }
}
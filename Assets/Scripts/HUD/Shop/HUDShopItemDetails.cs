using UnityEngine;
using UnityEngine.UI;
using Undefined.Items.Shop;
using System;

using TMPro;

public class HUDShopItemDetails : MonoBehaviour {

        #region Variables

    [Header("UI Components")]
    [SerializeField] private Image itemIcon = null;
    [SerializeField] private TextMeshProUGUI itemName = null;
    [SerializeField] private TextMeshProUGUI shopMessage = null;
    [SerializeField] private Button buyButton = null;

    [Header("Gold")]
    [SerializeField] private GameObject itemPriceGoldVisible = null;
    [SerializeField] private TextMeshProUGUI itemPriceGold = null;
    [SerializeField] private TextMeshProUGUI yourGold = null;

    [Header("Secret")]
    [SerializeField] private GameObject itemPriceSecretGoldVisible = null;
    [SerializeField] private TextMeshProUGUI itemPriceSecret = null;
    [SerializeField] private TextMeshProUGUI yourSecret = null;

    // script-side variables
    private int lastGold = 0;
    private int lastSecretGold = 0;

        #endregion

    public void UpdatePlayerGold(int quantity, ShopCoin coin)
    {
        switch(coin)
        {
            case ShopCoin.Gold:
                yourGold.text = quantity.ToString();
                lastGold = quantity;
                break;
            
            case ShopCoin.Secret:
                yourSecret.text = quantity.ToString();
                lastSecretGold = quantity;
                break;

        }
    }

    public void ShowItem(ItemShop item) {

        itemIcon.sprite = item.sellingItem.icon;

        System.Random rand = new System.Random();
        int index = rand.Next(item.sellerDescriptions.Length);

        shopMessage.text = item.sellerDescriptions[index];
        itemName.text = item.sellingItem.itemName;


        itemPriceGoldVisible.SetActive(item.coinType == ShopCoin.Gold);
        itemPriceSecretGoldVisible.SetActive(item.coinType == ShopCoin.Secret);

        switch(item.coinType)
        {
            case ShopCoin.Gold:
                itemPriceGold.text = item.value.ToString();
                buyButton.interactable = lastGold >= item.value;
                break;
            
            case ShopCoin.Secret:
                itemPriceSecret.text = item.value.ToString();
                buyButton.interactable = lastSecretGold >= item.value;
                break;
        }

    }

    public void ShowItem(ShopDetails shop, bool noItems) {

        itemIcon.sprite = null;

        System.Random rand = new System.Random();
        String message = "";

        if(noItems && shop.noItemsMessage != null) {
            int index = rand.Next(shop.noItemsMessage.Length);
            message = shop.noItemsMessage[index];
        }
        else if(shop.welcomeMessage != null)
        {
            int index = rand.Next(shop.welcomeMessage.Length);
            message = shop.welcomeMessage[index];
        }

        shopMessage.text = message;
        itemName.text = shop.shopName;

        itemPriceGoldVisible.SetActive(false);
        itemPriceSecretGoldVisible.SetActive(false);

        itemPriceGold.text = "";
        itemPriceSecret.text = "";

        buyButton.interactable = false;

    }

}
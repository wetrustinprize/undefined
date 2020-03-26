using UnityEngine;
using UnityEngine.UI;
using Undefined.Items.Shop;
using System;

public class HUDShopItemDetails : MonoBehaviour {

    [Header("UI Components")]
    [SerializeField] private Image itemIcon;
    [SerializeField] private Text itemName;
    [SerializeField] private Text itemPrice;
    [SerializeField] private Text yourGold;
    [SerializeField] private Text shopMessage;
    [SerializeField] private Button buyButton;

    public void ShowItem(ItemShop item, int playerGold) {

        itemIcon.sprite = item.sellingItem.icon;

        System.Random rand = new System.Random();
        int index = rand.Next(item.sellerDescriptions.Length);

        shopMessage.text = item.sellerDescriptions[index];
        itemName.text = item.sellingItem.itemName;
        itemPrice.text = $"{item.value}G";
        yourGold.text = $"{playerGold}G";

        buyButton.interactable = playerGold >= item.value;

    }

    public void ShowItem(ShopDetails shop, bool noItems, int playerGold) {

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

        itemPrice.text = "";
        yourGold.text = $"{playerGold}G";

        buyButton.interactable = false;

    }

}
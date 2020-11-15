using UnityEngine;
using Undefined.Items;
using Undefined.Items.Shop;
using System.Collections.Generic;
using System;

public class PlayerInventory : MonoBehaviour {

        #region Variables
    [Header("Equiped Items")]        
    [SerializeField] private InventoryItem equipedActive = new InventoryItem();
    [SerializeField] private InventoryItem equipedPassive = new InventoryItem();
    [SerializeField] private InventoryItem equipedWeapon = new InventoryItem();

    [Header("Backpack")]
    [SerializeField] private List<InventoryItem> items = new List<InventoryItem>();
    [SerializeField] private int gold = 0;
    [SerializeField] private int secretGold = 0;

    // Events
    public Action<ItemObject> onEquip;
    public Action<ItemObject> onUnequip;
    public Action<ItemObject> onUse;

    public Action<int> onChangeGold;
    public Action<int> onChangeSecretGold;

    // Public acess
    public List<InventoryItem> Items { get { return items; } }
    public InventoryItem ActiveItem { get { return equipedActive; } }
    public InventoryItem PassiveItem { get { return equipedPassive; } }
    public InventoryItem WeaponItem { get { return equipedWeapon; } }
    public int Gold { get { return gold; } set { 
        SetGold(value); 
    } }
    public int SecretGold { get { return secretGold; } set { 
        SetGold(value, ShopCoin.Secret); 
    } }

        #endregion

    public void UseActiveItem() {

        if(equipedActive.itemObj == null) return;

        ItemObject usedItem = equipedActive.itemObj;

        equipedActive.itemObj.OnUse(this.gameObject);

        equipedActive.quantity--;

        if(equipedActive.quantity <= 0)
            equipedActive = new InventoryItem();
        
        onUse?.Invoke(usedItem);

    }

    public void OverrideInventory(List<InventoryItem> newItems)
    {
        items = newItems;
    }

    public void SetGold(int quantity, ShopCoin coinType, bool callAction = true) {
        switch(coinType)
        {
            case ShopCoin.Gold:
                if(callAction) onChangeGold?.Invoke(quantity);
                gold = quantity;
                break;
            
            case ShopCoin.Secret:
                if(callAction) onChangeSecretGold?.Invoke(quantity);
                secretGold = quantity;
                break;
        }
    }

    public void AddGold(int quantity, ShopCoin coinType = ShopCoin.Gold)
    {
        switch(coinType)
        {
            case ShopCoin.Gold:
                gold += quantity;
                break;
            
            case ShopCoin.Secret:
                secretGold += quantity;
                break;
        }
    }

    public void SetGold(int quantity, ShopCoin coinType = ShopCoin.Gold)
    {
        
        switch(coinType)
        {
            case ShopCoin.Gold:
                onChangeGold?.Invoke(quantity);
                gold = quantity;
                break;
            
            case ShopCoin.Secret:
                onChangeSecretGold?.Invoke(quantity);
                secretGold = quantity;
                break;
        }

    }

    public void UnequipItem(ItemType type) {

        InventoryItem item = null;

        switch(type) {
            case ItemType.Active:
                if(equipedActive.itemObj != null) { 
                    item = equipedActive;
                    equipedActive = new InventoryItem();
                }
                break;
            
            case ItemType.Passive:
                if(equipedPassive.itemObj != null) { 
                    item = equipedPassive;
                    equipedPassive = new InventoryItem();
                }
                break;

            case ItemType.Weapon:
                if(equipedWeapon.itemObj != null) {
                    item = equipedWeapon;
                    equipedWeapon = new InventoryItem();
                }
                break;
        }

        if(item == null) return;

        GameManager.Sound.PlayUISFX(item.itemObj.onUnequipSFX);

        item.itemObj.OnUnequip(this.gameObject);

        AddItem(item.itemObj, item.quantity);

        onUnequip?.Invoke(item.itemObj);

    }

    public void EquipItem(InventoryItem equipItem)
    {
        InventoryItem oldItem = null;

        switch(equipItem.itemObj.type) {
            case ItemType.Active:
                if(equipedActive.itemObj != null) { oldItem = equipedActive; }
                equipedActive = equipItem;
                break;
            
            case ItemType.Passive:
                if(equipedPassive.itemObj != null) { oldItem = equipedPassive; }
                equipedPassive = equipItem;
                break;

            case ItemType.Weapon:
                if(equipedWeapon.itemObj != null) { oldItem = equipedWeapon; }
                equipedWeapon = equipItem;
                break;
        }

        GameManager.Sound.PlayUISFX(equipItem.itemObj.onEquipSFX);

        oldItem?.itemObj.OnUnequip(this.gameObject);
        equipItem.itemObj.OnEquip(this.gameObject);
        
        onEquip?.Invoke(equipItem.itemObj);

        if(oldItem != null)
        {
            AddItem(oldItem.itemObj, oldItem.quantity);
        }

        RemoveItem(equipItem);

    }

    public void EquipItem(int index) {

        InventoryItem item = items[index];
        
        EquipItem(item);

    }

    public void AddItem(ItemObject item, int quantity = 1) {

        if(equipedActive.itemObj == item) {
            equipedActive.quantity += quantity;
            return;
        }

        int itemIndex = items.FindIndex(obj => obj.itemObj == item);

        if(itemIndex != -1)
        {
            InventoryItem i = items[itemIndex];

            if(i.itemObj.type == ItemType.Passive) {
                return;
            }

            i.quantity += quantity;
            return;
        }

        items.Add(new InventoryItem(item, quantity));

    }

    public bool HasItem(ItemObject item) {

        return items.FindIndex(obj => obj.itemObj == item) != -1;

    }

    public void RemoveItem(ItemObject item) {

        InventoryItem itemToRemove = new InventoryItem(item);
        RemoveItem(itemToRemove);

    }

    public void RemoveItem(InventoryItem item) {

        items.RemoveAll(obj => obj == item);

    }

}
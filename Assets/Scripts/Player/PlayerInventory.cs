using UnityEngine;
using Undefined.Items;
using Undefined.Sound;
using System.Collections.Generic;
using System;

public class PlayerInventory : MonoBehaviour {

        #region Variables
    [Header("Equiped Items")]        
    [SerializeField] private InventoryItem equipedActive = new InventoryItem();
    [SerializeField] private InventoryItem equipedPassive = new InventoryItem();

    [Header("Backpack")]
    [SerializeField] private List<InventoryItem> items = new List<InventoryItem>();
    [SerializeField] private int gold = 0;

    // Events
    public Action<ItemObject> onEquip;
    public Action<ItemObject> onUnequip;
    public Action<ItemObject> onUse;

    // Public acess
    public List<InventoryItem> Items { get { return items; } }
    public InventoryItem ActiveItem { get { return equipedActive; } }
    public InventoryItem PassiveItem { get { return equipedPassive; } }
    public int Gold { get { return gold; } set { gold = value; } }

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
        }

        if(item == null) return;

        SoundsManager.PlayUISFX(item.itemObj.onUnequipSFX);

        item.itemObj.OnUnequip(this.gameObject);

        AddItem(item.itemObj);

        onUnequip?.Invoke(item.itemObj);

    }

    public void EquipItem(int index) {

        InventoryItem item = items[index];
        InventoryItem oldItem = null;

        switch(item.itemObj.type) {
            case ItemType.Active:
                if(equipedActive.itemObj != null) { oldItem = equipedActive; }
                equipedActive = item;
                break;
            
            case ItemType.Passive:
                if(equipedPassive.itemObj != null) { oldItem = equipedPassive; }
                equipedPassive = item;
                break;
        }

        SoundsManager.PlayUISFX(item.itemObj.onEquipSFX);

        item.itemObj.OnEquip(this.gameObject);
        onEquip?.Invoke(item.itemObj);

        if(oldItem != null)
        {
            AddItem(oldItem.itemObj, oldItem.quantity);
        }

        RemoveItem(item);

    }

    public void AddItem(ItemObject item, int quantity = 1) {

        if(equipedActive.itemObj == item) {
            equipedActive.quantity += quantity;
            Debug.Log($"Equiped active is the same as this new item. ({item.name}) (total: {equipedActive.quantity})");
            return;
        }

        int itemIndex = items.FindIndex(obj => obj.itemObj == item);

        if(itemIndex != -1)
        {
            InventoryItem i = items[itemIndex];

            if(i.itemObj.type == ItemType.Passive) {
                Debug.Log($"Found the item in inventory, but passive items cannot be stacked. ({item.name})");
                return;
            }

            i.quantity += quantity;
            Debug.Log($"Found the item in inventory, stacking. ({item.name}) (total: {i.quantity})");
            return;
        }

        Debug.Log($"Didn't found the item in inventory, creating new one. ({item.name})");
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
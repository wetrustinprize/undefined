using UnityEngine;
using Undefined.Items;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour {

        #region Variables
    [Header("Equiped Items")]        
    [SerializeField] private InventoryItem equipedActive = new InventoryItem();
    [SerializeField] private InventoryItem equipedPassive = new InventoryItem();

    [Header("Backpack")]
    [SerializeField] private List<InventoryItem> items = new List<InventoryItem>();

    // Public acess
    public List<InventoryItem> Items { get { return items; } }
    public InventoryItem ActiveItem { get { return equipedActive; } }
    public InventoryItem PassiveItem { get { return equipedPassive; } }

        #endregion

    public void UnequipItem(ItemType type) {

        ItemObject item = null;

        switch(type) {
            case ItemType.Active:
                if(equipedActive.itemObj != null) { item = equipedActive.itemObj; AddItem(equipedActive.itemObj, equipedActive.quantity); }
                equipedActive = new InventoryItem();
                break;
            
            case ItemType.Passive:
                if(equipedPassive.itemObj != null) { item = equipedPassive.itemObj; AddItem(equipedPassive.itemObj, equipedPassive.quantity); }
                equipedPassive = new InventoryItem();
                break;
        }

        if(item == null) return;

        item.OnUnequip(this.gameObject);

    }

    public void EquipItem(int index) {

        InventoryItem item = items[index];

        switch(item.itemObj.type) {
            case ItemType.Active:
                if(equipedActive.itemObj != null) { AddItem(equipedActive.itemObj, equipedActive.quantity); }
                equipedActive = item;
                break;
            
            case ItemType.Passive:
                if(equipedPassive.itemObj != null) { AddItem(equipedPassive.itemObj, equipedPassive.quantity); }
                equipedPassive = item;
                break;
        }

        item.itemObj.OnEquip(this.gameObject);

        RemoveItem(item);

    }

    public void AddItem(ItemObject item, int quantity = -1) {

        InventoryItem newItem = new InventoryItem(item);

        int itemIndex = items.FindIndex(obj => obj == newItem);

        if(itemIndex != -1) {
            if(item.stackable)
            {
                if(quantity != -1)
                    items[itemIndex].quantity = quantity;
                else
                    items[itemIndex].quantity += 1;
            }
        }
        else
        {
            items.Add(newItem);
        }

    }

    public void RemoveItem(ItemObject item) {

        InventoryItem itemToRemove = new InventoryItem(item);
        RemoveItem(itemToRemove);

    }

    public void RemoveItem(InventoryItem item) {

        items.RemoveAll(obj => obj == item);

    }

}
using UnityEngine;
using Undefined.Items;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour {

        #region Variables

    [Header("Equiped Items")]
    [SerializeField] private Item equipedActive;
    [SerializeField] private Item equipedPassive;

    [Header("Backpack")]
    [SerializeField] private List<Item> items = new List<Item>();

    // Public acess
    public List<Item> Items { get { return items; } }

        #endregion

    public void UnequipItem(ItemType type) {

        switch(type) {
            case ItemType.Active:
                if(equipedActive != null) { AddItem(equipedActive.itemObj, equipedActive.quantity); }
                equipedActive = null;
                break;
            
            case ItemType.Passive:
                if(equipedPassive != null) { AddItem(equipedPassive.itemObj, equipedPassive.quantity); }
                equipedPassive = null;
                break;
        }

    }

    public void EquipItem(int index) {

        Item item = items[index];

        switch(item.itemObj.type) {
            case ItemType.Active:
                if(equipedActive != null) { AddItem(equipedActive.itemObj, equipedActive.quantity); }
                equipedActive = item;
                break;
            
            case ItemType.Passive:
                if(equipedPassive != null) { AddItem(equipedPassive.itemObj, equipedPassive.quantity); }
                equipedPassive = item;
                break;
        }

        RemoveItem(item);

    }

    public void AddItem(ItemObject item, int quantity = -1) {

        Item newItem = new Item(item);

        int itemIndex = items.FindIndex(obj => obj == newItem);

        if(itemIndex != -1)
            if(item.stackable)
            {
                if(quantity != -1)
                    items[itemIndex].quantity = quantity;
                else
                    items[itemIndex].quantity += 1;
            }
        else
            items.Add(newItem);

    }

    public void RemoveItem(ItemObject item) {

        Item itemToRemove = new Item(item);
        RemoveItem(itemToRemove);

    }

    public void RemoveItem(Item item) {

        items.RemoveAll(obj => obj == item);

    }

}
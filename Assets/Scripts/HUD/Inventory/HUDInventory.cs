using UnityEngine;
using Undefined.Items;
using System.Collections.Generic;

public class HUDInventory : MonoBehaviour
{
        #region Variables

    [Header("Selected Item Info")]
    [SerializeField] private HUDItemDetails itemsDetails = null;

    [Header("Equiped Items")]
    [SerializeField] private HUDEquipedInfo activeItemInfo = null;
    [SerializeField] private HUDEquipedInfo passiveItemInfo = null;
    [SerializeField] private HUDEquipedInfo weaponItemInfo = null;

    [Header("Backpack")]
    [SerializeField] private Transform itemsTransform = null;
    [SerializeField] private GameObject itemPrefab = null;

    // Script side variables
    private int selectedItemIndex;
    //private bool selectedEquipItem;

    // Script side
    private GameObject player;
    private PlayerInventory inventory;
    private List<InventoryItem> items { get { return inventory.Items; } }
    private InventoryItem activeItem { get { return inventory.ActiveItem; }}
    private InventoryItem passiveItem { get { return inventory.PassiveItem; }}
    private InventoryItem weaponItem { get { return inventory.WeaponItem; } }

        #endregion

    void Awake() {

        // Gets the player and its inventory
        player = GameObject.FindWithTag("Player");
        inventory = player.GetComponent<PlayerInventory>();

        // Setup the player events
        inventory.onUse += cb => RefreshBackpack();

    }

    void Start() {

        // Sets initial value to the selected index
        selectedItemIndex = -1;
        //selectedEquipItem = false;

        // Configure the buttons
        activeItemInfo.SelectButton.onClick.AddListener(() => {ShowSelectedEquipedItem(ItemType.Active);});
        passiveItemInfo.SelectButton.onClick.AddListener(() => {ShowSelectedEquipedItem(ItemType.Passive);});
        weaponItemInfo.SelectButton.onClick.AddListener(() => {ShowSelectedEquipedItem(ItemType.Weapon);});

        // Refresh the backpack content
        RefreshBackpack();

    }

    public void RefreshBackpack() {

        // Resets the selected item
        selectedItemIndex = -1;
        itemsDetails.ShowItemInfo();

        // Shows the equiped items
        activeItemInfo.SetNewEquip(activeItem.itemObj, activeItem.quantity);
        passiveItemInfo.SetNewEquip(passiveItem.itemObj, passiveItem.quantity);
        weaponItemInfo.SetNewEquip(weaponItem.itemObj, weaponItem.quantity);

        // Deletes all item prefabs inside the backpack panel
        for(int i = 0; i < itemsTransform.childCount; i++) {

            Destroy(itemsTransform.GetChild(i).gameObject);

        }

        // Create all items prefabs inside the backpack panel
        for(int i = 0; i < items.Count; i++) {

            GameObject itemObj = Instantiate(itemPrefab, itemsTransform);
            itemObj.GetComponent<HUDItemInfo>().Setup(items[i], i, this);

        }

    }

        #region Public methods

    ///<summary>Shows information about a specific item</summary>
    ///<param name="itemToShow">The item object that contains the information about the item</param>
    ///<param name="positionInventory">The position (index) where the item is in the inventory</param>
    public void ShowSelectedItem(ItemObject itemToShow, int positionInventory) {

        itemsDetails.ShowItemInfo(itemToShow);

        selectedItemIndex = positionInventory;
        //selectedEquipItem = true;

    }

    public void ShowSelectedEquipedItem(ItemType type) {

        ItemObject item = null;

        switch(type) {
            case ItemType.Weapon:
                item = weaponItem.itemObj;
                selectedItemIndex = -4;
                break;

            case ItemType.Passive:
                item = passiveItem.itemObj;
                selectedItemIndex = -3;
                break;
            
            case ItemType.Active:
                item = activeItem.itemObj;
                selectedItemIndex = -2;
                break;
        }   

        if(item == null) return;

        itemsDetails.ShowItemInfo(item, true);
        //selectedEquipItem = true;

    }

    public void EquipUnequipItem() {
        
        switch(selectedItemIndex) {
            case -1:
                return;
            
            case -2:
                inventory.UnequipItem(ItemType.Active);
                break;
            
            case -3:
                inventory.UnequipItem(ItemType.Passive);
                break;
            
            case -4:
                inventory.UnequipItem(ItemType.Weapon);
                break;

            default:
                inventory.EquipItem(selectedItemIndex);
                break;
        }

        RefreshBackpack();

    }

        #endregion

}

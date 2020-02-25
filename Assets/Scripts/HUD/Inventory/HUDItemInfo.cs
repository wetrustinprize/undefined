using UnityEngine;
using UnityEngine.UI;
using Undefined.Items;

public class HUDItemInfo : MonoBehaviour
{
        #region Variables

    [Header("UI")]
    [SerializeField] private Image itemImage;

    [Header("Info")]
    public int inventoryIndex;
    public ItemObject myItem;

    // Script side
    HUDInventory invManager;


        #endregion

    void Start() {

        GetComponent<Button>().onClick.AddListener(ShowMyItemDetails);

    }

    void ShowMyItemDetails() {

        invManager.ShowSelectedItem(myItem, inventoryIndex);

    }

    public void Setup(ItemObject item, int index, HUDInventory invetoryManager) {

        inventoryIndex = index;
        myItem = item;
        invManager = invetoryManager;

        itemImage.sprite = myItem.icon;

    }

}

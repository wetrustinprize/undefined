using UnityEngine;
using UnityEngine.UI;
using Undefined.Items;

public class HUDItemDetails : MonoBehaviour {

        #region Variables

    [Header("HUD Inventory")]
    [SerializeField] private HUDInventory inventory_Manager;

    [Header("UI Components")]
    [SerializeField] private Image itemIcon;
    [SerializeField] private Text itemName;
    [SerializeField] private Text itemDescription;
    [SerializeField] private Text itemStats;

    // Script side
    private ItemObject selectedItem;
    private int selectedIndex;

    private CanvasGroup group;

        #endregion


    void Start() {

        // Gets the canvas groups
        group = GetComponent<CanvasGroup>();

        // Sets to invisible
        group.interactable = false;
        group.alpha = 0f;

    }

    public void Setup(ItemObject item = null, int index = 0) {

        selectedItem = item;
        selectedIndex = index;

        if(selectedItem == null)
        {
            group.interactable = false;
            group.alpha = 0f;
            return;
        }
        else
        {
            itemIcon.sprite = selectedItem.icon;
            itemName.text = selectedItem.itemName.ToUpper();
            itemDescription.text = $"<i>{selectedItem.description}</i>";
            itemStats.text = $"<b>Stats:</b>\n{selectedItem.statsDescription}";

            group.interactable = true;
            group.alpha = 1f;
        }

    }


}
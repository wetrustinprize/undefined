using UnityEngine;
using UnityEngine.UI;
using Undefined.Items;

public class HUDItemDetails : MonoBehaviour {

        #region Variables

    [Header("UI Components")]
    [SerializeField] private Image itemIcon;
    [SerializeField] private Text itemName;
    [SerializeField] private Text itemDescription;
    [SerializeField] private Text itemStats;
    [SerializeField] private Text buttonText;

    // Script side
    private CanvasGroup group;

        #endregion


    void Start() {

        // Gets the canvas groups
        group = GetComponent<CanvasGroup>();

        // Sets to invisible
        group.interactable = false;
        group.alpha = 0f;

    }

    public void ShowItemInfo(ItemObject item = null, bool equipedItem = false) {

        if(item == null)
        {
            group.interactable = false;
            group.alpha = 0f;
            return;
        }
        else
        {
            itemIcon.sprite = item.icon;
            itemName.text = item.itemName.ToUpper();
            itemDescription.text = $"<i>{item.description}</i>";
            itemStats.text = $"<b>Stats:</b>\n{item.statsDescription}";
            buttonText.text = equipedItem ? "Unequip" : "Equip";

            group.interactable = true;
            group.alpha = 1f;
        }

    }


}
using UnityEngine;
using UnityEngine.UI;
using Undefined.Items;

public class HUDItemDetails : MonoBehaviour {

        #region Variables

    [Header("UI Components")]
    [SerializeField] private Image itemIcon = null;
    [SerializeField] private Text itemName = null;
    [SerializeField] private Text itemDescription = null;
    [SerializeField] private Text itemStats = null;
    [SerializeField] private Text buttonText = null;

    // Script side
    private CanvasGroup group;

        #endregion


    void Awake() {

        // Gets the canvas groups
        group = GetComponent<CanvasGroup>();

    }

    void Start() {

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
using UnityEngine;
using UnityEngine.UI;
using Undefined.Items;

using TMPro;

public class HUDItemDetails : MonoBehaviour {

        #region Variables

    [Header("UI Components")]
    [SerializeField] private Image itemIcon = null;
    [SerializeField] private TextMeshProUGUI itemName = null;
    [SerializeField] private TextMeshProUGUI itemDescription = null;
    [SerializeField] private TextMeshProUGUI itemStats = null;
    [SerializeField] private TextMeshProUGUI buttonText = null;

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
            itemName.text = item.itemName;
            itemDescription.text = $"<i>{item.description}</i>";
            itemStats.text = $"<b>Stats:</b>\n{item.statsDescription}";
            buttonText.text = equipedItem ? "Unequip" : "Equip";

            group.interactable = true;
            group.alpha = 1f;
        }

    }


}
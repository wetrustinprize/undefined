using UnityEngine;
using UnityEngine.UI;
using Undefined.Items;

public class HUDShopItemInfo : MonoBehaviour {

        #region Variables

    [Header("UI")]
    [SerializeField] private Image itemImage;

    [Header("Info")]
    public int shopListIndex;

    // Script side
    HUDShop shopManager;

        #endregion

    void Start() {

        GetComponent<Button>().onClick.AddListener(ShowMyItemDetails);

    }

    void ShowMyItemDetails() {

        shopManager.ShowSelectedItem(shopListIndex);

    }

    public void Setup(ItemObject item, int index, HUDShop shopManager) {

        this.shopListIndex = index;
        this.shopManager = shopManager;

        this.itemImage.sprite = item.icon;

    }

}
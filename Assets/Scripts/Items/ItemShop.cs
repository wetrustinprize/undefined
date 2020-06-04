using UnityEngine;

namespace Undefined.Items.Shop {

    public enum ShopCoin {
        Gold,
        Secret
    }

    [System.Serializable]
    public class ItemShop {

            #region Variables

        [Header("Information")]
        public int value;
        public ShopCoin coinType;

        [Multiline]
        public string[] sellerDescriptions;
        public ItemObject sellingItem;

            #endregion

    }

}

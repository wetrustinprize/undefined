using UnityEngine;

namespace Undefined.Items.Shop {

    [System.Serializable]
    public class ItemShop {

            #region Variables

        [Header("Information")]
        public int value;

        [Multiline]
        public string[] sellerDescriptions;
        public ItemObject sellingItem;

            #endregion

    }

}

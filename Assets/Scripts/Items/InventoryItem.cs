using UnityEngine;

namespace Undefined.Items {

    [System.Serializable]
    public class InventoryItem {

        public int quantity;
        public ItemObject itemObj;

        public InventoryItem(ItemObject item) {
            quantity = item.stackable ? 1 : -1;
            itemObj = item;

        }

        public InventoryItem(ItemObject item, int quantity) {
            this.quantity = quantity;
            this.itemObj = item;
        }

        public InventoryItem() {
            quantity = 0;
            itemObj = null;
        }

    }

}

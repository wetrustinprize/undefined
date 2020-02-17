using UnityEngine;

namespace Undefined.Items {

    public enum ItemType {
        Passive,
        Active,
    }

    [System.Serializable]
    public class Item {

        public int quantity;
        public ItemObject itemObj;

        public Item(ItemObject item) {
            quantity = item.stackable ? 1 : -1;
            itemObj = item;

        }

    }

    [CreateAssetMenu(fileName = "New Item", menuName = "Undefined/Item")]
    public class ItemObject : ScriptableObject {

            #region Variables

        [Header("Information")]
        public string itemName;

        [Multiline]
        public string description;

        [Multiline]
        public string statsDescription;
        
        [Space(10)]
        public Sprite icon;

        [Space(10)]
        public ItemType type;
        public bool stackable;

        [Header("Behaviour")]
        public ItemBehaviour behaviour;

            #endregion

    }

    [CreateAssetMenu(fileName = "New Shop Item", menuName = "Undefined/Shop Item")]
    public class ShopItemObject : ScriptableObject {

            #region Variables

        [Header("Information")]
        public int value;
        public string[] sellerDescriptions;
        public Item sellItem;

            #endregion

    }

}

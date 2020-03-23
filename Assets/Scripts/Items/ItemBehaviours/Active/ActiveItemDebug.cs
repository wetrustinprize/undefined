using UnityEngine;

namespace Undefined.Items {

    [CreateAssetMenu(
        fileName = "New Active Debug Item",
        menuName = "Undefined/Item/Active/Debug Item"
    )]
    public class ActiveItemDebug : ItemObject {

            #region Variables

        [Header("Debug")]
        public string equipMessage;
        public string unequipMessage;
        public string buyMessage;
        public string useMessage;

        [Header("Item Info")]
        public bool isStackable;

            #endregion

        override public ItemType type { get { return ItemType.Active; } }
        override public bool stackable { get { return isStackable; } }

        override public void OnEquip(GameObject player) {
            Debug.Log(equipMessage);
        }

        override public void OnUnequip(GameObject player) {
            Debug.Log(unequipMessage);
        }

        override public void OnBuy(GameObject player) {
            Debug.Log(buyMessage);
        }

        override public void OnUse(GameObject player) {
            Debug.Log(useMessage);
        }

    }

}
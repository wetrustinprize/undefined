using UnityEngine;

namespace Undefined.Items {

    [CreateAssetMenu(
        fileName = "New Passive Debug Item",
        menuName = "Undefined/Item/Passive/Debug Item"
    )]
    public class PassiveItemDebug : ItemObject {

            #region Variables

        [Header("Debug")]
        public string equipMessage;
        public string unequipMessage;
        public string buyMessage;

            #endregion

        override public ItemType type { get { return ItemType.Passive; } }
        override public bool stackable { get { return false; } }

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
            return;
        }

    }

}
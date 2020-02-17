using UnityEngine;

namespace Undefined.Items {

    public abstract class ItemBehaviour {

        public abstract void OnUse(GameObject player);
        public abstract void OnBuy(GameObject player);
        public abstract void OnEquip(GameObject player);
        public abstract void OnUnequip(GameObject player);

    }

}
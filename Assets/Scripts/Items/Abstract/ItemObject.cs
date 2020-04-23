using UnityEngine;

namespace Undefined.Items {

    public abstract class ItemObject : ScriptableObject {

            #region Variables

        [Header("Information")]
        public string itemName;

        [Multiline]
        public string description;

        [Multiline]
        public string statsDescription;
        
        [Space(10)]
        public Sprite icon;

        public abstract bool stackable { get; }
        public abstract ItemType type { get; }

        [Header("Sounds")]
        public AudioClip onEquipSFX;
        public AudioClip onUnequipSFX;

        // Absctract classes

        public abstract void OnEquip(GameObject player);
        public abstract void OnUnequip(GameObject player);
        public abstract void OnBuy(GameObject player);
        public abstract void OnUse(GameObject player);

            #endregion

    }

}

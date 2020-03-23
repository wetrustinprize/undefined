using UnityEngine;

namespace Undefined.Items {

    [CreateAssetMenu(
        fileName = "New Passive Stats Item", 
        menuName = "Undefined/Item/Passive/Passive Stats Item"
    )]
    public class PassiveItemStats : ItemObject {

            #region Variables

        [Header("Stats")]
        public int agility;
        public int strength;
        public int toughness;

            #endregion

        override public ItemType type { get { return ItemType.Passive; } }
        override public bool stackable { get { return false; } }

        override public void OnEquip(GameObject player) {

            PlayerStats stats = player.GetComponent<PlayerStats>();

            stats.Agility += agility;
            stats.Strength += strength;
            stats.Toughness += toughness;

            

        }
        
        override public void OnUnequip(GameObject player) {

            PlayerStats stats = player.GetComponent<PlayerStats>();

            stats.Agility -= agility;
            stats.Strength -= strength;
            stats.Toughness -= toughness;

        }

        override public void OnBuy(GameObject player) { return; }

        override public void OnUse(GameObject player) { return; }

    }

}
using UnityEngine;

namespace Undefined.Items {

    [CreateAssetMenu(fileName = "New Passive Stats Item", menuName = "Undefined/Item/Passive/Passive Stats Item")]
    public class ItemStats : ItemObject {

        public int agility;
        public int strength;
        public int toughness;

        override public ItemType type { get { return ItemType.Passive; } }

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
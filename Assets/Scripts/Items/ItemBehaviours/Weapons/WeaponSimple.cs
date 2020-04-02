using UnityEngine;
using Undefined.Force;

namespace Undefined.Items {

    [CreateAssetMenu(
        fileName = "New Simple Weapon",
        menuName = "Undefined/Item/Weapon/Simple Weapon"
    )]
    public class WeaponSimple : ItemObject {

            #region Variables

        [Header("Attack Change")]
        public int attackDamage;
        public float attackCooldown;

        [Header("Push Force")]
        public float attackPushForce;
        public float attackPushTime;

        [Header("Slow")]
        public Slow attackerSlow;
        public float attackerSlowTimer;

        override public ItemType type { get { return ItemType.Weapon; } }
        override public bool stackable { get { return false; } }

            #endregion


        override public void OnEquip(GameObject player) {

            Attack attack = player.GetComponent<Attack>();

            attack.Damage = attackDamage;
            attack.coolDown = attackCooldown;
            
            attack.pushForce = attackPushForce;
            attack.pushTime = attackPushTime;

            attack.attackerSlow = attackerSlow;
            attack.attackerSlowTimer = attackerSlowTimer;

        }

        override public void OnUnequip(GameObject player) {

            Attack attack = player.GetComponent<Attack>();

            attack.SetDefaultValues();

        }

        override public void OnBuy(GameObject player) { return; }

        override public void OnUse(GameObject player) { return; }

    }

}
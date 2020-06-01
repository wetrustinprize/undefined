using UnityEngine;

namespace Undefined.Items {

    [CreateAssetMenu(
        fileName = "New Passive Stats Item", 
        menuName = "Undefined/Item/Passive/Passive Stats Item"
    )]
    public class PassiveItemStats : ItemObject {

            #region Variables

        [Header("Aditionals")]
        public Vector2 aditionalSpeed;
        public Vector2 aditionalJumpSpeed;
        public Vector2 aditionalWallJumpSpeed;
        
        [Space]
        public int aditionalHealth;

        [Range(0.0f, 1.0f)]
        public float aditionalDefense;

            #endregion

        override public ItemType type { get { return ItemType.Passive; } }
        override public bool stackable { get { return false; } }

        override public void OnEquip(GameObject player) {

            PlayerStats stats = player.GetComponent<PlayerStats>();

            stats.AditionalSpeed = aditionalSpeed;
            stats.AditionalJumpSpeed = aditionalJumpSpeed;
            stats.AditionalWallJumpSpeed = aditionalWallJumpSpeed;

            stats.AditionalHealth = aditionalHealth;
            stats.AditionalDefense = aditionalDefense;

            

        }
        
        override public void OnUnequip(GameObject player) {

            PlayerStats stats = player.GetComponent<PlayerStats>();

            stats.AditionalSpeed = Vector2.zero;
            stats.AditionalJumpSpeed = Vector2.zero;
            stats.AditionalWallJumpSpeed = Vector2.zero;
            
            stats.AditionalHealth = 0;
            stats.AditionalDefense = 0;

        }

        override public void OnBuy(GameObject player) { return; }

        override public void OnUse(GameObject player) { return; }

    }

}
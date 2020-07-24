using UnityEngine;

using Undefined.Items;
using Undefined.Items.Shop;

using System.Collections.Generic;

namespace Undefined.Checkpoints {
    [System.Serializable]
    public class Save {

            #region Variables

        [Header("Map Info")]
        public Vector2 PlayerPosition;

        [Header("Alive Info")]
        public int Health;

        [Header("Inventory Info")]
        public int PlayerGold;
        public int PlayerSecret;

        public InventoryItem ActiveItem;
        public InventoryItem PassiveItem;
        public InventoryItem WeaponItem;
        public List<InventoryItem> Items;

        [Header("Skills Info")]
        public bool CanDash;
        public bool CanTeleport;
        public bool CanExplode;
        public bool CanWalljump;

            #endregion

        public Save(Vector3 position, PlayerController controller, PlayerInventory inventory, Alive alive)
        {
            this.PlayerPosition = position;
            
            this.PlayerGold = inventory.Gold;
            this.PlayerSecret = inventory.SecretGold;
            this.ActiveItem = inventory.ActiveItem;
            this.PassiveItem = inventory.PassiveItem;
            this.WeaponItem = inventory.WeaponItem;
            this.Items = inventory.Items;

            this.CanDash = controller.canDash;
            this.CanTeleport = controller.canTeleport;
            this.CanExplode = controller.canExplode;
            this.CanWalljump = controller.canWallJump;

            this.Health = alive.Health;
        }

        public void Load() {
            GameObject player = GameManager.Player.gameObject;

            PlayerController controller = player.GetComponent<PlayerController>();
            PlayerInventory inventory = player.GetComponent<PlayerInventory>();
            Alive alive = player.GetComponent<Alive>();

            GameManager.Player.gameObject.transform.position = this.PlayerPosition;

            inventory.SetGold(this.PlayerGold, ShopCoin.Gold);
            inventory.SetGold(this.PlayerSecret, ShopCoin.Secret);

            if(ActiveItem.itemObj != null)
                inventory.EquipItem(this.ActiveItem);
            
            if(PassiveItem.itemObj != null)
                inventory.EquipItem(this.PassiveItem);
            
            if(WeaponItem.itemObj != null)
                inventory.EquipItem(this.WeaponItem);
            
            inventory.OverrideInventory(this.Items);

            controller.canDash = this.CanDash;
            controller.canTeleport = this.CanTeleport;
            controller.canExplode = this.CanExplode;
            controller.canWallJump = this.CanWalljump;

            alive.OverrideHealth(this.Health);

        }

    }
}
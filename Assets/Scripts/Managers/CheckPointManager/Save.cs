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

        public Save(Vector2 position)
        {
            PlayerController controller = GameManager.Player;

            this.PlayerPosition = position;
            
            this.PlayerGold = controller.inventory.Gold;
            this.PlayerSecret = controller.inventory.SecretGold;
            this.ActiveItem = controller.inventory.ActiveItem;
            this.PassiveItem = controller.inventory.PassiveItem;
            this.WeaponItem = controller.inventory.WeaponItem;
            this.Items = controller.inventory.Items;

            this.CanDash = controller.canDash;
            this.CanTeleport = controller.canTeleport;
            this.CanExplode = controller.canExplode;
            this.CanWalljump = controller.canWallJump;

            this.Health = controller.alive.Health;
        }

        public void Load() {

            PlayerController controller = GameManager.Player;
            PlayerInventory inventory = controller.inventory;
            Alive alive = controller.alive;

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
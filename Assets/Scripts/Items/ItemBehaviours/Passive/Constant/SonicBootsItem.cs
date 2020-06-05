using UnityEngine;

namespace Undefined.Items {

    [CreateAssetMenu(
        fileName = "New Sonic Boot Item",
        menuName = "Undefined/Item/Passive/Constant/Sonic Boot"
    )]
    public class SonicBootsItem : ItemObject {

            #region Variables

        [Header("Rainbow")]
        public GameObject rainbowEffect = null;

        [Header("Aditional")]
        public Vector2 aditionalWallJumpSpeed;

        //script side
        public static GameObject curRainbow = null;

            #endregion

        override public ItemType type { get { return ItemType.Passive; } }
        override public bool stackable { get { return false; } }

        override public void OnEquip(GameObject player)
        {
            PlayerStats stats = player.GetComponent<PlayerStats>();

            stats.AditionalWallJumpSpeed = aditionalWallJumpSpeed;

            if(curRainbow == null)
            {
                curRainbow = Instantiate(rainbowEffect, player.transform);
                curRainbow.GetComponent<PlayerRainbow>().Setup(player);
            }

        }

        override public void OnUnequip(GameObject player)
        {
            PlayerStats stats = player.GetComponent<PlayerStats>();

            stats.AditionalWallJumpSpeed = Vector2.zero;

            if(curRainbow != null)
                Destroy(curRainbow);

        }

        override public void OnBuy(GameObject player) { return; }

        override public void OnUse(GameObject player) { return; }

    }

}
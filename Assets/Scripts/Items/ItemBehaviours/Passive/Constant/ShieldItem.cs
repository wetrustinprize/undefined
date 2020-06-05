using UnityEngine;

namespace Undefined.Items {

    [CreateAssetMenu(
        fileName = "New Shield Item",
        menuName = "Undefined/Item/Passive/Constant/Shield"
    )]
    public class ShieldItem : ItemObject {

            #region Variables

        [Header("Shield")]
        public GameObject shieldEffect = null;

        [Header("Aditional")]
        [Range(0.0f, 1.0f)]
        public float aditionalDefense = 0;

        //script side
        public static GameObject curDefense = null;

            #endregion

        override public ItemType type { get { return ItemType.Passive; } }
        override public bool stackable { get { return false; } }

        override public void OnEquip(GameObject player)
        {

            PlayerStats stats = player.GetComponent<PlayerStats>();

            stats.AditionalDefense = aditionalDefense;

            if(curDefense == null)
            {
                curDefense = Instantiate(shieldEffect, player.transform);
                curDefense.GetComponent<PlayerShield>().Setup(player);
            }

        }

        override public void OnUnequip(GameObject player)
        {

            PlayerStats stats = player.GetComponent<PlayerStats>();

            stats.AditionalDefense = 0.0f;

            if(curDefense != null)
                Destroy(curDefense);

        }

        override public void OnBuy(GameObject player) { return; }
        override public void OnUse(GameObject player) { return; }

    }

}
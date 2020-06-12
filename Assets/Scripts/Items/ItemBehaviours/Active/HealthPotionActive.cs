using UnityEngine;


namespace Undefined.Items {

    [CreateAssetMenu(
        fileName = "New Health Potion",
        menuName = "Undefined/Item/Active/Health Potion"
    )]
    public class HealthPotionActive : ItemObject {

            #region Variables

    [Header("Heal Value")]
    public int healValue = 0;

    [Header("Effect")]
    public GameObject effect;

            #endregion

    override public ItemType type { get { return ItemType.Active; } }
    override public bool stackable { get { return true; } }

    override public void OnEquip(GameObject player) { return; }
    override public void OnUnequip(GameObject player) { return; }
    override public void OnBuy(GameObject player) { return; }
    override public void OnUse(GameObject player)
    {
        Alive alive = player.GetComponent<Alive>();

        alive.Heal(healValue, player);

        Instantiate(effect, player.transform);
    }

    }

}
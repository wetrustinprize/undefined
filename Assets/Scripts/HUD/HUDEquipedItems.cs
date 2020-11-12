using UnityEngine;
using UnityEngine.UI;

public class HUDEquipedItems : MonoBehaviour
{
    
                #region Variables

        [Header("Images")]
        [SerializeField] private Image passiveIcon = null;
        [SerializeField] private Text passiveQuantity = null;

        [Space(10)]
        [SerializeField] private Image activeIcon = null;

        // Script side
        PlayerInventory myInventory = null;

                #endregion

        void Start() {

                myInventory = GameManager.Player.inventory;

                myInventory.onEquip += cb => { Refresh(); };
                myInventory.onUnequip += cb => { Refresh(); };
                myInventory.onUse += cb => { Refresh(); };

                Refresh();

        }

        void Refresh() {

                passiveIcon.sprite = myInventory.PassiveItem.itemObj != null ? myInventory.PassiveItem.itemObj.icon : null;
                passiveIcon.color = new Color(1,1,1, myInventory.PassiveItem.itemObj != null ? 1 : 0);
                   
                activeIcon.sprite = myInventory.ActiveItem.itemObj != null ? myInventory.ActiveItem.itemObj.icon : null;
                activeIcon.color = new Color(1,1,1, myInventory.ActiveItem.itemObj != null ? 1 : 0);
                passiveQuantity.text = myInventory.ActiveItem.itemObj != null ? $"x{myInventory.ActiveItem.quantity}" : "";

        }

}

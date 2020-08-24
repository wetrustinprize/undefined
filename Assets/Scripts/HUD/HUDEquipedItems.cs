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

                if(myInventory.PassiveItem.itemObj != null)
                        passiveIcon.sprite = myInventory.PassiveItem.itemObj.icon;
                else
                        passiveIcon.sprite = null;


                if(myInventory.ActiveItem.itemObj != null)
                {                        
                        activeIcon.sprite = myInventory.ActiveItem.itemObj.icon;
                        passiveQuantity.text = $"x{myInventory.ActiveItem.quantity}";
                }
                else
                {
                        activeIcon.sprite = null;
                        passiveQuantity.text = $"";
                }

        }

}

using UnityEngine;
using UnityEngine.UI;
using Undefined.Items;

using TMPro;

public class HUDEquipedInfo : MonoBehaviour {

        #region Variables

    [Header("Informations")]
    [SerializeField] private Image equipedSprite = null;
    [SerializeField] private TextMeshProUGUI ammount = null;

    [Header("Button")]
    [SerializeField] private Button selectButton = null;

    // public access
    public Button SelectButton { get { return selectButton; } }

        #endregion

    void Start() {

        equipedSprite.color = new Color(1,1,1, 0);
        equipedSprite.sprite = null;
        selectButton.interactable = false;

        if(ammount == null) return;

        ammount.text = "";

    }

    public void SetNewEquip(ItemObject itemToEquip = null, int quantity = 0) {

        equipedSprite.color = new Color(1,1,1, itemToEquip == null ? 0 : 1);

        equipedSprite.sprite = itemToEquip != null ? itemToEquip.icon : null;
        selectButton.interactable = itemToEquip != null;

        if(ammount == null) return;

        ammount.text = itemToEquip != null ? $"x{quantity}" : "";

    }

}
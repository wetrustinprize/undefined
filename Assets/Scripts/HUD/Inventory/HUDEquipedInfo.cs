using UnityEngine;
using UnityEngine.UI;
using Undefined.Items;

public class HUDEquipedInfo : MonoBehaviour {

        #region Variables

    [Header("Informations")]
    [SerializeField] private Image equipedSprite;
    [SerializeField] private Text ammount;

    [Header("Button")]
    [SerializeField] private Button selectButton;

    // public access
    public Button SelectButton { get { return selectButton; } }

        #endregion

    void Start() {

        equipedSprite.sprite = null;
        selectButton.interactable = false;

        if(ammount == null) return;

        ammount.text = "";

    }

    public void SetNewEquip(ItemObject itemToEquip = null, int quantity = 0) {

        equipedSprite.sprite = itemToEquip != null ? itemToEquip.icon : null;
        selectButton.interactable = itemToEquip != null;

        if(ammount == null) return;

        ammount.text = itemToEquip != null ? $"x{quantity}" : "";

    }

}
using UnityEngine;
using UnityEngine.UI;
using Undefined.Items;

public class HUDItemInfo : MonoBehaviour
{
        #region Variables

    [Header("UI")]
    [SerializeField] private Image itemImage;

    [Header("Info")]
    public int inventoryIndex;
    public ItemObject myItem;

    // Script side
    HUDItemDetails itemDetailsManager;


        #endregion

    void Start() {

        GetComponent<Button>().onClick.AddListener(ShowMyItemDetails);

    }

    void ShowMyItemDetails() {

        itemDetailsManager.Setup(myItem, inventoryIndex);

    }

    public void Setup(ItemObject item, int index, HUDItemDetails hudItemDetails) {

        inventoryIndex = index;
        myItem = item;
        itemDetailsManager = hudItemDetails;

        itemImage.sprite = myItem.icon;

    }

}

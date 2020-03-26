using UnityEngine;

namespace Undefined.Items.Shop {

    [CreateAssetMenu(
        fileName = "New Shop Info",
        menuName = "Undefined/Shop/Shop Info"
    )]
    public class ShopDetails : ScriptableObject {

        [Header("Shop Info")]
        public string shopName;

        [Header("Shop Messages")]

        [Multiline]
        public string[] welcomeMessage;
        [Multiline]
        public string[] noItemsMessage;

        [Header("Selling items")]
        
        public ItemShop[] sellingItems;

    }

}
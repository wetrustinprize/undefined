using UnityEngine;

namespace Undefined.Items.Shop {

    [CreateAssetMenu(
        fileName = "New Shop Info",
        menuName = "Undefined/Shop/Shop Info"
    )]
    public class ShopDetails : ScriptableObject {

        [Header("Information")]
        public ItemShop[] sellingItems;

    }

}
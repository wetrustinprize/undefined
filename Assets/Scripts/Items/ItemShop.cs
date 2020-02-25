using UnityEngine;

namespace Undefined.Items {

    [CreateAssetMenu(fileName = "New Shop Item", menuName = "Undefined/Item/Shop Item")]
    public class ItemShop : ScriptableObject {

            #region Variables

        [Header("Information")]
        public int value;
        public string[] sellerDescriptions;
        public ItemObject sellItem;

            #endregion

    }

}

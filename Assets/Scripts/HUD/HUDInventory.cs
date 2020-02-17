using UnityEngine;
using UnityEngine.UI;
using Undefined.Items;
using System.Collections.Generic;

public class HUDInventory : MonoBehaviour
{
        #region Variables

    [Header("Selected Item Info")]
    [SerializeField] private HUDItemDetails itemsDetails;

    [Header("Items")]
    [SerializeField] private Transform itemsTransform;
    [SerializeField] private GameObject itemPrefab;

    // Script side
    private GameObject player;
    private List<Item> items { get { return player.GetComponent<PlayerInventory>().Items; } }

        #endregion

    void Start() {

        player = GameObject.FindWithTag("Player");

        for(int i = 0; i < items.Count; i++) {

            GameObject itemObj = Instantiate(itemPrefab, itemsTransform);
            itemObj.GetComponent<HUDItemInfo>().Setup(items[i].itemObj, i, itemsDetails);

        }

    }

}

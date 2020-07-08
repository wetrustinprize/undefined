using UnityEngine;
using Undefined.Items.Shop;

public class CoinBehaviour : MonoBehaviour
{
    
        #region Variables

    [Header("Sounds")]
    [SerializeField] private AudioClip pickupSound = null;

    [Header("Animator")]
    [SerializeField] private Animator animator = null;

    [Header("Information")]
    public ShopCoin coinType;
    public int value;

    // Script side
    private bool picked = false;

        #endregion

    void OnTriggerEnter2D(Collider2D col) {

        if(col.tag == "Player" && !picked)
        {
            picked = true;
            animator.SetTrigger("Pick");
            col.GetComponent<PlayerInventory>().AddGold(value, coinType);
            GameManager.Sound.PlayUISFX(pickupSound, 1);
            Destroy(this.gameObject, 1);
        }

    }

}

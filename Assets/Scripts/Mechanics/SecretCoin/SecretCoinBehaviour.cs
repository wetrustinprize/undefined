using UnityEngine;
using Undefined.Items.Shop;

public class SecretCoinBehaviour : MonoBehaviour
{
    
        #region Variables

    [Header("Sounds")]
    [SerializeField] private AudioClip pickupSound = null;

    [Header("Animator")]
    [SerializeField] private Animator animator = null;

        #endregion

    void OnTriggerEnter2D(Collider2D col) {

        if(col.tag == "Player")
        {
            animator.SetTrigger("Pick");
            col.GetComponent<PlayerInventory>().AddGold(1, ShopCoin.Secret);
            GameManager.Sound.PlayUISFX(pickupSound, 1);
            Destroy(this.gameObject, 1);
        }

    }

}

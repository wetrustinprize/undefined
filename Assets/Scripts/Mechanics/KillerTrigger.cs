using UnityEngine;

public class KillerTrigger : MonoBehaviour
{

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            GameManager.Player.alive.TakeDamage(99999, this.gameObject);
        }
    }

}

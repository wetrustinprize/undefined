using UnityEngine;

public class PlayerShield : MonoBehaviour
{
    
        #region Variables

    [Header("Animation")]
    [SerializeField] private Animator animator = null;

    // Script side
    private GameObject myPlayer = null;

        #endregion
    
    public void Setup(GameObject player) {

        myPlayer = player;

        Alive alive = player.GetComponent<Alive>();

        alive.onDamage += Damaged;
    }

    void Damaged(int damage, GameObject dealer) {

        Vector2 size = dealer.GetComponent<BoxCollider2D>().size / 2;
        Vector2 offset = dealer.GetComponent<BoxCollider2D>().offset;

        Vector2 dir = ((Vector2)dealer.transform.position - (Vector2)this.transform.position).normalized;

        Debug.Log($"Player Pos: {myPlayer.transform.position}\nAtacker Pos: {dealer.transform.position}\nDirection: {dir}");

        float rot_z = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        this.transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);

        animator.SetTrigger("Blink");

    }

    void OnDestroy() {

        Alive alive = myPlayer.GetComponent<Alive>();

        alive.onDamage -= Damaged;

    }

}

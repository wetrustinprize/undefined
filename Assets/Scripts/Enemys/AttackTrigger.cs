using UnityEngine;

public class AttackTrigger : MonoBehaviour {

        #region Variables

    [SerializeField] private Attack attack;

        #endregion

    void OnTriggerEnter2D(Collider2D collision) {

        if(collision.tag == "Player") {
            if(collision.TryGetComponent<Alive>(out Alive a)) {
                attack.DirectAttack(a, collision.transform);
            }
        }

    }

}
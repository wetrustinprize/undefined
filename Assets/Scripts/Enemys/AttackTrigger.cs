using UnityEngine;

public class AttackTrigger : MonoBehaviour {

        #region Variables

    [SerializeField] private Attack attack;
    [SerializeField] private bool ignorePlayer;

        #endregion

    void Start() {

        if(attack == null)
        {
            if(TryGetComponent<Attack>(out Attack a)) { attack = a; }
            else { Destroy(this); }
        }

    }

    void OnTriggerEnter2D(Collider2D collision) {

        if(collision.tag == "Player" && ignorePlayer) return;

        if(collision.TryGetComponent<Alive>(out Alive a)) {
            attack.DirectAttack(a, collision.transform);
        }

    }

}
using UnityEngine;

[RequireComponent(typeof(Attack))]
public class ExplosionBehaviour : MonoBehaviour
{
    
        #region Variable

    [Header("Explosion Settings")]
    [SerializeField] private float explosionRadius = 6;
    [SerializeField] private LayerMask raycastLayers = 0;
    [SerializeField] private bool ignorePlayer = false;

    // Local variables
    private Attack attack;

        #endregion

    void Start() {

        // Gets the attack component
        attack = GetComponent<Attack>();

        // Pos
        Vector3 pos = transform.position;
        pos.z = 0;

        // Explodes
        foreach(Collider2D col in Physics2D.OverlapCircleAll(transform.position, explosionRadius, raycastLayers))
        {
            Debug.Log(col.gameObject.name);
            if(col.tag == "Player" && ignorePlayer) return;

            attack.DirectAttack(col.gameObject);
        }

    }

}

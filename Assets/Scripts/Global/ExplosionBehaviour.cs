using UnityEngine;

[RequireComponent(typeof(Attack))]
public class ExplosionBehaviour : MonoBehaviour
{
    
        #region Variable

    [Header("Explosion Settings")]
    [SerializeField] private float explosionRadius = 6;
    [SerializeField] private LayerMask raycastLayers = 0;

    // Local variables
    private Attack attack;

        #endregion

    void Start() {

        // Gets the attack component
        attack = GetComponent<Attack>();

        // Explodes
        foreach(Collider2D col in Physics2D.OverlapCircleAll(transform.position, explosionRadius, raycastLayers))
        {
            if(col.tag != "Player")
            {
                attack.DirectAttack(col.gameObject);
            }
        }

    }

}

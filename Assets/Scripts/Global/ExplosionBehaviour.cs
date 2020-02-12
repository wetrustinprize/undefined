using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Attack))]
public class ExplosionBehaviour : MonoBehaviour
{
    
        #region Variable

    [Header("Explosion Settings")]
    [SerializeField] private float explosion_Time;
    [SerializeField] private float explosion_MaxSize;
    [SerializeField] private bool destroy_Gameobject;

    // Local variables
    private float explosionCurTime;
    private Attack attack;

        #endregion

    void Start() {

        // Gets the attack component
        attack = GetComponent<Attack>();

        transform.localScale = Vector3.zero;

    }

    void FixedUpdate() {

        explosionCurTime += Time.fixedDeltaTime;
        float percentage = explosionCurTime / explosion_Time;
        transform.localScale = new Vector3(explosion_MaxSize * percentage, explosion_MaxSize * percentage, 1);

        if(explosionCurTime > explosion_Time)
        {
            if(destroy_Gameobject)
                Destroy(this.gameObject);
            else
                Destroy(this);
        }

    }

}

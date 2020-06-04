using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class ExplosiveMineBehaviour : MonoBehaviour
{
    
        #region Variables

    [Header("Explosion")]
    [SerializeField] private GameObject explosion = null;

    [Header("Animation")]
    [SerializeField] private Animator anim = null;

    // Script-side variables
    private GameObject player;
    private bool insideRadius;
    private float radius;

        #endregion

    void Start() {

        player = GameManager.Player.gameObject;

        radius = GetComponent<CircleCollider2D>().radius;

    }

    void Update() {

        if(!insideRadius) return;

        float distance = Vector2.Distance(this.transform.position, player.transform.position);

        if(distance < 1.5)
        {
            Instantiate(explosion, transform.position, transform.rotation);
            Destroy(this.gameObject);
        }
        else
        {
            float velocityAnimation = 10 - distance;

            anim.SetFloat("BeepSpeed", velocityAnimation);
        }


    }

    void OnTriggerEnter2D(Collider2D col) {

        if(col.gameObject == player)
        {
            anim.SetBool("Show", true);
            insideRadius = true;
        }

    }

    void OnTriggerExit2D(Collider2D col) {

        if(col.gameObject == player)
        {
            anim.SetBool("Show", false);
            insideRadius = false;
        }

    }

}

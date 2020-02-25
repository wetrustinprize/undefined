using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureAnimator : MonoBehaviour
{

        #region Variables

    [SerializeField] private CreatureController creatureController;

    // Script side
    private SpriteRenderer spriteRenderer;
    private Animator animator;

        #endregion



    void Start() {

        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();


    }

    void Update()
    {
        
        spriteRenderer.flipX = creatureController.LastFacingDir != 1;
        animator.SetBool("Flying", !creatureController.IsNearToThePlayer);

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySimpleAnimation : MonoBehaviour
{

        #region Variables

    [Header("Animation Settings")]
    [SerializeField] private bool ignoreFlipX;
    [SerializeField] private bool ignoreFlipY;

    [Header("Animator Reference")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer flipSprite;

    // Script side
    private Motor motor;

        #endregion

    void Start() {

        motor = GetComponent<Motor>();

    }


    void Update()
    {
        // Get values
        Vector2 inputVel = motor.GetForce("input").ActualForce;

        float inputHVel = inputVel.x / motor.MaxSpeed.x;
        float inputVVel = inputVel.y / motor.MaxSpeed.y;

        float inputHDir = motor.InputAxis.x;
        float inputVDir = motor.InputAxis.y;

        float hVel = motor.finalSpeed.x;
        float vVel = motor.finalSpeed.y;

        bool onAir = !motor.OnGround;
        bool onWall = motor.OnWall;

        // Check flip
        if(!ignoreFlipX)
        {
            if(inputHDir > 0) flipSprite.flipX = false;
            else if(inputHDir < 0) flipSprite.flipX = true;
        }

        if(!ignoreFlipY)
        {
            if(inputVDir > 0) flipSprite.flipY = true;
            else if(inputVDir < 0) flipSprite.flipY = false;
        }

        // Apply animator values
        animator.SetFloat("inputHVel", inputHVel);
        animator.SetFloat("inputVVel", inputVVel);
        animator.SetFloat("inputHDir", inputHDir);
        animator.SetFloat("inputVDir", inputVDir);

        animator.SetFloat("hVel", hVel);
        animator.SetFloat("vVel", vVel);

        animator.SetBool("onAir", onAir);
        animator.SetBool("onWall", onWall);
    }
}

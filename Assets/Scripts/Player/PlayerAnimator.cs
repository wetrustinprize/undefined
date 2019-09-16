﻿using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private SpriteRenderer sprite;

    private PlayerMotor motor {get {return GetComponent<PlayerMotor>();}}

    void Update() {
        float vInput = motor.verticalInput;
        float vVel = motor.verticalSpeed;

        if(vInput != 0) sprite.flipX = vInput < 0;
        animator.SetFloat("Speed", vInput * (vInput < 0 ? -1 : 1));
        animator.SetFloat("Walk Anim Speed", (vVel * (vVel < 0 ? -1 : 1)) / 6);
    }

}

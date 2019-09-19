using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    
        #region Variables

    [SerializeField]
    private Animator animator;
    [SerializeField]
    private SpriteRenderer sprite;

    //Script side vars

    private PlayerMotor motor {get {return GetComponent<PlayerMotor>();}}

        #endregion

    void Update() {
        float vInput = motor.verticalInput;
        float vVel = motor.verticalSpeed;

        if(vInput != 0) sprite.flipX = vInput < 0;
        animator.SetFloat("Speed", vInput * (vInput < 0 ? -1 : 1));
        animator.SetFloat("Walk Anim Speed", (vVel * (vVel < 0 ? -1 : 1)) / 6);
    }

}

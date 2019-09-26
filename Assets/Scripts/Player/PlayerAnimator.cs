using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    
        #region Variables

    [SerializeField]
    private Animator animator;
    [SerializeField]
    private SpriteRenderer sprite;

    //Script side vars

    private Motor motor {get {return GetComponent<Motor>();}}

        #endregion

    void Update() {
        float vInput = 0;
        float vVel = 0;

        if(vInput != 0) sprite.flipX = vInput < 0;
        animator.SetFloat("Speed", vInput * (vInput < 0 ? -1 : 1));
        animator.SetFloat("Walk Anim Speed", (vVel * (vVel < 0 ? -1 : 1)) / 6);
    }

}

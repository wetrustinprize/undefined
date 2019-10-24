using UnityEngine;

[RequireComponent(typeof(Motor))]
[DisallowMultipleComponent]
public class PlayerAnimator : MonoBehaviour
{
    
        #region Variables

    [Header("Animator References")]
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private SpriteRenderer sprite;

    //Script side vars

    private Motor motor { get { return GetComponent<Motor>(); } }

        #endregion

    void Update() {
        float vInput = motor.inputAxis.x;
        float vVel = motor.GetForce("input").ActualForce.x / motor.MaxSpeed.x;
        float vDir = vVel > 0 ? 1 : -1;

        bool onAir = !motor.OnGround;
        bool drifting = vInput != 0 ? (vDir != vInput) : false;
        bool onWall = motor.OnWall && motor.HasSlow("wallSlow");
        bool dashing = motor.HasForce("dash", false);

        if(vInput != 0) {
            bool flip = false;

            if(!onWall)
            {
                flip = vInput < 0;
            }
            else
            {
                flip = motor.onRightWall ? false : true;
            }

            sprite.flipX = flip;
        }
        animator.SetFloat("SpeedH", Mathf.Abs(vVel));
        animator.SetFloat("Input", Mathf.Abs(vInput));
        animator.SetBool("OnAir", onAir);
        animator.SetBool("Drift", drifting);
        animator.SetBool("OnWall", onWall);
        animator.SetBool("Dashing", dashing);
    }

}

using UnityEngine;

[DisallowMultipleComponent]
public class PlayerAnimator : MonoBehaviour
{
    
        #region Variables

    [Header("Animator Settings")]
    [SerializeField]
    private Motor motor;
    [SerializeField]
    private float driftThreshold;

    [Header("Particles")]
    [SerializeField]
    private GameObject dustParticles;
    [SerializeField]
    private GameObject dashParticles;
    [SerializeField]
    private Transform dustSpawnTransform;

    [Header("Animator References")]
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private SpriteRenderer sprite;

    [Header("Camera Refereces")]
    [SerializeField]
    private CameraController cam;

    //Script side var

    private bool attacked;
    private bool dashParticleSpawned;

        #endregion

    void Start() {

        GetComponent<Attack>().onPerform += AttackAnim;
        GetComponent<Attack>().onAttack += AttackEffect;

    }

    void AttackAnim() {

        attacked = true;

    }

    void AttackEffect() {

        Vector2 dir = new Vector2(0.3f * motor.lastFaceDir, 0);
        cam.Push(dir, 0, 0.1f);

    }

    public void DustParticles() {

        Vector3 pos = dustSpawnTransform.position;

        Quaternion rot = Quaternion.identity;

        float offset = dustSpawnTransform.localPosition.x;

        if(motor.lastFaceDir == -1)
        {
            pos.x -= offset*2;
            rot.y = 180;
        }

        GameObject.Instantiate(dustParticles, pos, rot);

    }

    public void DashParticles() {

        Quaternion rot = Quaternion.identity;

        if(motor.lastFaceDir == 1)
        {
            rot.y = 180;
        }

        GameObject.Instantiate(dashParticles, transform.parent.position, rot, transform);

    }

    void Update() {
        float vInput = motor.inputAxis.x;
        float hVel = motor.GetForce("input").ActualForce.x / motor.MaxSpeed.x;
        float vDir = hVel > 0 ? 1 : -1;

        float vVel = motor.finalSpeed.y / 15;

        bool onAir = !motor.OnGround;
        bool drifting = vInput != 0 ? (vDir != vInput) : false;
        bool onWall = motor.OnWall && !motor.OnGround;
        bool dashing = motor.HasForce("dash", false);

        if(vInput != 0) {
            bool flip = false;

            if(!onWall && !onAir)
            {

                if(vInput < 0)
                    flip = hVel < driftThreshold;
                else
                    flip = !(hVel > driftThreshold);
            }
            else if(onWall)
            {
                flip = motor.onRightWall ? false : true;
            }
            else
            {
                flip = vInput < 0;
            }

            sprite.flipX = flip;
        }
        animator.SetFloat("SpeedH", Mathf.Abs(hVel));
        animator.SetFloat("SpeedV", vVel);
        animator.SetFloat("Input", Mathf.Abs(vInput));
        animator.SetBool("OnAir", onAir);
        animator.SetBool("Drift", drifting);
        animator.SetBool("OnWall", onWall);
        animator.SetBool("Dashing", dashing);
        animator.SetBool("Attack", attacked);

        if(!dashParticleSpawned && dashing)
        {
            dashParticleSpawned = true;
            DashParticles();
        } else if(dashParticles && !dashing)
        {
            dashParticleSpawned = false;
        }

        if(attacked) attacked = false;
    }

}

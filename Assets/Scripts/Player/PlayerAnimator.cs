using UnityEngine;

[DisallowMultipleComponent]
public class PlayerAnimator : MonoBehaviour
{
    
        #region Variables

    [Header("References")]
    [SerializeField] private Motor motor;               // Reference to the Player Motor component
    [SerializeField] private Attack attack;             // Reference to the Attack component
    [SerializeField] private Jump jump;                 // Reference to the Jump component
    [SerializeField] private Alive alive;               // Referente to the Alive component

    [Header("Animator Settings")]
    [SerializeField] private float driftThreshold;      // The threshold to play the drift animation

    [Header("Particles")]
    [SerializeField] private Transform dustSpawnTransform;      // The dust particles transform
    [SerializeField] private GameObject dustParticles;          // The dust particles when walking/running
    [Space]
    [SerializeField] private GameObject wallJumpDustParticles;  // The dust particles when walljumping
    [SerializeField] private ParticleSystem wallParticles;      // The wall dust particles when walking/running
    [SerializeField] private float wallSlideParticlesVel;       // The velocity required to enable the wall slide particles
    [Space]
    [SerializeField] private GameObject dashParticles;          // The dash particles when dashing

    [Header("Animator References")]
    [SerializeField] private Animator animator;             // Reference to the animator
    [SerializeField] private SpriteRenderer sprite;         // Reference to the sprite renderer


    //Script side var
    private CameraController cam { get { return CameraController.main; } }

    private bool attacked;
    private bool dashParticleSpawned;
    private float wallParticleDefaultX;
    private float squishPercentage;
    private float lastInput;

        #endregion

    void Start() {

        // Setup the attack events
        attack.onPerform += AttackAnim;
        attack.onAttack += AttackEffect;
        
        // Setup the alive events
        alive.onDamage += DamageEffect;

        // Setup the jump event
        jump.OnWallJump += WallJumpEffect;

        // Sets the initial transform of the wall particle
        wallParticleDefaultX = wallParticles.gameObject.transform.localPosition.x;

    }

        #region Alive Events

    private void DamageEffect() {

        cam.Shake(1, 1, 0.3f, Vector2.one);

    }

        #endregion

        #region Jump Events

    void WallJumpEffect() {

        Vector3 pos = wallParticles.transform.position;

        Quaternion rot;

        if(motor.lastFaceDir == 1)
            rot = Quaternion.Euler(0, 0, -90);
        else
            rot = Quaternion.Euler(0, 0, 90);

        GameObject.Instantiate(wallJumpDustParticles, pos, rot);
        
    }

        #endregion

        #region Attack Events

    // Called when attacks
    void AttackAnim() {

        attacked = true;

    }

    // Called when hits something
    void AttackEffect() {

        Vector2 dir = new Vector2(0.3f * motor.lastFaceDir, 0);
        cam.Push(dir, 0, 0.1f);

    }

        #endregion

        #region Particles

    // Called to create the dust particles
    public void DustParticles() {

        if(!motor.OnGround) return;

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

    // Called to create the dash particles
    public void DashParticles() {

        Quaternion rot = Quaternion.identity;

        if(motor.lastFaceDir == 1)
        {
            rot.y = 180;
        }

        GameObject.Instantiate(dashParticles, transform.parent.position, rot, transform);

    }

        #endregion

    public void Flip(bool flip) {
        sprite.flipX = flip;
    }

    // Updates the animator
    void Update() {
        // Gets all velocity info
        float vInput = motor.inputAxis.x;
        float hVel = motor.GetForce("input").ActualForce.x / motor.MaxSpeed.x;
        float vDir = hVel > 0 ? 1 : -1;
        float vFace = motor.lastFaceDir;

        float vVel = motor.finalSpeed.y;

        // Get all states
        bool onAir = !motor.OnGround;
        bool drifting = vInput != lastInput && vInput != 0 && Mathf.Abs(hVel) > driftThreshold;
        bool onWall = motor.OnWall;
        bool canWallJump = jump.canWallJump;
        bool dashing = motor.HasForce("dash", false);

        lastInput = vInput;

        // Check if should flip
        
        if(vInput != 0) {
            if(onAir && onWall && onAir && canWallJump)
            {
                Flip(!motor.OnRightWall);
            }
            else if(!onAir)
            {
                Flip(vInput < 0);
            }
            else
            {
                Flip(motor.lastFaceDir == -1);
            }
        }
        else if(sprite.flipX && motor.lastFaceDir == 1 && !attacked)
        {
            Flip(false);
        }

        // Send values to the animator
        animator.SetFloat("SpeedH", Mathf.Abs(hVel));
        animator.SetFloat("SpeedV", vVel);
        animator.SetFloat("Input", Mathf.Abs(vInput));
        animator.SetBool("OnAir", onAir);
        animator.SetBool("OnWall", onWall && canWallJump && onAir);
        
        animator.SetBool("Drift", drifting);

        //if(drifting)    animator.SetTrigger("Drift");
        if(dashing)     animator.SetTrigger("Dash");
        if(attacked)    animator.SetTrigger("Attack");

        // Squish effect
        squishPercentage = Mathf.Lerp(squishPercentage, Mathf.Clamp(vVel / -70, 0, 1), Time.fixedDeltaTime);
        sprite.transform.localScale = new Vector3(1, 1 + (0.05f * squishPercentage), 1);

        // Enables/disables wall particles
        Vector2 pos = wallParticles.gameObject.transform.localPosition;
        wallParticles.gameObject.transform.localPosition = new Vector2(wallParticleDefaultX * (sprite.flipX ? -1 : 1), pos.y);

        if(!(onWall && onAir && canWallJump && vVel < -wallSlideParticlesVel)) wallParticles.Play();

        // Spawns and checks the dash particles
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

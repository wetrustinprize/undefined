using UnityEngine;

[DisallowMultipleComponent]
public class PlayerAnimator : MonoBehaviour
{
    
        #region Variables

    [Header("Animator Settings")]
    [SerializeField] private float driftThreshold = 0.3f;      // The threshold to play the drift animation
    [SerializeField] private float dashThreshold = 2;      // The threshold to stop the dash animation

    [Header("Death Animation Settingss")]
    [SerializeField] private Transform deathFocus = null;      // Where will the camera focus when the player dies
    [SerializeField] private Material deahtSpriteMaterial = null;

    [Header("Particles")]
    [SerializeField] private GameObject ghostParticle = null;          // The ghost "particle"
    [Space]
    [SerializeField] private Transform dustSpawnTransform = null;      // The dust particles transform
    [SerializeField] private GameObject dustParticles = null;          // The dust particles when walking/running
    [Space]
    [SerializeField] private GameObject wallJumpDustParticles = null;  // The dust particles when walljumping
    [SerializeField] private ParticleSystem wallParticles = null;      // The wall dust particles when walking/running
    [SerializeField] private float wallSlideParticlesVel = 5;       // The velocity required to enable the wall slide particles

    [Header("Animator References")]
    [SerializeField] private Animator animator = null;             // Reference to the animator
    [SerializeField] private SpriteRenderer sprite = null;         // Reference to the sprite renderer


    //Script side var
    private CameraController cam { get { return CameraController.main; } }

    private bool attacked;
    private float wallParticleDefaultX;
    private float squishPercentage;
    private float lastInput;

    private Motor motor = null;
    private Attack attack = null;
    private Jump jump = null;
    private Alive alive = null;
    private PlayerInventory inventory = null;

        #endregion

    void Start() {

        // Get components
        PlayerController controller = GameManager.Player;
        motor = controller.motor;
        attack = controller.attack;
        jump = controller.jump;
        alive = controller.alive;
        inventory = controller.inventory;

        // Setup the attack events
        attack.onPerform += AttackAnim;
        attack.onAttack += AttackEffect;
        
        // Setup the alive events
        alive.onDamage += (dmg, dealer) => { DamageEffect(); };

        // Setup the jump event
        jump.OnWallJump += WallJumpEffect;

        // Sets the initial transform of the wall particle
        wallParticleDefaultX = wallParticles.gameObject.transform.localPosition.x;

    }

        #region Alive Events

    public void DeathAnimation() {
        animator.SetTrigger("Death");
    }

    private void DamageEffect() {

        cam.Shake(1, 25, 0.1f, Vector2.one);

    }

        #endregion

        #region Jump Events

    void WallJumpEffect() {

        Vector3 pos = wallParticles.transform.position;

        Quaternion rot;

        if(motor.LastFacingDir == 1)
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

        Vector2 dir = new Vector2(0.3f * motor.LastFacingDir, 0);
        cam.Push(dir, 0, 0.1f);

    }

        #endregion

        #region Particles

    // Called to call a "ghost"
    public void SpawnGhost(Sprite newSprite) {

        GameObject newGhost = Instantiate(ghostParticle, transform.position, transform.rotation);
        newGhost.GetComponent<SpriteRenderer>().sprite = newSprite;
        newGhost.GetComponent<SpriteRenderer>().flipX = sprite.flipX;

    }

    // Called to create the dust particles
    public void DustParticles() {

        if(!motor.OnGround) return;

        Vector3 pos = dustSpawnTransform.position;

        Quaternion rot = Quaternion.identity;

        float offset = dustSpawnTransform.localPosition.x;

        if(motor.LastFacingDir == -1)
        {
            pos.x -= offset*2;
            rot.y = 180;
        }

        GameObject.Instantiate(dustParticles, pos, rot);

    }

    public void DeathEffect(int stage) {

        switch(stage) {
            case 0:
                sprite.material = deahtSpriteMaterial;
                sprite.sortingLayerName = "Details";
                sprite.sortingOrder = 999;

                GameManager.Camera.SetIgnoreBoundaries(true);
                GameManager.Camera.Resize(10, 0.05f);
                GameManager.Camera.Shake(5, 5, 0.3f, Vector2.one);
                break;
            
            case 1:
                GameManager.Camera.Resize(7, 2f);
                GameManager.Camera.LookAt(deathFocus, 1f);
                GameManager.HUD.UpdateHUD(HUDType.GameOver, false);
                break;
        }

    }

        #endregion

    public void Flip(bool flip) {
        sprite.flipX = flip;
    }

    // Updates the animator
    void Update() {
        // Gets all velocity info
        float vInput = motor.InputAxis.x;
        float hVel = motor.GetForce("input").ActualForce.x / motor.MaxSpeed.x;
        float vDir = hVel > 0 ? 1 : -1;
        float vFace = motor.LastFacingDir;

        float vVel = motor.finalSpeed.y;

        // Get all states
        bool onAir = !motor.OnGround;
        bool drifting = vInput != lastInput && vInput != 0 && Mathf.Abs(hVel) > driftThreshold;
        bool onWall = motor.OnWall;
        bool canWallJump = jump.CanWallJump;
        bool dashing = motor.HasForce("dash");
        bool highVelocityDashing = dashing ? Mathf.Abs(motor.GetForce("dash").ActualForce.x) > dashThreshold : false;

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
                Flip(motor.LastFacingDir == -1);
            }
        }
        else if(sprite.flipX && motor.LastFacingDir == 1 && !attacked)
        {
            Flip(false);
        }

        // Send values to the animator
        animator.SetFloat("SpeedH", Mathf.Abs(hVel));
        animator.SetFloat("SpeedV", vVel);
        animator.SetFloat("Input", Mathf.Abs(vInput));
        animator.SetBool("OnAir", onAir);
        animator.SetBool("Dashing", highVelocityDashing);
        animator.SetBool("Atacking", attacked);
        animator.SetBool("OnWall", onWall && canWallJump && onAir);
        
        animator.SetBool("Drift", drifting);

        if(highVelocityDashing)     animator.SetTrigger("Dash");
        if(attacked)    animator.SetTrigger(inventory.WeaponItem.itemObj == null ? "Attack" : "SwordAttack");

        // Squish effect
        squishPercentage = Mathf.Lerp(squishPercentage, Mathf.Clamp(vVel / -70, 0, 1), Time.fixedDeltaTime);
        sprite.transform.localScale = new Vector3(1, 1 + (0.05f * squishPercentage), 1);

        // Enables/disables wall particles
        Vector2 pos = wallParticles.gameObject.transform.localPosition;
        wallParticles.gameObject.transform.localPosition = new Vector2(wallParticleDefaultX * (sprite.flipX ? -1 : 1), pos.y);

        if(!(onWall && onAir && canWallJump && vVel < -wallSlideParticlesVel)) wallParticles.Play();

        if(attacked) attacked = false;
    }

}

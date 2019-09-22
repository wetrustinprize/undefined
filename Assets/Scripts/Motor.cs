using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Force {

    public string Name;
    public Vector2 ForceApplied;
    public Vector2 ActualForce;
    public float TimeToStop;
    public bool ApplyGravity;

    ///<param name="f">Force to be applied</param>
    ///<param name="t">Time to the force stop</param>
    ///<param name="g">Should calculate gravity?</param>
    public Force(Vector2 f, float t = 0, bool g = true) {
        Name = "noname";
        ForceApplied = f;
        ActualForce = f;
        TimeToStop = t;
        ApplyGravity = g;
    }

    ///<param name="f">Force to be applied</param>
    ///<param name="n">Name of the force</param>
    ///<param name="t">Time to the force stop</param>
    ///<param name="g">Should calculate gravity?</param>
    public Force(string n, Vector2 f, float t = 0, bool g = true) {
        Name = n;
        ForceApplied = f;
        ActualForce = f;
        TimeToStop = t;
        ApplyGravity = g;
    }

}

[RequireComponent(typeof(Rigidbody2D))]
public class Motor : MonoBehaviour
{
    
        #region Variables

    [Header("Bypass:")]
    public bool BypassWalk;
    public bool BypassJump;

    [Header("Velocity")]
    [SerializeField]
    private float maxSpeed = 8f;
    [SerializeField]
    private float fric = 0.65f;
    [SerializeField]
    private float defaultGravityScale = 3f;

    [Header("Jump")]
    [SerializeField]
    private float jumpSpeed = 12f;
    [SerializeField]
    private int framesToWallJump;
    [SerializeField]
    private int framesToJump;
    [SerializeField]
    private int framesToPreJump;

    [Header("Ground and Wall Check")]
    [SerializeField]
    private float wallCheckDist = 0.2f;

    [Space]

    [SerializeField]
    private Vector2 wallBoxOffset;
    [SerializeField]
    private Vector2 wallBoxSize;

    [Space]

    [SerializeField]
    private Vector2 cellingBoxOffset;
    [SerializeField]
    private Vector2 cellingBoxSize;

    [Space]

    [SerializeField]
    private Vector2 groundBoxOffset;
    [SerializeField]
    private Vector2 groundBoxSize;

    [Space]

    [SerializeField]
    private LayerMask groundLayer;
    public bool OnGround;
    public bool OnWall;
    public bool OnCelling;
    public bool onRightWall;
    public bool onLeftWall;

    [Header("Other")]
    public float lastFaceDir;

    //Script side

    private Rigidbody2D rb {get { return GetComponent<Rigidbody2D>(); } }
    private BoxCollider2D col {get {return GetComponent<BoxCollider2D>(); } }
    public float verticalSpeed {get {return Mathf.Round(inputSpeed.x);}}
    public float verticalInput {get {return Mathf.Round(input.x);}}
    public bool isFacingWall {get {return (input.x == 1 && onRightWall || input.x == -1 && onLeftWall); } }

    // Input recieved by the player
    private Vector2 input;

        #region Speed Vars
    
    private List<Force> forces = new List<Force>();
    private Vector2 inputSpeed; // Calculate in CalculateVelocity()
    private Vector2 externalSpeed; // Calculate in CalculateExternalSpeed()
    private Vector2 constantExternalSpeed; // Calculate in CalculateConstantSpeed()
    private Vector2 finalSpeed { get { return inputSpeed + externalSpeed + constantExternalSpeed; } } // Final velocity

        #endregion

        #region Jump Vars

    private bool jumped;
    private bool jumpCalc;
    private bool canWallJump;
    private bool canJump;
    
    // Frame stuff
    private int framesToNotJump;
    private int framesToNotWallJump;
    private int frameToNotPreJump;

        #endregion
        #endregion

    void Start() {
        rb.gravityScale = defaultGravityScale;
    }

    public void SetInput(Vector2 v2) {

        input = v2;

    }

    // Jump void called when the players hits the jump button
    // Start the pre jump frames
    void Jump() {
        if(BypassJump) return;
        frameToNotPreJump = framesToPreJump;
        if(jumped || !canJump && !canWallJump) return;
        jumped = true;
        frameToNotPreJump = 0;
    }

    // Called to apply a force
    public void ApplyForce(Force f) {

        forces.Add(f);

    }

    // Called to set the consatant speed
    public void ApplyConstSpeed(Vector2 f) {
        constantExternalSpeed = f;
    }

    // Called to remove a force
    public void RemoveForce(Force f) {

        if(forces.Exists(force => force == f)) forces.Remove(f);

    }

    // Check if a force exists by its name
    public bool HasForce(string n) {
        return forces.Exists(force => force.Name == n);
    }

    // Called to remove a force (using only name)
    ///<param name="n">Name of the force</param>
    public void RemoveForce(string n) {

        if(forces.Exists(force => force.Name == n)) forces.Remove(forces.Find(force => force.Name == n));

    }

    void Update() {
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if(Input.GetKeyDown(KeyCode.Space)) Jump();
    }

    // Called before drawing the screen
    void FixedUpdate() {
        // TODO:
        // * make a void that apply aditional velocity to the player (like explosion impulses etc...)

        //Do all the calculation
        CalculateInputVelocity();
        CheckIsGroundedOrCelling();
        CheckNearWall();
        CalculateJump();
        CalculateExternalSpeed();
        DebugPlayer();

        // Apply the final speed
        rb.velocity = finalSpeed;
        
    }

        #region Checkers and Calculators


    // Simple void to calculate if the player is on ground
    // Also calculates the extra frames to jump
    // NEED to be called before calculating velocity and checking wall
    void CheckIsGroundedOrCelling() {
        Vector2 gpos = (Vector2)transform.position + groundBoxOffset;
        Vector2 gsiz = groundBoxSize;

        Vector2 cpos = (Vector2)transform.position + cellingBoxOffset;
        Vector2 csiz = cellingBoxSize;

        OnGround = Physics2D.OverlapBox(gpos, gsiz, 0, groundLayer);
        OnCelling = Physics2D.OverlapBox(cpos, csiz, 0, groundLayer);

        if(OnGround)
            framesToNotJump = framesToJump;

        canJump = framesToNotJump != 0 && !OnWall;
        
        CalculateFrames();
    }

    // Void to check if is near a wall
    // Also apply slowness
    void CheckNearWall() {


        // Checking if has a wall nearby
        Vector2 pos = (Vector2)transform.position + col.offset;
        Vector2 siz = col.size - new Vector2(0, 0.1f);
        
        Vector2 wjpos = (Vector2)transform.position + wallBoxOffset;
        Vector2 wjsiz = wallBoxSize;

        onRightWall = Physics2D.OverlapBox(pos + new Vector2(wallCheckDist, 0), siz, 0, groundLayer);
        onLeftWall = Physics2D.OverlapBox(pos + new Vector2(-wallCheckDist, 0), siz, 0, groundLayer);

        bool WallJump = Physics2D.OverlapBox(wjpos + new Vector2(wallCheckDist, 0), wjsiz, 0, groundLayer) || Physics2D.OverlapBox(wjpos + new Vector2(-wallCheckDist, 0), wjsiz, 0, groundLayer);

        // If is on wall or have extra frames to jump, canWallJump will be true
        canWallJump = WallJump || framesToNotWallJump != 0;

        OnWall = onRightWall || onLeftWall;

        // Do gravity calculations if is on wall
        if(OnWall) {
            // Give player extra frames to wall jump
            framesToNotWallJump = framesToWallJump;

            //Calculate gravity
            if(input.y != -1 && canWallJump && isFacingWall) {  //If is facing the wall and is 'grabbing' it
                inputSpeed = new Vector2(0, rb.velocity.y * 0.70f);
            } else if((!OnGround || input.y == -1) && isFacingWall) { //If is still on the wall but is holding down or not 'grabbing' a wall
                inputSpeed = new Vector2(0, rb.velocity.y);
            } else { //If players moves against the wall direction
                inputSpeed = new Vector2(inputSpeed.x, jumpCalc ? inputSpeed.y : rb.velocity.y);
            }
        }

    }

    //Do all the calculations for the jump
    void CalculateJump() {

        if((!canWallJump && !canJump) && jumped)
            jumped = false;

        if(BypassJump) return;

        // Jump check
        if((canJump || frameToNotPreJump != 0) && !jumpCalc && !OnWall && jumped) { //If has started the jump and didn't calculate the y velocity

            jumped = false;
            jumpCalc = true;
            inputSpeed.y = jumpSpeed;

        } else if(OnGround || OnWall) { //If is grounded

            jumpCalc = false;
            jumped = OnWall ? jumped : false;
            inputSpeed.y = OnWall ? inputSpeed.y : 0;

        } else if(!jumped) { //If has not jumped

            jumped = false;
            inputSpeed.y = rb.velocity.y;

        }

        if(jumped && !jumpCalc && (canWallJump || framesToNotWallJump != 0)) {
            float dir = 0;

            // See which direction the player will impulse depending on the wall is grabbing or the last direction
            if(lastFaceDir == 1 && onRightWall)
                dir = -1;
            else if(lastFaceDir == -1 && onLeftWall)
                dir = 1;
            else
                dir = lastFaceDir;

            // Reset variables
            jumped  = false;
            jumpCalc = true;
            framesToNotWallJump = 0;
            canWallJump = false;

            // Apply force
            inputSpeed = new Vector2(dir * (jumpSpeed / 2), jumpSpeed);
        }

    }

    // Calculates all the external speed
    void CalculateExternalSpeed() {

        if(forces == null || forces.Count == 0) return;
        externalSpeed = Vector2.zero;

        // Do gravity calculatations and stops when hit something
        forces.ForEach(f => {

            // Check is has collided
            if(f.ActualForce.y > 0 && OnCelling) f.ActualForce.y = 0;
            if(f.ActualForce.y < 0 && OnGround) f.ActualForce.y = 0;
            if(f.ActualForce.x != 0 && OnWall) f.ActualForce.x = 0;

            // Apply gravity
            if(f.ActualForce.y > 0 && f.ApplyGravity) {
                f.ActualForce += ((Vector2)Physics.gravity * rb.gravityScale) * Time.fixedDeltaTime;
                if(f.ActualForce.y <= 0) f.ActualForce.y = 0;
            }

            // Apply slowdown X if is on ground or if has timer
            if(f.ActualForce.x != 0 && OnGround && f.ApplyGravity || f.ActualForce.x != 0 && f.TimeToStop != 0) {
                f.ActualForce.x -= (f.ActualForce.x * Time.deltaTime / (f.TimeToStop != 0 ? f.TimeToStop : 0.3f));
                if(f.ForceApplied.normalized.x < 0 ? f.ActualForce.x >= 0 : f.ActualForce.x <= 0) f.ActualForce.x = 0;
            }

            // Apply slowdown Y
            if(f.ActualForce.y != 0 && f.TimeToStop != 0) {
                f.ActualForce.y -= f.ActualForce.y * Time.deltaTime / f.TimeToStop;
                if(f.ForceApplied.normalized.y < 0 ? f.ActualForce.y >= 0 : f.ActualForce.y <= 0) f.ActualForce.y = 0;
            }

        });

        forces.RemoveAll(f => f.ActualForce == Vector2.zero);

        forces.ForEach(f => {

            externalSpeed += f.ActualForce;

        });

    }

    // Calculates the velocity given by INPUT
    void CalculateInputVelocity() {
        
        if(BypassWalk) return;

        // X velocity calc w/ friction
        if(input.x > 0) {
            lastFaceDir = 1;
            inputSpeed.x = Mathf.Clamp(inputSpeed.x + fric, -maxSpeed, maxSpeed);
        } else if(input.x < 0) {
            lastFaceDir = -1;
            inputSpeed.x = Mathf.Clamp(inputSpeed.x - fric, -maxSpeed, maxSpeed);
        } else {
            inputSpeed.x = Mathf.Lerp(inputSpeed.x, 0, (fric / 2) * Time.deltaTime * 30);
        }

    }

    // Called every Update() to remove a frame from the extra frames
    void CalculateFrames() {

        if(framesToNotJump != 0)
            framesToNotJump--;
        
        if(framesToNotWallJump != 0)
            framesToNotWallJump--;

        if(frameToNotPreJump != 0)
            frameToNotPreJump--;

        

    }

        #endregion

    void DebugPlayer() {
        // Debug stuff //
        // Blue:    x velocity
        // Green:   y velocity
        // Red:     actual velocity
        // Yellow:  extra velocitys
        Debug.DrawLine(transform.position, transform.position + new Vector3(maxSpeed * input.x, 0, 0) * 0.5f, Color.blue, Time.deltaTime, false);
        Debug.DrawLine(transform.position, transform.position + new Vector3(inputSpeed.x, 0, 0) * 0.4f, Color.red, Time.deltaTime, false);
        Debug.DrawLine(transform.position, transform.position + new Vector3(0, inputSpeed.y, 0) * 0.5f, Color.green, Time.deltaTime, false);
        Debug.DrawLine(transform.position, transform.position + ((Vector3)externalSpeed + (Vector3)constantExternalSpeed) * 0.3f, Color.yellow, Time.deltaTime, false);
    }

}

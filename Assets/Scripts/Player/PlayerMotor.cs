using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    
        #region Variables

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

    [Header("Ground and Wall Check")]
    [SerializeField]
    private float wallCheckDist = 0.2f;
    [SerializeField]
    private Vector2 wallBoxOffset;
    [SerializeField]
    private Vector2 wallBoxSize;
    [SerializeField]
    private int framesToWallJump;
    [Space]

    [SerializeField]
    private Vector2 groundBoxOffset;
    [SerializeField]
    private Vector2 groundBoxSize;
    [SerializeField]
    private int framesToJump;
    [SerializeField]
    private int framesToPreJump;
    [Space]

    [SerializeField]
    private LayerMask groundLayer;
    public bool IsGrounded;
    public bool OnWall;

    //Script side

    private Rigidbody2D rb {get { return GetComponent<Rigidbody2D>(); } }
    private BoxCollider2D col {get {return GetComponent<BoxCollider2D>(); } }
    public float verticalSpeed {get {return Mathf.Round(inputSpeed.x);}}
    public float verticalInput {get {return Mathf.Round(input.x);}}

    // Input recieved by the player
    private Vector2 input;

        #region Speed Vars
    
    private Vector2 inputSpeed; // Calculate in CalculateVelocity()
    private Vector2 externalSpeed; // TODO
    private Vector2 constantExternalSpeed; // TODO
    private Vector2 finalSpeed { get { return inputSpeed + externalSpeed + constantExternalSpeed; } } // Final velocity

        #endregion

        #region Checker Vars

    private bool onRightWall;
    private bool onLeftWall;

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

    //Called every frame
    void Update() {

        // TODO:
        // * apply all the input commands inside PlayerController class

        // Get the axis from the default input system from unity
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        // Get the space key to jump
        if(Input.GetKeyDown(KeyCode.Space)) Jump();
    }

    // Called before drawing the screen
    void FixedUpdate() {
        // TODO:
        // * make a void that apply aditional velocity to the player (like explosion impulses etc...)

        //Do all the calculation
        CheckIsGrounded();
        CalculateVelocity();
        CheckNearWall();
        DebugPlayer();

        // Apply the final speed
        rb.velocity = finalSpeed;
        
    }

    // Jump void called when the players hits the jump button
    // Start the pre jump frames
    void Jump() {
        frameToNotPreJump = framesToJump;
        if(jumped || !canJump && !canWallJump) return;
        jumped = true;
        frameToNotPreJump = 0;
    }

        #region Checkers and Calculators


    // Simple void to calculate if the player is on ground
    // Also calculates the extra frames to jump
    // NEED to be called before calculating velocity and checking wall
    void CheckIsGrounded() {
        Vector2 pos = (Vector2)transform.position + groundBoxOffset;
        Vector2 siz = groundBoxSize;

        IsGrounded = Physics2D.OverlapBox(pos, siz, 0, groundLayer);

        if(IsGrounded)
            framesToNotJump = framesToJump;

        canJump = framesToNotJump != 0;
        
        CalculateFrames();
    }

    // Void to check if is near a wall
    // Also calculate extra frames to walljump
    // Do the calculation of the walljump
    void CheckNearWall() {


        // Checking if has a wall nearby
        // By checking that, a slower gravity will be applied so the player will be "grabbed" at the wall
        Vector2 pos = (Vector2)transform.position + col.offset;
        Vector2 siz = col.size - new Vector2(0, 0.1f);
        
        Vector2 wjpos = (Vector2)transform.position + wallBoxOffset;
        Vector2 wjsiz = wallBoxSize;

        onRightWall = Physics2D.OverlapBox(pos + new Vector2(wallCheckDist, 0), siz, 0, groundLayer);
        onLeftWall = Physics2D.OverlapBox(pos + new Vector2(-wallCheckDist, 0), siz, 0, groundLayer);

        var wallJump = Physics2D.OverlapBox(wjpos + new Vector2(wallCheckDist, 0), wjsiz, 0, groundLayer) || Physics2D.OverlapBox(wjpos + new Vector2(-wallCheckDist, 0), wjsiz, 0, groundLayer);

        OnWall = onRightWall || onLeftWall;
        
        if(wallJump && !IsGrounded) {
            framesToNotWallJump = framesToWallJump;
            canWallJump = true;
        } else {
            if(framesToNotWallJump == 0 || IsGrounded) {
                canWallJump = false;
            }
        }

        if(!OnWall)
            return;

        if((input.x == 1 && onRightWall || input.x == -1 && onLeftWall) && input.y != -1 && canWallJump) {
            inputSpeed = new Vector2(0, rb.velocity.y * 0.70f);
        } else if(input.x == 0 || !canWallJump) {
            inputSpeed = new Vector2(0, rb.velocity.y);
        }

        if(jumped && !jumpCalc && onRightWall != onLeftWall && canWallJump && !IsGrounded) {
            jumpCalc = true;
            inputSpeed = new Vector2((onLeftWall ? 1 : -1) * (jumpSpeed / 2), jumpSpeed);
            Debug.Log(inputSpeed);
        }

    }

    // Calculates the velocity on GROUND
    // Do not calculate walljumps
    // Calculate frame pre jump
    void CalculateVelocity() {
        
        // Ground and jump check
        if((jumped || frameToNotPreJump != 0) && !jumpCalc && (!OnWall || OnWall && IsGrounded)) { //If has started the jump and didn't calculate the y velocity

            jumped = false;
            jumpCalc = true;
            inputSpeed.y = jumpSpeed;

        } else if(IsGrounded || OnWall) { //If is grounded

            jumpCalc = false;
            jumped = OnWall ? jumped : false;
            inputSpeed.y = OnWall ? inputSpeed.y : 0;

        } else { //If is not grounded

            jumped = false;
            inputSpeed.y = rb.velocity.y;

        }

        // X velocity calc w/ friction
        if(input.x > 0) {
            inputSpeed.x = Mathf.Clamp(inputSpeed.x + fric, -maxSpeed, maxSpeed);
        } else if(input.x < 0) {
            inputSpeed.x = Mathf.Clamp(inputSpeed.x - fric, -maxSpeed, maxSpeed);
        } else {
            inputSpeed.x = Mathf.Lerp(inputSpeed.x, 0, (fric / 2) * Time.deltaTime * 30);
        }

    }

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

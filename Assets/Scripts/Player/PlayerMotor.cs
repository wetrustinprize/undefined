using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    
        #region Variables

    [Header("Velocity")]
    [SerializeField]
    private float maxSpeed;
    [SerializeField]
    private float fric;
    [SerializeField]
    private float defaultGravityScale;

    [Header("Jump")]
    [SerializeField]
    private float jumpSpeed;

    [Header("Ground Check")]
    [SerializeField]
    private LayerMask groundLayer;
    [SerializeField]
    private Transform groundChecker;
    public bool IsGrounded;
    public bool OnWall;

    //Script side

    private Rigidbody2D rb {get { return GetComponent<Rigidbody2D>(); } }
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

        #region Jump Vars

    private bool jumped;
    private bool jumpCalc;

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

        // Apply the final speed
        rb.velocity = finalSpeed;
        
    }

    void Jump() {
        if(jumped || !OnWall && !IsGrounded) return;
        jumped = true;
    }

        #region Checkers and Calculators

    // Simple void to calculate if the player is on ground
    // NEED to be called before calculating velocity and checking wall
    void CheckIsGrounded() {
        IsGrounded = Physics2D.OverlapCircle(groundChecker.position, 0.1f, groundLayer);
    }

    void CheckNearWall() {

        // Checking if has a wall nearby
        // By checking that, a slower gravity will be applied so the player will be "grabbed" at the wall

        OnWall = Physics2D.OverlapBox((Vector2)transform.position + new Vector2(input.x * 0.1f, 0), new Vector2(1.1f, 2f), 0, groundLayer);
        
        if(OnWall && input.x != 0) {
            inputSpeed = new Vector2(0, rb.velocity.y / 5);
        }

    }

    void CalculateVelocity() {
        
        // Ground and jump check
        if(jumped && !jumpCalc) { //If has started the jump and didn't calculate the y velocity
            jumpCalc = true;
            inputSpeed.y = jumpSpeed;
        } else if(IsGrounded || OnWall) { //If is grounded
            jumpCalc = false;
            jumped = false;
            inputSpeed.y = 0;
        } else { //If is not grounded
            inputSpeed.y = rb.velocity.y;
        }

        // X velocity calc w/ friction
        if(input.x > 0) {
            inputSpeed.x = Mathf.Clamp(inputSpeed.x + fric, -maxSpeed, maxSpeed);
        } else if(input.x < 0) {
            inputSpeed.x = Mathf.Clamp(inputSpeed.x - fric, -maxSpeed, maxSpeed);
        } else {
            inputSpeed.x = Mathf.Lerp(inputSpeed.x, 0, fric * Time.deltaTime * 30);
        }

        // Debug stuff //
        // Blue:    target velocity
        // Red:     actual velocity
        // Green:   gravity
        // Yellow:  total velocity
        Debug.DrawLine(transform.position, transform.position + new Vector3(maxSpeed * input.x, 0, 0) * 0.5f, Color.blue, Time.deltaTime, false);
        Debug.DrawLine(transform.position, transform.position + new Vector3(inputSpeed.x, 0, 0) * 0.4f, Color.red, Time.deltaTime, false);
        Debug.DrawLine(transform.position, transform.position + new Vector3(0, inputSpeed.y, 0) * 0.5f, Color.green, Time.deltaTime, false);
        Debug.DrawLine(transform.position, transform.position + (Vector3)finalSpeed * 0.3f, Color.yellow, Time.deltaTime, false);

    }

        #endregion

}

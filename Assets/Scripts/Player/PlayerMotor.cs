using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    
    #region Variaveis

    [Header("Velocity")]
    [SerializeField]
    private float maxSpeed;
    [SerializeField]
    private float fric;

    [Header("Jump")]
    [SerializeField]
    private float jumpSpeed;

    [Header("Ground Check")]
    [SerializeField]
    private LayerMask groundLayer;
    [SerializeField]
    private Transform groundChecker;
    public bool IsGrounded;

    //Script side

    public float verticalSpeed {get {return Mathf.Round(speed.x);}}
    public float verticalInput {get {return Mathf.Round(input.x);}}

    private Vector2 input;
    private Vector2 speed;
    private Vector2 externalSpeed;
    private Vector2 constantExternalSpeed;
    private Vector2 finalSpeed { get { return speed + externalSpeed + constantExternalSpeed; } }

    private bool jumped;
    private bool jumpCalc;

    // Get {} references
    
    private Rigidbody2D rb {get { return GetComponent<Rigidbody2D>(); } }

    #endregion

    public void Update() {
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if(Input.GetKeyDown(KeyCode.Space)) Jump();
        Debug.Log(input);
    }

    public void FixedUpdate() {
        CheckIsGrounded();
        CalculateVelocity();
        rb.velocity = finalSpeed;
        
    }

    void CheckIsGrounded() {
        IsGrounded = Physics2D.OverlapCircle(groundChecker.position, 0.1f, groundLayer);
    }

    void CalculateVelocity() {
        
        if(jumped && !jumpCalc) {
            jumpCalc = true;
            speed.y = jumpSpeed;
        } else if(IsGrounded) {
            jumpCalc = false;
            jumped = false;
            speed.y = rb.velocity.y;
        } else {
            speed.y = rb.velocity.y;
        }

        if(input.x > 0) {
            speed.x = Mathf.Clamp(speed.x + fric, -maxSpeed, maxSpeed);
        } else if(input.x < 0) {
            speed.x = Mathf.Clamp(speed.x - fric, -maxSpeed, maxSpeed);
        } else {
            speed.x = Mathf.Lerp(speed.x, 0, fric * Time.deltaTime * 30);
        }

        //Debug stuff
        Debug.DrawLine(transform.position, transform.position + new Vector3(maxSpeed * input.x, 0, 0) * 0.5f, Color.blue, Time.deltaTime, false);
        Debug.DrawLine(transform.position, transform.position + new Vector3(speed.x, 0, 0) * 0.4f, Color.red, Time.deltaTime, false);
        Debug.DrawLine(transform.position, transform.position + new Vector3(0, speed.y, 0) * 0.5f, Color.green, Time.deltaTime, false);
        Debug.DrawLine(transform.position, transform.position + (Vector3)speed * 0.3f, Color.yellow, Time.deltaTime, false);

    }

    void Jump() {
        if(jumped || !IsGrounded) return;
        jumped = true;
    }

}

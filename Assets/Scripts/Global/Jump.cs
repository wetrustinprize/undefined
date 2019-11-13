using UnityEngine;
using Undefined.Force;

[RequireComponent(typeof(Motor))]
[DisallowMultipleComponent]
public class Jump : MonoBehaviour
{

    #region Variables

    [Header("Force settings")]
    public Vector2 JumpForce = new Vector2(0, 8);
    public Vector2 WallJumpForce = new Vector2(4, 4);
    
    [Header("Jump Settings")]
    public int maxWallJumps = 1;

    [Space]

    public bool allowWallJumps = true;
    public bool allowGroundJumps = true;

    [Header("Wall slow settings")]
    public bool applyWallSlow = true;
    public float normalWallSlow = -0.9f;
    public float noWallJumpSlow = -0.3f;

    // Script-side variables
    private Motor m { get { return GetComponent<Motor>(); } }

    private bool groundJumped;

    private int wallJumps = 0;
    
    public bool canWallJump { get { return wallJumps < maxWallJumps && m.OnWall && m.onRightWall != m.onLeftWall && allowWallJumps; } }
    public bool canGroundJump { get { return m.OnGround && !groundJumped && allowGroundJumps; } }

    public bool hasGroundJumped { get { return groundJumped; } }
    public bool hasWallJumped { get { return wallJumps == maxWallJumps; } }

    #endregion

    void Start() {

        m.onTouchGround += Grounded;
        m.onTouchWall += OnWall;
        m.onUntouchWall += OnOutWall;
        m.onReceiveInput += AirStrafe;

    }

        #region Functions

    ///<summary>Forces a ground jump</summary>
    public void GroundJump() {

        Vector2 dir = Vector2.zero;

        dir += JumpForce;
        groundJumped = true;

        m.AddForce(new Force("jump", dir), false, true, true);
        m.RemoveSlow("wallSlow");

    }

    ///<summary>Force a walljump</summary>
    ///<param name="dirToJump">0 = auto, 1 = right, -1 = left</param>
    public void WallJump(int dirToJump = 0) {

        Vector2 dir = WallJumpForce;
        int directionToJump = dirToJump != 0 ? dirToJump : (m.onRightWall ? -1 : 1);
        dir.x *= directionToJump;

        if(m.inputAxis.x != (int)Mathf.Clamp(dir.x, -1, 1) && m.inputAxis.x != 0)
        {
            dir.x = 0;
        }

        wallJumps++;

        m.AddForce(new Force("jump", dir), false, true, true);
        m.RemoveSlow("wallSlow");

    }

    // Function to execute the jump
    public void Execute() {

        Vector2 dir = Vector2.zero;
        
        if(canGroundJump) // Ground jump
        {
            GroundJump();
        }
        else if(canWallJump) // Wall jump
        {
            WallJump();
        }

    }

    // Function to remove horizontal force on air
    void AirStrafe(Vector2 input) {
        
        if(m.GetForce("jump") == null) return;
        if(m.OnGround) return;
        
        float dir = m.GetForce("jump").Direction().x;

        if(dir != input.x)
        {
            m.GetForce("jump").ActualForce.x = 0;
        }

    }

        #region Event functions

    // Called when OnTouchGround is called
    void Grounded() {

        m.RemoveForce("jump");
        m.RemoveSlow("wallSlow");

        wallJumps = 0;

        if(groundJumped)
        {
            groundJumped = false;
        }

    }

    // Called when OnTouchWall is called
    void OnWall(int dir) {

        m.ResetGravity();
        m.RemoveForce("jump");

        if(!applyWallSlow) return;

        float totalSlow = canWallJump ? normalWallSlow : noWallJumpSlow;
        Slow s = new Slow("wallSlow", new Vector2(0, totalSlow), SlowType.Gravity);

        if(m.inputAxis.x == dir)
        {
            m.AddSlow(s, true);
        }

    }

    // Called when OnUntouchWall is called
    void OnOutWall(int dir) {
        
        m.RemoveSlow("wallSlow");

    }

        #endregion

        #endregion

}

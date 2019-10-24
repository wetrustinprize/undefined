using UnityEngine;
using Untitled.Motor;

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

    // Script-side variables
    private Motor m { get { return GetComponent<Motor>(); } }

    private bool groundJumped;

    private int wallJumps = 0;
    public bool canWallJump { get { return wallJumps < maxWallJumps; } }

    #endregion

    void Start() {

        m.onTouchGround += Grounded;
        m.onTouchWall += OnWall;
        m.onUntouchWall += OnOutWall;
        m.onReceiveInput += AirStrafe;

    }

        #region Functions

    // Function to execute the jump
    public void Execute() {

        Vector2 dir = Vector2.zero;
        
        if(m.OnGround && !groundJumped && allowGroundJumps) // Ground jump
        {
            dir += JumpForce;
            groundJumped = true;
        }
        else if(m.OnWall && m.onRightWall != m.onLeftWall && canWallJump && allowWallJumps) // Wall jump
        {
            Vector2 wallDir = WallJumpForce;
            wallDir.x *= m.onRightWall ? -1 : 1;

            if(m.inputAxis.x != (int)Mathf.Clamp(wallDir.x, -1, 1) && m.inputAxis.x != 0)
            {
                wallDir.x = 0;
            }

            dir += wallDir;

            wallJumps++;

        }

        if(dir != Vector2.zero)
            m.AddForce(new Force("jump", dir), false, true, true);
        m.RemoveSlow("wallSlow");

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
        float totalSlow = canWallJump ? -0.9f : -0.3f;
        Slow s = new Slow("wallSlow", new Vector2(0, totalSlow), SlowType.Gravity);

        if(m.inputAxis.x == dir)
        {
            m.AddSlow(s, true);
        }
        m.RemoveForce("jump");

    }

    // Called when OnUntouchWall is called
    void OnOutWall(int dir) {
        
        m.RemoveSlow("wallSlow");

    }

        #endregion

        #endregion

}

using UnityEngine;
using System;
using Undefined.Force;

[RequireComponent(typeof(Motor))]
[DisallowMultipleComponent]
public class Jump : MonoBehaviour
{

    #region Variables

    [Header("Force settings")]
    [SerializeField] private Vector2 jumpForce = new Vector2(0, 8);
    [SerializeField] private Vector2 wallJumpForce = new Vector2(4, 4);
    
    [Header("Jump Settings")]
    [SerializeField] private int maxWallJumps = 1;

    [Space]

    [SerializeField] private bool allowWallJumps = true;
    [SerializeField] private bool allowGroundJumps = true;

    [Header("Wall slow settings")]
    [SerializeField] private bool applyWallSlow = true;
    [SerializeField] private float normalWallSlow = -0.9f;
    [SerializeField] private float noWallJumpSlow = -0.3f;

    // Actions
    public Action OnJump;
    public Action OnWallJump;

    // Script-side variables
    private Motor m { get { return GetComponent<Motor>(); } }

    private bool groundJumped;

    private int wallJumps = 0;
    
    // Public acess variables
    public bool CanWallJump { get { return wallJumps < maxWallJumps && m.OnRightWall != m.OnLeftWall && allowWallJumps; } }
    public bool CanGroundJump { get { return m.OnGround && !groundJumped && allowGroundJumps; } }

    public bool HasGroundJumped { get { return groundJumped; } }
    public bool HasWallJumped { get { return wallJumps == maxWallJumps; } }

    #endregion

    void Start() {

        m.onTouchGround += Grounded;
        m.onTouchWall += OnWall;
        m.onUntouchWall += OnOutWall;
        m.onReceiveInput += AirStrafe;

    }

        #region Functions

    ///<summary>Sets new jump force</summary>
    public void SetNewJumpForce(Vector2 newJumpForce) {

        jumpForce = newJumpForce;

    }

    public void SetNewWallJumpForce(Vector2 newWallJumpForce) {

        wallJumpForce = newWallJumpForce;

    }

    ///<summary>Forces a ground jump</summary>
    public void GroundJump() {

        Vector2 dir = Vector2.zero;

        dir += jumpForce;
        groundJumped = true;

        m.AddForce(new Force("jump", dir), false, true, true);
        m.RemoveSlow("wallSlow");
        OnJump?.Invoke();

    }

    ///<summary>Force a walljump</summary>
    ///<param name="dirToJump">0 = auto, 1 = right, -1 = left</param>
    public void WallJump(int dirToJump = 0) {

        Vector2 dir = wallJumpForce;
        int directionToJump = dirToJump != 0 ? dirToJump : (m.OnRightWall ? -1 : 1);
        dir.x *= directionToJump;

        if(m.InputAxis.x != (int)Mathf.Clamp(dir.x, -1, 1) && m.InputAxis.x != 0)
        {
            dir.x = 0;
        }

        wallJumps++;

        m.ResetGravity();
        m.AddForce(new Force("jump", dir), false, true, true);
        m.RemoveSlow("wallSlow");
        OnWallJump?.Invoke();

    }

    // Function to execute the jump
    public void Execute() {

        Vector2 dir = Vector2.zero;
        
        if(CanGroundJump) // Ground jump
        {
            GroundJump();
        }
        else if(CanWallJump) // Wall jump
        {
            WallJump();
        }

    }

    void WallSlowRemove(Vector2 input) {
        if(!m.HasSlow("wallSlow")) return;
        
        int dir = 0;
        dir += m.OnRightWall ? 1 : 0;
        dir -= m.OnLeftWall ? 1 : 0;

        if(input.x != dir)
            m.RemoveSlow("wallSlow");
    }

    // Function to remove horizontal force on air
    void AirStrafe(Vector2 input) {
        
        if(!m.HasForce("jump", false)) return;
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

        if(CanWallJump)
            m.ResetGravity();
        m.RemoveForce("jump");

        if(!applyWallSlow) return;

        float totalSlow = CanWallJump ? normalWallSlow : noWallJumpSlow;
        Slow s = new Slow("wallSlow", new Vector2(0, totalSlow), SlowType.Gravity);

        if(m.InputAxis.x == dir && totalSlow != 0)
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

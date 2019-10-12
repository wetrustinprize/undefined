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

    // Script-side variables
    private Motor m { get { return GetComponent<Motor>(); } }

    private bool canJump;
    private bool groundJumped;
    private bool wallJumped;

    #endregion

    void Start() {

        m.onTouchGround += Grounded;
        m.onTouchWall += OnWall;
        m.onUntouchWall += OnOutWall;

    }

    void Wall(int dir)
    {
        Debug.Log(dir);
    }

    public void Execute() {

        Vector2 dir = Vector2.zero;
        
        if(m.OnGround && !groundJumped)
        {
            dir += JumpForce;
            groundJumped = true;
        }
        else if(m.OnWall && m.onRightWall != m.onLeftWall && !wallJumped)
        {
            Vector2 wallDir = WallJumpForce;
            wallDir.x *= m.onRightWall ? -1 : 1;

            dir += wallDir;
            wallJumped = true;

        }

        if(dir != Vector2.zero)
            m.AddForce(new Force("jump", dir, 1, true));
        m.RemoveSlow("wallSlow");

    }

    void Grounded() {

        m.RemoveForce("jump");

        if(groundJumped)
            groundJumped = false;
        if(wallJumped)
            wallJumped = false;
        
    }

    void OnWall(int dir) {

        m.ResetGravity();
        Slow s = new Slow("wallSlow", new Vector2(0, -0.9f), SlowType.Gravity);
        
        m.AddSlow(s, true);
        m.RemoveForce("jump");

    }

    void OnOutWall(int dir) {
        
        m.RemoveSlow("wallSlow");

    }

}

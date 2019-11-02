using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(Motor))]
[RequireComponent(typeof(Jump))]
[DisallowMultipleComponent]
public class PlayerJump : MonoBehaviour
{
    
        #region Variables

    [Header("Extra Frames")]
    public float extraFramesGroundJump;
    public float extraFramesWallJump;

    // Script side variables
    private float framesToGroundJump;
    private float framesToWallJump;

    private bool countWallJump;
    private bool countGroundJump;

    private bool canWallJump { get { return (framesToWallJump > 0); } }
    private bool canGroundJump { get { return (framesToGroundJump > 0); } }

    private Motor motor { get { return GetComponent<Motor>(); } }
    private Jump jump { get {return GetComponent<Jump>(); } }

        #endregion

    void Start() {

        motor.onTouchGround += GroundRestart;
        motor.onTouchWall += (wall) => { WallRestart(); };

        motor.onUntouchGround += () => { countGroundJump = true; };
        motor.onUntouchWall += (wall) => { countWallJump = true; };

    }

    public void Update() {

        if(countWallJump && framesToWallJump > 0)
            framesToWallJump = Mathf.Clamp(framesToWallJump - Time.deltaTime, 0, extraFramesWallJump);

        if(countGroundJump && framesToGroundJump > 0)
            framesToGroundJump = Mathf.Clamp(framesToGroundJump - Time.deltaTime, 0, extraFramesGroundJump);

    }

    public void Execute() {

        if(canGroundJump && !jump.hasGroundJumped)
            jump.GroundJump();
        else if(canWallJump && !jump.hasWallJumped)
            jump.WallJump();

    }

    void GroundRestart() {

        framesToGroundJump = extraFramesGroundJump;
        framesToWallJump = 0;

        countGroundJump = false;

    }

    void WallRestart() {

        framesToWallJump = extraFramesWallJump;

        countWallJump = false;

    }

}

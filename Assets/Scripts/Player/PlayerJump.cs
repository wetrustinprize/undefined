using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(Motor))]
[RequireComponent(typeof(Jump))]
[DisallowMultipleComponent]
public class PlayerJump : MonoBehaviour
{
    
        #region Variables

    [Header("Extra Frames")]
    [SerializeField] private float extraTimeGroundJump;
    [SerializeField] private float extraTimeWallJump;
    [SerializeField] private float extraTimePreJump;

    // Script side variables
    private float timeToGroundJump;
    private float timeToWallJump;
    private float timeToPreJump;

    private bool countWallJump;
    private bool countGroundJump;

    private bool canWallJump { get { return (timeToWallJump > 0); } }
    private bool canGroundJump { get { return (timeToGroundJump > 0); } }

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

        if(timeToPreJump > 0)
        {
            if(motor.OnGround) {
                Execute();
                timeToPreJump = 0f;
            }
            else
            {
                timeToPreJump = Mathf.Clamp(timeToPreJump - Time.deltaTime, 0, extraTimePreJump);
            }
        }

        if(countWallJump && timeToWallJump > 0)
            timeToWallJump = Mathf.Clamp(timeToWallJump - Time.deltaTime, 0, extraTimeWallJump);

        if(countGroundJump && timeToGroundJump > 0)
            timeToGroundJump = Mathf.Clamp(timeToGroundJump - Time.deltaTime, 0, extraTimeGroundJump);

    }

    public void Execute() {

        if(canGroundJump && !jump.HasGroundJumped && jump.allowGroundJumps)
            jump.GroundJump();
        else if(canWallJump && !jump.HasWallJumped && jump.allowWallJumps)
            jump.WallJump();
        else if(!motor.OnGround && jump.HasGroundJumped)
            timeToPreJump = extraTimePreJump;


    }

    void GroundRestart() {

        timeToGroundJump = extraTimeGroundJump;
        timeToWallJump = motor.OnWall ? extraTimeWallJump : 0;

        countGroundJump = false;

    }

    void WallRestart() {

        timeToWallJump = extraTimeWallJump;
        countWallJump = false;


    }

}

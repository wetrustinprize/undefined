using UnityEngine;
using System;
using System.Collections.Generic;
using Undefined.Force;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class Motor : MonoBehaviour
{
    
        #region Variables

    [Header("Bypass:")]
    [SerializeField] private bool bypassInput;
    [SerializeField] private bool bypassGravity;

    [Header("Velocity")]
    [SerializeField] private Vector2 maxSpeed = Vector2.one;
    [SerializeField] private float fric = 0.65f;
    [SerializeField] private float gravityScale = 3f;

    [Space]

    [SerializeField] private LayerMask collisionLayer = 0;
    [SerializeField] private bool onGround;
    [SerializeField] private bool onWall;
    [SerializeField] private bool onCelling;
    [SerializeField] private bool onRightWall;
    [SerializeField] private bool onLeftWall;

    [Header("Other")]
    [SerializeField] private int lastFaceDir;
    [SerializeField] private Vector2 inputAxis; // Input received by the player
    

    //Script side

    private Rigidbody2D rb {get { return GetComponent<Rigidbody2D>(); } }
    private BoxCollider2D col {get {return GetComponent<BoxCollider2D>(); } }

        #region Public Acess Variables

    public LayerMask CollisionLayer { get { return collisionLayer; } }

    public Vector2 MaxSpeed { get { return maxSpeed; } }
    public Vector2 InputAxis { get { return inputAxis; } }

    public int LastFacingDir { get { return lastFaceDir; } }

    public bool OnGround { get { return onGround; } }

    public bool OnWall { get { return onWall; } }
    public bool OnRightWall { get { return onRightWall; } }
    public bool OnLeftWall { get { return onLeftWall; } }

        #endregion

        #region Events

    public event Action onTouchGround;
    public event Action onUntouchGround;

    public event Action onTouchCelling;
    public event Action onUntouchCelling;

    public event Action<Vector2> onReceiveInput;

    public event Action<int> onTouchWall;
    public event Action<int> onUntouchWall;

        #endregion

        #region Speed Vars
    
    [SerializeField] private List<Force> externalForces = new List<Force>();  // List of external forces
    [SerializeField] private List<Force> constantForces = new List<Force>();  // List of constant external forces
    [SerializeField] private List<Slow> slowness = new List<Slow>();          // List of all slowness
    [SerializeField] private bool newSlow = false;
    [SerializeField] private bool resetGrav = false;
    [SerializeField] private Dictionary<SlowType, Vector2> totalSlowness = new Dictionary<SlowType, Vector2>() { // All slowness to be applied
        { SlowType.Input, Vector2.one },
        { SlowType.Gravity, Vector2.one },
        { SlowType.External, Vector2.one },
        { SlowType.Constant, Vector2.one }
    };
    private Vector2 externalSpeed;                                                          // Calculate in CalculateExternalSpeed()
    private Vector2 constantExternalSpeed;                                                  // Calculate in CalculateConstantSpeed()
    public Vector2 finalSpeed { get { return  externalSpeed + constantExternalSpeed; } }    // Final velocity

        #endregion

        #region Colliders

    // Save all information about the colliders
    private Vector2 _celColliderPosition;   // Celling collider position
    private Vector2 _grdColliderPosition;   // Ground collider position
    private Vector2 _walRColliderPosition;  // Right wall collider position
    private Vector2 _walLColliderPosition;  // Left wall collider position
    private Vector2 _ckcGCColliderSize;     // Celling and Floor collider size
    private Vector2 _ckcWColliderSize;      // Wall collider size

    // Information about where is colliding
    public Vector2 CellingColliderPosition { get { return _celColliderPosition; } }
    public Vector2 GroundColliderPosition { get { return _grdColliderPosition; } }
    public Vector2[] WallsColliderPosition { get { return new Vector2[] {_walRColliderPosition, _walLColliderPosition}; } }
    public Vector2 GroundCellingColliderSize { get { return _ckcGCColliderSize; } }
    public Vector2 WallColliderSize { get { return _ckcWColliderSize; } }


    //Check if the collider has changed size or not
    private Vector2 _lastColliderSiz = Vector2.zero;
    private Vector2 _lastColliderOffset = Vector2.zero;

        #endregion

        #endregion

    // Initial setup
    void Start() {
        lastFaceDir = 1;
        gravityScale = !bypassGravity ? gravityScale : 0;

        AddForce(new Force("input", Vector2.zero, 0, false), false);
        AddForce(new Force("grav", Vector2.zero, 0, false), true);
    }

    // Called to apply a input
    ///<summary>Called to move the motor using the fric and maxspeed values</summary>
    ///<param name="input">The direction to move to</param>
    public void ReceiveInput(Vector2 input) {

        inputAxis = input;

        onReceiveInput?.Invoke(input.normalized);

        if(input.normalized.x != 0)
            lastFaceDir = (int)input.normalized.x;

    }

        #region Public Functions

    public void SetNewMaxSpeed(Vector2 newMaxSpeed) {

        maxSpeed = newMaxSpeed;

    }

    ///<summary>Resets tha gravity</summary>
    public void ResetGravity() {
        resetGrav = true;
    }

        #region Force Related

    // Called to apply a force
    ///<summary>Adds a new force to the motor.</summary>
    ///<param name="force">The force to apply</param>
    ///<param name="constant">Is it constant</param>
    ///<param name="resetGrav">Reset gravity?</param>
    ///<param name="replaceForce">Replace force?</param>
    public void AddForce(Force force, bool constant = false, bool resetGvt = false, bool replaceForce = false) {
        
        ref List<Force> list = ref externalForces;
        if(constant)
            list = ref constantForces;

        if(HasForce(force.Name, constant))
        {
            int index = list.FindIndex(f2 => f2.Name == force.Name);
            if(replaceForce) {
                list[index] = force;
            }
            else
            {
                list[index].ActualForce += force.ForceApplied;
                list[index].ForceApplied += force.ForceApplied;
                if(force.TimeToStop != 0)
                {
                    list[index].TimeToStop = force.TimeToStop;
                    list[index].Timer = 0;
                }
            }
            if(resetGvt)
            {
                resetGrav = true;
                list[index].Gravity = Vector2.zero;
            }
        }
        else
        {
            list.Add(force);
        }
    }

    // Check if a force exists by its name
    ///<summary>Check if has a force with the name</string>
    ///<param name="name">Name of the force</param>
    ///<param name="constant">Is it a constant force?</param>
    public bool HasForce(string name, bool constant) {
        ref List<Force> list = ref externalForces;
        if(constant)
            list = ref constantForces;

        return list.Exists(force => force.Name == name);

    }

    // Get an existing force
    ///<summary>Gets and existing force</string>
    ///<param name="name">Name of the force</param>
    ///<param name="constant">Is it a constant force?</param>
    public Force GetForce(string name, bool constant = false) {
        ref List<Force> list = ref externalForces;
        if(constant)
            list = ref constantForces;

        if(HasForce(name, constant))
            return list.Find(force => force.Name == name);
        else
            return null;
            
    }

    // Called to remove a force (using only name)
    ///<summary>Remove a force by its name</summary>
    ///<param name="name">Name of the force</param>
    ///<param name="constant">Is it a constant force?</param>
    ///<param name="resetGrav">Reset gravity?</param>
    public void RemoveForce(string name, bool constant = false, bool resetGrav = false) {
        ref List<Force> list = ref externalForces;
        if(constant)
            list = ref constantForces;

        list.RemoveAll(force => force.Name == name);
        if(resetGrav) resetGrav = true;

    }

        #endregion

        #region Slow Related

    //Called to add a slowness to an axis
    ///<summary>Add a slowness</summary>
    ///<param name="slow">The slowness to apply</param>
    ///<param name="replaceSlow">Replace the slowness?</param>
    public void AddSlow(Slow slow, bool replaceSlow = false) {
        if(HasSlow(slow.Name))
            if(replaceSlow)
                slowness[slowness.FindIndex(s2 => s2.Name == slow.Name)] = slow;
            else
                slowness.Find(s2 => s2.Name == slow.Name).Value += slow.Value;
        else
            slowness.Add(slow);
        
        newSlow = true;
    }

    // Check if exists a slow by its name
    ///<summary>Check is exists a slow with this name</summary>
    ///<param name="name">Name of the slow</param>
    public bool HasSlow(string name) {
        return slowness.Exists(s => s.Name == name);
    }

    // Remove a slow by its name
    ///<summary>Remove a slow by its name</summary>
    ///<param name="name">Name of the slow</param>
    public void RemoveSlow(string name) {
        slowness.RemoveAll(s => s.Name == name);
        newSlow = true;
    }

        #endregion

        #endregion

    // Called before drawing the screen, all the physics is called here
    void FixedUpdate() {

        //Check if everything is alright
        CheckColliderChange();

        //Do all the calculation
        CheckIsGroundedOrCelling();
        CheckNearWall();
        CalculateSlowness();
        CalculateExternalSpeed();
        CalculateConstantSpeed();

        rb.velocity = finalSpeed;
        if(resetGrav) resetGrav = false;
        
    }

        #region Checkers and Calculators

    void CheckColliderChange() {
        Vector2 colliderSize = col.size;
        Vector2 colliderOffset = col.offset;

        // Verify if has changed values to the collider
        if(_lastColliderOffset != colliderOffset || _lastColliderSiz != colliderSize) {

            // Ground and celling collider
            _grdColliderPosition = new Vector2(0, -(colliderSize.y / 2)+colliderOffset.y);
            _celColliderPosition = new Vector2(0, (colliderSize.y / 2)+colliderOffset.y);
            _ckcGCColliderSize = new Vector2(colliderSize.x * 0.98f, 0.1f);

            // Wall collider
            _walRColliderPosition = new Vector2((colliderSize.x / 2)+colliderOffset.x, colliderOffset.y);
            _walLColliderPosition = new Vector2(-(colliderSize.x / 2)+colliderOffset.x, colliderOffset.y);
            _ckcWColliderSize = new Vector2(0.1f, colliderSize.y * 0.98f);

            _lastColliderOffset = colliderOffset;
            _lastColliderSiz = colliderSize;
        }
    }

    // Simple void to calculate if the player is on ground
    // NEED to be called before calculating velocity and checking wall
    void CheckIsGroundedOrCelling() {

        Vector2 grdPos = _grdColliderPosition + (Vector2)transform.position;
        Vector2 celPos = _celColliderPosition + (Vector2)transform.position;

        //Check if is grounded or touching celling
        bool ground = false;
        bool celling = false;

        foreach(Collider2D c in Physics2D.OverlapBoxAll(grdPos, _ckcGCColliderSize, 0, collisionLayer))
        {
            if(c.gameObject == this.gameObject) continue;
            ground = true;
            break;
        }

        foreach(Collider2D c in Physics2D.OverlapBoxAll(celPos, _ckcGCColliderSize, 0, collisionLayer))
        {
            if(c.gameObject == this.gameObject) continue;
            celling = true;
            break;
        }

        if(onGround != ground)
        {
            onGround = ground;
            if(ground)
            {
                if(onTouchGround != null)
                    onTouchGround();
            }
            else
            {
                if(onUntouchGround != null)
                    onUntouchGround();
            }
        }

        if(onCelling != celling)
        {
            onCelling = celling;
            if(celling)
            {
                if(onTouchCelling != null)
                    onTouchCelling();
            }
            else
            {
                if(onUntouchCelling != null)
                    onUntouchCelling();
            }
        }

    }

    // Void to check if is near a wall
    // Also apply slowness
    void CheckNearWall() {

        Vector2 rWallPos = _walRColliderPosition + (Vector2)transform.position;
        Vector2 lWallPos = _walLColliderPosition + (Vector2)transform.position;

        // Checking if has a wall nearby
        bool rightwall = false;
        bool leftwall = false;
        bool wall = onLeftWall || onRightWall;

        foreach(Collider2D c in Physics2D.OverlapBoxAll(lWallPos, _ckcWColliderSize, 0, collisionLayer))
        {
            if(c.gameObject == this.gameObject) continue;
            leftwall = true;
            break;
        }

        foreach(Collider2D c in Physics2D.OverlapBoxAll(rWallPos, _ckcWColliderSize, 0, collisionLayer))
        {
            if(c.gameObject == this.gameObject) continue;
            rightwall = true;
            break;
        }

        if(onWall != wall || rightwall != onRightWall || leftwall != onLeftWall)
        {

            int wallSide = 0;
            wallSide += onRightWall != rightwall ? 1 : 0;
            wallSide -= onLeftWall != leftwall ? 1 : 0;

            bool touch = false;
            touch = rightwall && onRightWall != rightwall || leftwall && onLeftWall != leftwall;

            onWall = wall;
            onRightWall = rightwall;
            onLeftWall = leftwall;

            if(wallSide != 0) 
            {
                if(touch)
                {
                    onTouchWall?.Invoke(wallSide);
                }
                else
                {
                    onUntouchWall?.Invoke(wallSide);
                }
            }

        }

    }

    void CalculateSlowness() {

        if(!newSlow) return;
        newSlow = false;

        totalSlowness = new Dictionary<SlowType, Vector2>() { // All slowness to be applied
            { SlowType.Input, Vector2.one },
            { SlowType.Gravity, Vector2.one },
            { SlowType.External, Vector2.one },
            { SlowType.Constant, Vector2.one }
        };

        if(slowness.Count == 0 || slowness == null) return;

        // Remove all slowness with no value
        slowness.RemoveAll(s => s.Value == Vector2.zero);

        // Apply all values
        slowness.ForEach(s => {

            s.Types.ForEach(t => {

                totalSlowness[t] = new Vector2(
                    Mathf.Clamp(totalSlowness[t].x + s.Value.x, 0, Mathf.Infinity),
                    Mathf.Clamp(totalSlowness[t].y + s.Value.y, 0, Mathf.Infinity)
                );

            });

        });

    }

    // Calculates all the external speed
    void CalculateExternalSpeed() {

        // Resets the external speed
        externalSpeed = Vector2.zero;

        // Checks if the list is null or empty
        if(externalForces == null || externalForces.Count == 0) return;

        // Calculates speed
        externalForces.ForEach(f => {

            // If is not the input force
            if(f.Name != "input")
            {
                    #region Other external forces
                        
                // Smooth slow down if has timer
                if(f.TimeToStop != 0 && f.Timer <= f.TimeToStop)
                {
                    float p = 1 - (f.Timer / f.TimeToStop);
                    f.ActualForce = f.ForceApplied * p * 2/f.TimeToStop;
                    f.Timer += Time.fixedDeltaTime;
                }

                // Apply gravity (Y)
                if(f.ActualForce.y > 0 && f.ApplyGravity && !resetGrav) {
                    f.Gravity += ((Vector2)Physics.gravity * gravityScale) * Time.fixedDeltaTime;
                }
                else if(resetGrav) {
                    f.Gravity = Vector2.zero;
                }

                    #endregion
            }
            else
            {
                    #region Input external force

                if(bypassInput) 
                {
                    f.ActualForce = Vector2.zero;
                    return;
                }

                Vector2 newSpeed = f.ActualForce;

                // Calculates the new force with friction
                float xDir = inputAxis.x > 0 ? fric : -fric;
                float yDir = inputAxis.y > 0 ? fric : -fric;

                float xSub = newSpeed.x > 0 ? -fric : fric;
                float ySub = newSpeed.y > 0 ? -fric : fric;

                // x velocity
                if(inputAxis.x != 0)
                    newSpeed.x = Mathf.Clamp(
                        newSpeed.x + xDir,
                        -maxSpeed.x,
                        maxSpeed.x
                    );
                else if(newSpeed.x != 0)
                    newSpeed.x = Mathf.Clamp(
                        newSpeed.x + xSub, 
                        xSub > 0 ? -maxSpeed.x : 0,
                        xSub > 0 ? 0 : maxSpeed.x
                    );

                // y velocity
                if(inputAxis.y != 0)
                    newSpeed.y = Mathf.Clamp(
                        newSpeed.y + yDir,
                        -maxSpeed.y,
                        maxSpeed.y
                    );
                else if(newSpeed.y != 0)
                    newSpeed.y = Mathf.Clamp(
                        newSpeed.y + ySub, 
                        ySub > 0 ? -maxSpeed.y : 0,
                        ySub > 0 ? 0 : maxSpeed.y
                    );

                f.ActualForce = newSpeed;

                    #endregion
            }

            // Check is has collided
            if((f.ActualForce.y < 0 && f.applied && onGround) || (f.ActualForce.y > 0 && f.applied && onCelling)) {
                f.ActualForce.y = 0;
                f.Gravity.y = 0;
            }

            if((f.ActualForce.x > 0 && onRightWall) || (f.ActualForce.x < 0 && onLeftWall)) {
                f.ActualForce.x = 0;
                f.Gravity.x = 0;
            }

        });

        externalForces.RemoveAll(f => (f.ActualForce == Vector2.zero && f.Name != "input"));
        externalForces.RemoveAll(f => (f.TimeToStop != 0 && f.Timer >= f.TimeToStop));

        externalForces.ForEach(f => {

            // Apply to the final external force
            Vector2 finalForce = f.ActualForce + f.Gravity * totalSlowness[SlowType.Gravity];

            if(f.Name != "input")
                finalForce *= totalSlowness[SlowType.External];
            else
                finalForce *= totalSlowness[SlowType.Input];
            
            if(!f.applied) f.applied = true;
            externalSpeed += finalForce;


        });

    }

    ///<summary>Calculates all the constant speed of the motor</summary>
    void CalculateConstantSpeed() {

        // Resets the original value of the constant speed
        constantExternalSpeed = Vector2.zero;

        // Checks if the list is null or is empty
        if(constantForces == null || constantForces.Count == 0) return;

        // Removes all zero vector forces
        constantForces.RemoveAll(f => (f.ActualForce == Vector2.zero && f.Name != "grav"));

        // Calculates all constant speeds
        constantForces.ForEach(f => {

            if(f.Name == "grav")
            {
                if(resetGrav) {
                    f.Gravity = Vector2.zero;
                }
                else
                {
                    if(!onGround)
                        f.Gravity += (Vector2)Physics.gravity * Time.fixedDeltaTime * gravityScale;
                    else
                        f.Gravity = Vector2.zero;
                }
            }

            //Apply final speed
            Vector2 finalForce = f.FinalForce;

            if(f.Name != "grav")
                finalForce *= totalSlowness[SlowType.Constant];
            else
                finalForce *= totalSlowness[SlowType.Gravity];

            
            constantExternalSpeed += finalForce;

        });

    }

        #endregion

    // Called to draw gizmos on the editor
    public void OnDrawGizmos() {

        Vector3 grdPos = (Vector3)_grdColliderPosition + transform.position;
        Vector3 celPos = (Vector3)_celColliderPosition + transform.position;

        Vector3 wallLPos = (Vector3)_walLColliderPosition + transform.position;
        Vector3 wallRPos = (Vector3)_walRColliderPosition + transform.position;

        Gizmos.color = onGround ? Color.green : Color.grey;
        Gizmos.DrawCube(grdPos, _ckcGCColliderSize);

        Gizmos.color = onCelling ? Color.green : Color.grey;
        Gizmos.DrawCube(celPos, _ckcGCColliderSize);

        Gizmos.color = onLeftWall ? Color.green : Color.grey;
        Gizmos.DrawCube(wallLPos, _ckcWColliderSize);

        Gizmos.color = onRightWall ? Color.green : Color.grey;
        Gizmos.DrawCube(wallRPos, _ckcWColliderSize);

    }

}

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
    [SerializeField] private float gravityScale = 7f;
    [SerializeField] private CollisionStopBehaviour gravityStopBehaviour = CollisionStopBehaviour.OpositeY;

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
    

        #region References

    private Rigidbody2D rb {get { return GetComponent<Rigidbody2D>(); } }
    private BoxCollider2D col {get {return GetComponent<BoxCollider2D>(); } }

        #endregion

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
    [SerializeField] private Dictionary<SlowType, Vector2> totalSlowness = new Dictionary<SlowType, Vector2>() { // All slowness to be applied
        { SlowType.Input, Vector2.one },
        { SlowType.Gravity, Vector2.one },
        { SlowType.External, Vector2.one },
        { SlowType.Constant, Vector2.one }
    };

    private float gravity;                                                    // This motor gravity
    private float gravityMultiplier;                                          // Multiplier for the gravity
    private bool newSlow = false;                                             // Has a new slow to apply?
    private bool resetGrav = false;                                           // Should reset gravity?

    private Vector2 externalSpeed;                                                          // Calculate in CalculateExternalSpeed()
    private Vector2 constantExternalSpeed;                                                  // Calculate in CalculateConstantSpeed()
    public Vector2 finalSpeed { get { return  externalSpeed + constantExternalSpeed; } }    // Final velocity
    public float GravityMultiplier { get { return gravityMultiplier; } set { gravityMultiplier = value; } }


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
    void Awake() {
        lastFaceDir = 1;
        gravityScale = !bypassGravity ? gravityScale : 0;
        gravityMultiplier = 1.0f;

        Force inputForce = new Force("input", Vector2.zero, 0, CollisionStopBehaviour.OpositeX);
        inputForce.applied = true;

        Force gravityForce = new Force("grav", Vector2.zero, 0, gravityStopBehaviour);
        gravityForce.applied = true;

        rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        AddForce(inputForce, false);
        AddForce(gravityForce, true);
    }

    
    // All the physics is called here
    void FixedUpdate() {

        //Check if everything is alright
        CheckColliderChange();

        //Reset grav
        if(resetGrav) {
            GetForce("grav", true).ActualForce = Vector2.zero;
            resetGrav = false;
        }

        //Check collisions
        CheckIsGroundedOrCelling();
        CheckNearWall();

        //Do all the calculation
        CalculateSlowness();
        CalculateExternalSpeed();
        CalculateConstantSpeed();

        // Apply rigidbody velocity
        rb.velocity = finalSpeed;
        
    }

        #region Collision Methods

    // void OnCollisionEnter2D(Collision2D collision) {

    //     CheckIsGroundedOrCelling();
    //     CheckNearWall();

    // }

    // void OnCollisionExit2D(Collision2D collision) {

    //     CheckIsGroundedOrCelling();
    //     CheckNearWall();

    // }

        #endregion

        #region Public Methods

    // Called to apply a input
    ///<summary>Called to move the motor using the fric and maxspeed values</summary>
    ///<param name="input">The direction to move to</param>
    public void ReceiveInput(Vector2 input) {

        inputAxis = input;

        onReceiveInput?.Invoke(input.normalized);

        if(input.normalized.x != 0)
            lastFaceDir = (int)input.normalized.x;

    }


    public void SetNewMaxSpeed(Vector2 newMaxSpeed) {
        maxSpeed = newMaxSpeed;

    }

        #region Gravity Related

    ///<summary>Resets tha gravity</summary>
    public void ResetGravity() {
        resetGrav = true;
    }

        #endregion

        #region Force Related

    public void ClearAllForces(bool constant = false) 
    {
        ref List<Force> list = ref externalForces;
        if(constant)
            list = ref constantForces;
        
        list.ForEach(f => {f.ActualForce = Vector2.zero;});
    }

    // Called to apply a force
    ///<summary>Adds a new force to the motor.</summary>
    ///<param name="force">The force to apply</param>
    ///<param name="constant">Is it constant</param>
    ///<param name="resetGrav">Reset gravity?</param>
    ///<param name="replaceForce">Replace force?</param>
    public void AddForce(Force force, bool constant = false, bool resetGvt = false, bool replaceForce = false, bool resetTimer = false) {
        
        ref List<Force> list = ref externalForces;
        if(constant)
            list = ref constantForces;

        if(HasForce(force.Name, constant))
        {
            int index = list.FindIndex(f2 => f2.Name == force.Name);

            list[index].applied = false;

            if(replaceForce) {
                list[index].ActualForce = force.ActualForce;
                list[index].ForceApplied = force.ForceApplied;
                list[index].TimeToStop = force.TimeToStop;
            }
            else
            {
                list[index].ActualForce += force.ForceApplied;
                list[index].ForceApplied += force.ForceApplied;
            }
            if(resetTimer)
            {
                list[index].Timer = 0f;
            }
        }
        else
        {
            list.Add(force);
        }

        if(resetGvt)
        {
            resetGrav = true;
        }
    }

    // Check if a force exists by its name
    ///<summary>Check if has a force with the name</string>
    ///<param name="name">Name of the force</param>
    ///<param name="constant">Is it a constant force?</param>
    public bool HasForce(string name, bool constant = false) {
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

        if(!constant) {
            list.FindAll(force => force.Name == name).ForEach(force => {
                RemoveFromGravity(force.ForceApplied);
            });
        }

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

            if(s.Types.HasFlag(SlowType.Input))
                totalSlowness[SlowType.Input] = totalSlowness[SlowType.Input] + s.Value;

            if(s.Types.HasFlag(SlowType.Gravity))
                totalSlowness[SlowType.Gravity] = totalSlowness[SlowType.Gravity] + s.Value;

            if(s.Types.HasFlag(SlowType.External))
                totalSlowness[SlowType.External] = totalSlowness[SlowType.External] + s.Value;

            if(s.Types.HasFlag(SlowType.Constant))
                totalSlowness[SlowType.Constant] = totalSlowness[SlowType.Constant] + s.Value;

        });

    }

    void CalculateForce(ref Force force) {

        // Smooth slow down if has timer
        if(force.TimeToStop != 0 && force.Timer <= force.TimeToStop)
        {
            float p = 1 - (force.Timer / force.TimeToStop);

            // Checks if has reached 0 velocity in any of the axis, if so, don't calculate velocity with time
            Vector2 forceToCalculate = new Vector2(
                force.ActualForce.x == 0 ? 0 : force.ForceApplied.x,
                force.ActualForce.y == 0 ? 0 : force.ForceApplied.y
            );

            force.ActualForce = forceToCalculate * p * 2/force.TimeToStop;
            force.Timer += Time.fixedDeltaTime;
        }

        if(force.disableAllGravity)
        {
            resetGrav = true;
        }


        if(!force.applied) {force.applied = true; return;}

        // Checks if should stop on oposite X
        if(force.stopBehaviour.HasFlag(CollisionStopBehaviour.OpositeX))
        {
            if(force.ActualForce.x > 0 && onRightWall)
                force.ActualForce.x = 0;
            else if(force.ActualForce.x < 0 && onLeftWall)
                force.ActualForce.x = 0;
        }

        // Checks if should stop on oposite Y
        if(force.stopBehaviour.HasFlag(CollisionStopBehaviour.OpositeY))
        {
            if(force.ActualForce.y > 0 && onCelling)
                force.ActualForce.y = 0;
            else if(force.ActualForce.y < 0 && onGround)
                force.ActualForce.y = 0;
        }

        // Checks if should stop on celling
        if(force.stopBehaviour.HasFlag(CollisionStopBehaviour.Celling) && onCelling)
            force.ActualForce.y = 0;

        // Check if should stop on ground
        if(force.stopBehaviour.HasFlag(CollisionStopBehaviour.Ground) && onGround)
            force.ActualForce.y = 0;

        // Check if should stop on right wall
        if(force.stopBehaviour.HasFlag(CollisionStopBehaviour.RightWall) && onRightWall)
            force.ActualForce.x = 0;

        // Check if should stop on left wall
        if(force.stopBehaviour.HasFlag(CollisionStopBehaviour.LeftWall) && onLeftWall)
            force.ActualForce.x = 0;

    }

    bool ShouldRemoveForce(Force force) {

        bool isZeroForce = force.ActualForce == Vector2.zero;
        bool isReservedName = force.Name == "input" || force.Name == "grav";
        bool isTimerEnded = force.Timer >= force.TimeToStop && force.TimeToStop != 0;

        bool shouldRemove = (isZeroForce || isTimerEnded) && !isReservedName && force.applied;

        return shouldRemove;

    }

    void RemoveFromGravity(Vector2 force) {

        Vector2 grav = GetForce("grav", true).ActualForce;

        if(grav.x > 0)
            grav.x = Mathf.Clamp(grav.x - force.x, 0, Mathf.Infinity);
        else if(grav.x < 0)
            grav.x = Mathf.Clamp(grav.x + force.x, -Mathf.Infinity, 0);

        if(grav.y > 0)
            grav.y = Mathf.Clamp(grav.y - force.y, 0, Mathf.Infinity);
        else if(grav.y < 0)
            grav.y = Mathf.Clamp(grav.y + force.y, -Mathf.Infinity, 0);

        GetForce("grav", true).ActualForce = grav;

    }

    // Calculates all the external speed
    void CalculateExternalSpeed() {

        // Resets the external speed
        externalSpeed = Vector2.zero;

        // Checks if the list is null or empty
        if(externalForces == null || externalForces.Count == 0) return;

        // Remove forces
        externalForces.RemoveAll(force => {

            bool remove = ShouldRemoveForce(force);

            if(remove)
                RemoveFromGravity(force.ForceApplied);

            return remove;

        });

        // Calculates forces
        externalForces.ForEach(f => {

                #region Input behaviour
            // If is the input force
            if(f.Name == "input")
            {

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

            }
                #endregion

            CalculateForce(ref f);

        });

        // Apply all forces
        externalForces.ForEach(f => {

            // Apply to the final external force
            Vector2 finalForce = f.ActualForce;

            if(f.Name != "input")
                finalForce *= totalSlowness[SlowType.External];
            else
                finalForce *= totalSlowness[SlowType.Input];
    
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
        constantForces.RemoveAll(force => ShouldRemoveForce(force));

        // Calculate forces
        constantForces.ForEach(f => {

                #region Gravity calculation
            if(f.Name == "grav")
            {
                if(!onGround) {
                    f.ActualForce += (Vector2)Physics.gravity * Time.fixedDeltaTime * gravityScale * gravityMultiplier * totalSlowness[SlowType.Gravity];
                }
            }
                #endregion

            CalculateForce(ref f);

        });

        // Apply all forces
        constantForces.ForEach(f => {

            //Apply final speed
            Vector2 finalForce = f.ActualForce;

            if(f.Name != "grav")
                finalForce *= totalSlowness[SlowType.Constant];

            
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

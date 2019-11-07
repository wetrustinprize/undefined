using UnityEngine;
using System;
using System.Collections.Generic;
using Undefined.Motor;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class Motor : MonoBehaviour
{
    
        #region Variables

    [Header("Bypass:")]
    public bool BypassInput;
    public bool BypassGravity;

    [Header("Velocity")]
    [SerializeField]
    public Vector2 MaxSpeed = Vector2.one;
    [SerializeField]
    private float Fric = 0.65f;
    [SerializeField]
    private float GravityScale = 3f;

    [Space]

    [SerializeField]
    private LayerMask CollisionLayer = 0;
    public bool OnGround;
    public bool OnWall;
    public bool OnCelling;
    public bool onRightWall;
    public bool onLeftWall;

    [Header("Other")]
    public float lastFaceDir;
    public Vector2 inputAxis; // Input received by the player

    //Script side

    private Rigidbody2D rb {get { return GetComponent<Rigidbody2D>(); } }
    private BoxCollider2D col {get {return GetComponent<BoxCollider2D>(); } }

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
    
    public List<Force> externalForces = new List<Force>(); // List of external forces
    public List<Force> constantForces = new List<Force>(); // List of constant external forces
    public List<Slow> slowness = new List<Slow>(); // List of all slowness
    private bool newSlow = false;
    private bool resetGrav = false;
    public Dictionary<SlowType, Vector2> totalSlowness = new Dictionary<SlowType, Vector2>() { // All slowness to be applied
        { SlowType.Input, Vector2.one },
        { SlowType.Gravity, Vector2.one },
        { SlowType.External, Vector2.one },
        { SlowType.Constant, Vector2.one }
    };
    private Vector2 externalSpeed; // Calculate in CalculateExternalSpeed()
    private Vector2 constantExternalSpeed; // Calculate in CalculateConstantSpeed()
    public Vector2 finalSpeed { get { return  externalSpeed + constantExternalSpeed; } } // Final velocity

        #endregion

        #region Colliders

    // Save all information about the colliders
    private Vector2 _celColliderPosition; // Celling collider position
    private Vector2 _grdColliderPosition; // Ground collider position
    private Vector2 _walRColliderPosition; // Right wall collider position
    private Vector2 _walLColliderPosition; // Left wall collider position
    private Vector2 _ckcGCColliderSize; // Celling and Floor collider size
    private Vector2 _ckcWColliderSize; // Wall collider size

    //Check if the collider has changed size or not
    private Vector2 _lastColliderSiz = Vector2.zero;
    private Vector2 _lastColliderOffset = Vector2.zero;

        #endregion

        #endregion

    // Initial setup
    void Start() {
        lastFaceDir = 1;
        GravityScale = !BypassGravity ? GravityScale : 0;

        AddForce(new Force("input", Vector2.zero, 0, false), false);
        AddForce(new Force("grav", Vector2.zero, 0, false), true);
    }

    // Called to apply a input
    ///<summary>Called to move the motor using the fric and maxspeed values</summary>
    ///<param name="input">The direction to move to</param>
    public void ReceiveInput(Vector2 input) {

        inputAxis = input;

        if(onReceiveInput != null)
            onReceiveInput(input.normalized);

        if(input.normalized.x != 0)
            lastFaceDir = input.normalized.x;

    }

    public void ResetGravity() {
        resetGrav = true;
    }

    // Called to apply a force
    ///<summary>Adds a new force to the motor.</summary>
    ///<param name="force">The force to apply</param>
    ///<param name="constant">Is it constant</param>
    ///<param name="resetGrav">Reset gravity?</param>
    ///<param name="replaceForce">Replace force?</param>
    public void AddForce(Force force, bool constant = false, bool resetGravi = false, bool replaceForce = false) {

        if(constant)
            if(HasForce(force.Name, true))
                if(replaceForce)
                    constantForces[constantForces.FindIndex(f2 => f2.Name == force.Name)] = force;
                else
                    constantForces.Find(f2 => f2.Name == force.Name).ActualForce += force.ActualForce;
            else
                constantForces.Add(force);
        else
            if(HasForce(force.Name, false))
                if(replaceForce)
                    externalForces[externalForces.FindIndex(f2 => f2.Name == force.Name)] = force;
                else
                    externalForces.Find(f2 => f2.Name == force.Name).ActualForce += force.ActualForce;
            else
                externalForces.Add(force);
            
        if(resetGravi) resetGrav = true;

    }

    // Check if a force exists by its name
    ///<summary>Check if has a force with the name</string>
    ///<param name="name">Name of the force</param>
    ///<param name="constant">Is it a constant force?</param>
    public bool HasForce(string name, bool constant) {
        if(constant)
            return constantForces.Exists(force => force.Name == name);
        else
            return externalForces.Exists(force => force.Name == name);

    }

    // Get an existing force
    ///<summary>Gets and existing force</string>
    ///<param name="name">Name of the force</param>
    ///<param name="constant">Is it a constant force?</param>
    public Force GetForce(string name, bool constant = false) {

        if(constant && HasForce(name, constant))
            return constantForces.Find(force => force.Name == name);
        else if(!constant)
            return externalForces.Find(force => force.Name == name);
        else
            return null;
            
    }

    // Called to remove a force (using only name)
    ///<summary>Remove a force by its name</summary>
    ///<param name="name">Name of the force</param>
    ///<param name="constant">Is it a constant force?</param>
    ///<param name="resetGrav">Reset gravity?</param>
    public void RemoveForce(string name, bool constant = false, bool resetGrav = false) {
        if(constant)
            constantForces.RemoveAll(force => force.Name == name);
        else
            externalForces.RemoveAll(force => force.Name == name);

        if(resetGrav) resetGrav = true;

    }

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

        foreach(Collider2D c in Physics2D.OverlapBoxAll(grdPos, _ckcGCColliderSize, 0, CollisionLayer))
        {
            if(c.gameObject == this.gameObject) continue;
            ground = true;
            break;
        }

        foreach(Collider2D c in Physics2D.OverlapBoxAll(celPos, _ckcGCColliderSize, 0, CollisionLayer))
        {
            if(c.gameObject == this.gameObject) continue;
            celling = true;
            break;
        }

        if(OnGround != ground)
        {
            OnGround = ground;
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

        if(OnCelling != celling)
        {
            OnCelling = celling;
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

        foreach(Collider2D c in Physics2D.OverlapBoxAll(lWallPos, _ckcWColliderSize, 0, CollisionLayer))
        {
            if(c.gameObject == this.gameObject) continue;
            leftwall = true;
            break;
        }

        foreach(Collider2D c in Physics2D.OverlapBoxAll(rWallPos, _ckcWColliderSize, 0, CollisionLayer))
        {
            if(c.gameObject == this.gameObject) continue;
            rightwall = true;
            break;
        }

        if(OnWall != wall || rightwall != onRightWall || leftwall != onLeftWall)
        {

            if(onRightWall != rightwall) 
            {
                if(rightwall) 
                {
                    if(onTouchWall != null)
                        onTouchWall(1);
                }
                else
                {
                    if(onUntouchWall != null)
                        onUntouchWall(1);
                }
            }
            
            if(onLeftWall != leftwall) 
            {
                if(leftwall)
                {
                    if(onTouchWall != null)
                        onTouchWall(-1);
                }
                else
                {
                    if(onUntouchWall != null)
                        onUntouchWall(-1);
                }
            }

            OnWall = wall;
            onRightWall = rightwall;
            onLeftWall = leftwall;

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

                totalSlowness[t] += s.Value;

            });

        });

    }

    // Calculates all the external speed
    void CalculateExternalSpeed() {

        externalSpeed = Vector2.zero;
        if(externalForces == null || externalForces.Count == 0) return;

        externalForces.ForEach(f => {


            if(f.Name != "input")
            {
                    #region Other external forces
                        
                // Apply gravity (Y)
                if(f.ActualForce.y > 0 && f.ApplyGravity) {
                    f.ActualForce += ((Vector2)Physics.gravity * GravityScale) * Time.fixedDeltaTime;
                    if(f.ActualForce.y <= 0) f.ActualForce.y = 0;
                }

                if(f.TimeToStop != 0 && f.Timer <= f.TimeToStop)
                {
                    f.Timer += Time.fixedDeltaTime;
                }

                    #endregion
            }
            else
            {
                    #region Input external force

                if(BypassInput) 
                {
                    f.ActualForce = Vector2.zero;
                    return;
                }

                Vector2 newSpeed = f.ActualForce;

                // Calculates the new force with friction
                float xDir = inputAxis.x > 0 ? Fric : -Fric;
                float yDir = inputAxis.y > 0 ? Fric : -Fric;

                float xSub = newSpeed.x > 0 ? -Fric : Fric;
                float ySub = newSpeed.y > 0 ? -Fric : Fric;

                // x velocity
                if(inputAxis.x != 0)
                    newSpeed.x = Mathf.Clamp(
                        newSpeed.x + xDir,
                        -MaxSpeed.x,
                        MaxSpeed.x
                    );
                else if(newSpeed.x != 0)
                    newSpeed.x = Mathf.Clamp(
                        newSpeed.x + xSub, 
                        xSub > 0 ? -MaxSpeed.x : 0,
                        xSub > 0 ? 0 : MaxSpeed.x
                    );

                // y velocity
                if(inputAxis.y != 0)
                    newSpeed.y = Mathf.Clamp(
                        newSpeed.y + yDir,
                        -MaxSpeed.y,
                        MaxSpeed.y
                    );
                else if(newSpeed.y != 0)
                    newSpeed.y = Mathf.Clamp(
                        newSpeed.y + ySub, 
                        ySub > 0 ? -MaxSpeed.y : 0,
                        ySub > 0 ? 0 : MaxSpeed.y
                    );

                f.ActualForce = newSpeed;

                    #endregion
            }

            // Check is has collided
            if(f.ActualForce.y < 0 && f.applied && OnGround) f.ActualForce.y = 0;
            if(f.ActualForce.y > 0 && f.applied && OnCelling) f.ActualForce.y = 0;

            if(f.ActualForce.x > 0 && onRightWall) f.ActualForce.x = 0;
            if(f.ActualForce.x < 0 && onLeftWall) f.ActualForce.x = 0;

        });

        externalForces.RemoveAll(f => (f.ActualForce == Vector2.zero && f.Name != "input"));
        externalForces.RemoveAll(f => (f.TimeToStop != 0 && f.Timer >= f.TimeToStop));

        externalForces.ForEach(f => {

            // Apply to the final external force
            Vector2 finalForce = f.ActualForce;

            if(f.Name != "input")
                finalForce *= totalSlowness[SlowType.External];
            else
                finalForce *= totalSlowness[SlowType.Input];
            
            if(!f.applied) f.applied = true;
            externalSpeed += finalForce;


        });

    }

    public void CalculateConstantSpeed() {

        constantExternalSpeed = Vector2.zero;

        if(constantForces == null || constantForces.Count == 0) return;

        constantForces.RemoveAll(f => (f.ActualForce == Vector2.zero && f.Name != "grav"));

        constantForces.ForEach(f => {

            // Calculate if is gravity force
            if(f.Name == "grav" && resetGrav) {
                f.ActualForce = Vector2.zero;
                resetGrav = false;
            }
            
            if(f.Name == "grav" && !OnGround) {
                f.ActualForce += (Vector2)Physics.gravity * Time.fixedDeltaTime * GravityScale * totalSlowness[SlowType.Gravity];
            } 
            else if(f.Name == "grav" && OnGround) {
                f.ActualForce = Vector2.zero;
            }

            //Apply final speed
            Vector2 finalForce = f.ActualForce;

            if(f.Name != "grav")
                finalForce *= totalSlowness[SlowType.Constant];

            
            constantExternalSpeed += finalForce;

        });

    }

        #endregion

    public void OnDrawGizmos() {

        Vector3 grdPos = (Vector3)_grdColliderPosition + transform.position;
        Vector3 celPos = (Vector3)_celColliderPosition + transform.position;

        Vector3 wallLPos = (Vector3)_walLColliderPosition + transform.position;
        Vector3 wallRPos = (Vector3)_walRColliderPosition + transform.position;

        Gizmos.color = OnGround ? Color.green : Color.grey;
        Gizmos.DrawCube(grdPos, _ckcGCColliderSize);

        Gizmos.color = OnCelling ? Color.green : Color.grey;
        Gizmos.DrawCube(celPos, _ckcGCColliderSize);

        Gizmos.color = onLeftWall ? Color.green : Color.grey;
        Gizmos.DrawCube(wallLPos, _ckcWColliderSize);

        Gizmos.color = onRightWall ? Color.green : Color.grey;
        Gizmos.DrawCube(wallRPos, _ckcWColliderSize);

    }

}

using UnityEngine;
using System.Collections.Generic;
using Untitled.Motor;

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
    private Vector2 MaxSpeed = Vector2.one;
    [SerializeField]
    private float Fric = 0.65f;
    [SerializeField]
    private float DefaultGravityScale = 3f;

    [Space]

    [SerializeField]
    private LayerMask GroundLayer = 0;
    public bool OnGround;
    public bool OnWall;
    public bool OnCelling;
    public bool onRightWall;
    public bool onLeftWall;

    [Header("Other")]
    public float lastFaceDir;

    //Script side

    private Rigidbody2D rb {get { return GetComponent<Rigidbody2D>(); } }
    private BoxCollider2D col {get {return GetComponent<BoxCollider2D>(); } }

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
    private Vector2 finalSpeed { get { return  externalSpeed + constantExternalSpeed; } } // Final velocity
    private Vector2 inputAxis; // Input received by the player

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
        rb.gravityScale = !BypassGravity ? DefaultGravityScale : 0;

        AddForce(new Force("input", Vector2.zero, 0, false), false);
        AddForce(new Force("grav", Vector2.zero, 0, false), true);
    }

    // Called to apply a input
    ///<summary>Called to move the motor using the fric and maxspeed values</summary>
    ///<param name="input">The direction to move to</param>
    public void ReceiveInput(Vector2 input) {

        inputAxis = input;

    }

    // Called to apply a force
    ///<summary>Adds a new force to the motor, if already exists, adds the force to the existing</summary>
    ///<param name="f">The force to apply</param>
    ///<param name="c">Is it constant</param>
    ///<param name="rg">Reset gravity?</param>
    ///<param name="rp">Replace force?</param>
    public void AddForce(Force f, bool c, bool rg = false, bool rp = false) {

        if(c)
            if(HasForce(f.Name, true))
                if(rp)
                    constantForces[constantForces.FindIndex(f2 => f2.Name == f.Name)] = f;
                else
                    constantForces.Find(f2 => f2.Name == f.Name).ActualForce += f.ActualForce;
            else
                constantForces.Add(f);
        else
            if(HasForce(f.Name, false))
                if(rp)
                    externalForces[externalForces.FindIndex(f2 => f2.Name == f.Name)] = f;
                else
                    externalForces.Find(f2 => f2.Name == f.Name).ActualForce += f.ActualForce;
            else
                externalForces.Add(f);
            
        if(rg) resetGrav = true;

    }

    // Check if a force exists by its name
    ///<summary>Check if has a force with the name</string>
    ///<param name="s">Name of the force</param>
    ///<param name="c">Is it a constant force?</param>
    public bool HasForce(string n, bool c) {
        if(c)
            return constantForces.Exists(force => force.Name == n);
        else
            return externalForces.Exists(force => force.Name == n);

    }

    // Called to remove a force (using only name)
    ///<summary>Remove a force by its name</summary>
    ///<param name="n">Name of the force</param>
    ///<param name="c">Is it a constant force?</param>
    ///<param name="rg">Reset gravity?</param>
    public void RemoveForce(string n, bool c, bool rg) {
        if(c)
            constantForces.RemoveAll(force => force.Name == n);
        else
            externalForces.RemoveAll(force => force.Name == n);

        if(rg) resetGrav = true;

    }

    //Called to add a slowness to an axis
    ///<summary>Add a slowness</summary>
    ///<param name="s">The slowness to apply</param>
    ///<param name="rp">Replace the slowness?</param>
    public void AddSlow(Slow s, bool rp = false) {
        if(HasSlow(s.Name))
            if(rp)
                slowness[slowness.FindIndex(s2 => s2.Name == s.Name)] = s;
            else
                slowness.Find(s2 => s2.Name == s.Name).Value += s.Value;
        else
            slowness.Add(s);
        
        newSlow = true;
    }

    // Check if exists a slow by its name
    ///<summary>Check is exists a slow with this name</summary>
    ///<param name="n">Name of the slow</param>
    public bool HasSlow(string n) {
        return slowness.Exists(s => s.Name == n);
    }

    // Remove a slow by its name
    ///<summary>Remove a slow by its name</summary>
    ///<param name="n">Name of the slow</param>
    public void RemoveSlow(string n) {
        slowness.RemoveAll(s => s.Name == n);
        newSlow = true;
    }

    // Called before drawing the screen, all the physics is called here
    void FixedUpdate() {

        //Do all the calculation
        CheckIsGroundedOrCelling();
        CheckNearWall();
        CalculateSlowness();
        CalculateExternalSpeed();
        CalculateConstantSpeed();

        rb.velocity = finalSpeed;
        
    }

        #region Checkers and Calculators

    // Simple void to calculate if the player is on ground
    // Also calculates the extra frames to jump
    // NEED to be called before calculating velocity and checking wall
    void CheckIsGroundedOrCelling() {
        Vector2 colliderSize = col.size;
        Vector2 colliderOffset = col.offset;

        // Verify if has changed values to the collider
        if(_lastColliderOffset != colliderOffset || _lastColliderSiz != colliderSize) {
            _grdColliderPosition = (Vector2)transform.position + new Vector2(0, -(colliderSize.y / 2)+colliderOffset.y);
            _celColliderPosition = (Vector2)transform.position + new Vector2(0, (colliderSize.y / 2)+colliderOffset.y);
            _ckcGCColliderSize = new Vector2(colliderSize.x * 0.98f, 0.1f);
        }

        //Check if is grounded or touching celling
        OnGround = Physics2D.OverlapBox(_grdColliderPosition, _ckcGCColliderSize, 0, GroundLayer);
        OnCelling = Physics2D.OverlapBox(_celColliderPosition, _ckcGCColliderSize, 0, GroundLayer);
    }

    // Void to check if is near a wall
    // Also apply slowness
    void CheckNearWall() {
        Vector2 colliderSize = col.size;
        Vector2 colliderOffset = col.offset;

        // Checking if has a wall nearby
        _walRColliderPosition = (Vector2)transform.position + new Vector2((colliderSize.x / 2)+colliderOffset.x, colliderOffset.y);
        _walLColliderPosition = (Vector2)transform.position + new Vector2(-(colliderSize.x / 2)+colliderOffset.x, colliderOffset.y);
        _ckcWColliderSize = new Vector2(0.1f, colliderSize.y * 0.98f);

        onRightWall = Physics2D.OverlapBox(_walRColliderPosition, _ckcWColliderSize, 0, GroundLayer);
        onLeftWall = Physics2D.OverlapBox(_walLColliderPosition, _ckcWColliderSize, 0, GroundLayer);
        OnWall = onLeftWall || onRightWall;

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
                    f.ActualForce += ((Vector2)Physics.gravity * rb.gravityScale) * Time.fixedDeltaTime;
                    if(f.ActualForce.y <= 0) f.ActualForce.y = 0;
                }

                // Apply slowdown X if is on ground or if has timer
                if(f.ActualForce.x != 0 && OnGround && f.ApplyGravity || f.ActualForce.x != 0 && f.TimeToStop != 0) {
                    f.ActualForce.x -= f.ActualForce.x * (Time.fixedDeltaTime / (f.TimeToStop != 0 ? f.TimeToStop : 0.3f));

                    if(f.ForceApplied.normalized.x < 0 ? f.ActualForce.x >= 0 : f.ActualForce.x <= 0) f.ActualForce.x = 0;
                }

                // Apply slowdown Y if is on air of if has timer
                if(f.ActualForce.y != 0 && f.TimeToStop != 0) {
                    f.ActualForce.y -= f.ActualForce.y * (Time.fixedDeltaTime / f.TimeToStop);

                    if(f.ForceApplied.normalized.y < 0 ? f.ActualForce.y >= 0 : f.ActualForce.y <= 0) f.ActualForce.y = 0;
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

            // Remove if has no values
            if(f.ActualForce == Vector2.zero && f.Name != "input")
            {
                externalForces.Remove(f);
                return;
            }

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

        constantForces.ForEach(f => {
            
            // Remove forces with no value
            if(f.ActualForce == Vector2.zero && f.Name != "grav")
            {
                constantForces.Remove(f);
                return;
            }

            // Calculate if is gravity force
            if(f.Name == "grav" && resetGrav) {
                f.ActualForce = Vector2.zero;
                resetGrav = false;
            }
            

            if(f.Name == "grav" && !OnGround) {
                f.ActualForce += (Vector2)Physics.gravity * Time.fixedDeltaTime * rb.gravityScale;
            } 
            else if(f.Name == "grav" && OnGround) {
                f.ActualForce = Vector2.zero;
            }

            //Apply final speed
            Vector2 finalForce = f.ActualForce;

            if(f.Name == "grav")
                finalForce *= totalSlowness[SlowType.Gravity];
            else
                finalForce *= totalSlowness[SlowType.Constant];

            constantExternalSpeed += finalForce;

        });

    }

        #endregion

    public void OnDrawGizmos() {

        Gizmos.DrawCube((Vector3)_grdColliderPosition, (Vector2)_ckcGCColliderSize);
        Gizmos.DrawCube((Vector3)_celColliderPosition, (Vector2)_ckcGCColliderSize);

        Gizmos.DrawCube((Vector3)_walRColliderPosition, (Vector2)_ckcWColliderSize);
        Gizmos.DrawCube((Vector3)_walLColliderPosition, (Vector2)_ckcWColliderSize);

    }

}

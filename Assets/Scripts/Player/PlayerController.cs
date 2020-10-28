using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Motor))]
[RequireComponent(typeof(Jump))]
[RequireComponent(typeof(Dash))]
[RequireComponent(typeof(Attack))]
[RequireComponent(typeof(Alive))]

[RequireComponent(typeof(PlayerJump))]
[RequireComponent(typeof(PlayerTeleport))]
[RequireComponent(typeof(PlayerExplosion))]
public class PlayerController : MonoBehaviour {

        #region Variables

    [Header("Input")]
    public bool receiveInput = true;
    public bool canInteract = true;

    [Header("Movement")]
    public bool canMove = true;
    public bool canJump = true;

    [Header("Attack")]
    public bool canAttack = true;

    [Header("Skills")]
    public bool canSkills = true;
    public bool canDash = true;
    public bool canTeleport = true;
    public bool canExplode = true;
    public bool canWallJump { get { return jump.allowWallJumps; } set { jump.allowWallJumps = value; } }

    [Space(10)]
    [SerializeField] private float explosionMinTimer = 0.35f;

    [Header("Inventory")]
    public bool canUseActiveItem = true;

    // Script side variables

    // Components
    public Motor motor { get; private set; }
    public Jump jump { get; private set; }
    public PlayerJump pjump { get; private set; }
    public Dash dash { get; private set; }
    public Attack attack { get; private set; }
    public PlayerTeleport teleport { get; private set; }
    public PlayerExplosion explosion { get; private set; }
    public PlayerInventory inventory { get; private set; }
    public PlayerInteraction interaction { get; private set; }
    public Alive alive { get; private set; }

    // Script side
    private PlayerInput inputs;

    private float totalAttackSeconds;
    private bool holdingAttack;

    public Vector2 lastMousePosition { get; private set; }

        #endregion

    void Awake() {

        // Gets all components
        this.motor = GetComponent<Motor>();
        this.jump = GetComponent<Jump>();
        this.pjump = GetComponent<PlayerJump>();
        this.dash = GetComponent<Dash>();
        this.attack = GetComponent<Attack>();
        this.teleport = GetComponent<PlayerTeleport>();
        this.explosion = GetComponent<PlayerExplosion>();
        this.inventory = GetComponent<PlayerInventory>();
        this.interaction = GetComponent<PlayerInteraction>();
        this.alive = GetComponent<Alive>();

        inputs = new PlayerInput();

    }

    void OnEnable() {
        inputs.Enable();
    }

    void OnDisable() {
        inputs.Disable();
    }

    void Update() {
        if(holdingAttack) HoldingAttackTick();
        if(Input.GetKeyDown(KeyCode.K)) { alive.TakeDamage(100, this.gameObject); }
    }

    void Die() {
        GameManager.HUD.HideAllHUD();
        GameManager.HUD.canOpenInventory = false;

        this.receiveInput = false;

        this.alive.CanReceiveDamage = false;
        this.alive.CanReceiveHeal = false;

        this.motor.SetFreeze(true);
    }

    void Start() {

        // Death
        alive.onDie += Die;

        // Movement
        inputs.Player.Move.performed += cb => { 
            WalkPerfomed(cb.ReadValue<Vector2>()); 
        };

        inputs.Player.Move.canceled += cb => { 
            WalkPerfomed(Vector2.zero, true);
        };

        inputs.Player.Jump.performed += cb => { 
            if(canJump && receiveInput) 
            {
                pjump.Execute(); 
            }
        };

        // Attack and explosion
        inputs.Player.Attack.started += cb => { 
            if(canAttack && receiveInput) 
            {
                holdingAttack = true; 
            }
        };

        inputs.Player.Attack.canceled += cb => { 
            AttackCanceled();
        };

        // Skills
            // Dash
        inputs.Player.Dash.performed += cb => { 
            if(canDash && canSkills && receiveInput)
            {
                dash.Execute(); 
            }
        };

            // Teleport
        inputs.Player.Teleport.started += cb => { 
            if(canTeleport && canSkills && receiveInput)
            {
                teleport.StartPerforming(); 
            }
        };

        inputs.Player.Teleport.canceled += cb => { 
            teleport.StopPerforming(); 
        };

        inputs.Player.Jump.performed += cb => { 
            teleport.CancelPerfoming(); 
        };

        // Interaction
        inputs.Player.Interact.performed += cb => {
            if(receiveInput && canInteract) {
                interaction.Interact();
            }
        };

        // Inventory
            // Use active
        inputs.Player.UseActiveItem.performed += cb => {
            if(receiveInput && canUseActiveItem)
            {
                inventory.UseActiveItem();
            }
        };

        // Mouse position
        inputs.Player.MousePosition.performed += cb => {
            lastMousePosition = cb.ReadValue<Vector2>();
        };

    }

        #region Attack

    void HoldingAttackTick() {

        if(!canExplode) AttackCanceled();

        totalAttackSeconds += Time.deltaTime;

        if(totalAttackSeconds >= explosionMinTimer && !explosion.IsShowingVisual)
        {
            explosion.StartExplosionVisual();
        }

    }

    void AttackCanceled() {

        if(!receiveInput) return;
        if(!holdingAttack) return;

        holdingAttack = false;

        if(totalAttackSeconds > explosionMinTimer && canExplode && canSkills)
        {
            explosion.Execute();
            explosion.StopExplosionVisual();
        }
        else if(canAttack)
        {
            explosion.StopExplosionVisual();
            attack.Execute();
        }

        totalAttackSeconds = 0;

    }

        #endregion

        #region Walk

    public void ResetInput() {
        motor.ReceiveInput(Vector2.zero);
    }

    void WalkPerfomed(Vector2 dir, bool force = false) {

        if(!force)
        {
            if(!receiveInput) return;
            if(!canMove) return;
        }

        Vector2 input = dir;
        input.y = 0;

        motor.ReceiveInput(input);

    }

        #endregion

}
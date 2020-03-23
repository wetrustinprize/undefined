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
    public bool canGhost = true;

    [Header("Inventory")]
    public bool canUseActiveItem = true;

    // Script side variables
    private Motor motor { get {return GetComponent<Motor>(); } }
    private Jump jump { get { return GetComponent<Jump>(); } }
    private PlayerJump pjump { get {return GetComponent<PlayerJump>(); } }
    private Dash dash { get { return GetComponent<Dash>(); } }
    private Attack attack { get {return GetComponent<Attack>(); } }
    private PlayerTeleport teleport { get { return GetComponent<PlayerTeleport>(); } }
    private PlayerExplosion explosion { get { return GetComponent<PlayerExplosion>(); } }
    private PlayerInventory inventory { get { return GetComponent<PlayerInventory>(); } }
    private PlayerInteraction interaction { get { return GetComponent<PlayerInteraction>(); } }

    // Script side
    private PlayerInput inputs;

    private float totalAttackSeconds;
    private bool holdingAttack;

        #endregion

    void Awake() {

        DontDestroyOnLoad(this.gameObject);
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
    }

    void Start() {

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

        // Attack
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

        inputs.Player.TeleportPosition.performed += cb => { 
            teleport.UpdateArrow(cb.ReadValue<Vector2>()); 
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

    }

        #region Attack

    void HoldingAttackTick() {

        totalAttackSeconds += Time.deltaTime;

    }

    void AttackStart() {

        

    }

    void AttackCanceled() {

        holdingAttack = false;

        if(canAttack)
        {
            if(totalAttackSeconds < 0.35f) {
                attack.Execute();
            }
            else
            {
                explosion.Execute();
            }
        }

        totalAttackSeconds = 0;

    }

        #endregion

        #region Walk

    void WalkPerfomed(Vector2 dir, bool force = false) {

        if((!canMove || !receiveInput) && !force) return;

        Vector2 input = dir;
        input.y = 0;

        motor.ReceiveInput(input);

    }

        #endregion

}
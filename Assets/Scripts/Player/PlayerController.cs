using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Motor))]
[RequireComponent(typeof(Jump))]
[RequireComponent(typeof(Dash))]
[RequireComponent(typeof(PlayerJump))]
[RequireComponent(typeof(Attack))]
[RequireComponent(typeof(Alive))]
public class PlayerController : MonoBehaviour {

        #region Variables

    [Header("Movement")]
    public bool CanMove = true;
    public bool CanJump = true;

    [Header("Attack")]
    public bool CanAttack = true;

    [Header("Skills")]
    public bool CanSkills = true;
    public bool CanDash = true;
    public bool CanTeleport = true;
    public bool CanGhost = true;

    // Script side variables
    private Motor motor { get {return GetComponent<Motor>(); } }
    private Jump jump { get { return GetComponent<Jump>(); } }
    private PlayerJump pjump { get {return GetComponent<PlayerJump>(); } }
    private Dash dash { get { return GetComponent<Dash>(); } }
    private Attack attack { get {return GetComponent<Attack>(); } }
    private PlayerTeleport teleport { get { return GetComponent<PlayerTeleport>(); } }

    private PlayerInput inputs;

        #endregion

    void Awake() {
        inputs = new PlayerInput();
    }

    void OnEnable() {
        inputs.Enable();
    }

    void OnDisable() {
        inputs.Disable();
    }

    void Start() {

        // Movement
        inputs.Player.Move.performed += cb => { PerformWalk(cb.ReadValue<Vector2>()); };
        inputs.Player.Move.canceled += cb => { PerformWalk(Vector2.zero, true); };
        inputs.Player.Jump.performed += cb => { if(CanJump) pjump.Execute(); };

        // Attack
        inputs.Player.Attack.performed += cb => { if(CanAttack) attack.Execute(); };

        // Skills
            // Dash
        inputs.Player.Dash.performed += cb => { if(CanDash && CanSkills) dash.Execute(); };

            // Teleport
        inputs.Player.Teleport.started += cb => { teleport.StartPerforming(); };
        inputs.Player.Teleport.canceled += cb => { teleport.StopPerforming(); };
        inputs.Player.TeleportPosition.performed += cb => { teleport.UpdateArrow(cb.ReadValue<Vector2>()); };
        inputs.Player.Jump.performed += cb => { teleport.CancelPerfoming(); };
    }

    public void PerformWalk(Vector2 dir, bool force = false) {

        if(!CanMove && !force) return;

        Vector2 input = dir;
        input.y = 0;

        motor.ReceiveInput(input);

    }

}
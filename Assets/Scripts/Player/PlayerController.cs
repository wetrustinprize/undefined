using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
[RequireComponent(typeof(Motor))]
[RequireComponent(typeof(Jump))]
[RequireComponent(typeof(Dash))]
public class PlayerController : MonoBehaviour {

        #region Variables

    public bool CanMove = true;
    public bool CanJump = true;
    public bool CanDash = true;

    // Script side variables
    private Motor motor { get {return GetComponent<Motor>(); } }
    private Jump jump { get { return GetComponent<Jump>(); } }
    private Dash dash { get { return GetComponent<Dash>(); } }
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

        inputs.Player.Move.performed += cb => { PerformWalk(cb.ReadValue<Vector2>()); };
        inputs.Player.Move.canceled += cb => { PerformWalk(Vector2.zero, true); };

        inputs.Player.Jump.performed += cb => { if(CanJump) jump.Execute(); };

        inputs.Player.Dash.performed += cb => { if(CanDash) dash.Execute(); };

    }

    public void PerformWalk(Vector2 dir, bool force = false) {

        if(!CanMove && !force) return;

        Vector2 input = dir;
        input.y = 0;

        motor.ReceiveInput(input);

    }

}
using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
[RequireComponent(typeof(Motor))]
[RequireComponent(typeof(Jump))]
public class PlayerController : MonoBehaviour {

        #region Variables

    public bool CanMove = true;
    public bool CanJump = true;
    public bool CanDash = true;

    // Script side variables
    private Motor m { get {return GetComponent<Motor>(); } }
    private Jump j { get { return GetComponent<Jump>(); } }
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

        inputs.Player.Move.performed += cb => { PerformJump(cb.ReadValue<Vector2>()); };
        inputs.Player.Move.canceled += cb => { PerformJump(Vector2.zero, true); };

        inputs.Player.Jump.performed += cb => { PerformJump(); };

    }

    public void PerformJump(Vector2 dir, bool force = false) {

        if(!CanMove && !force) return;

        Vector2 input = dir;
        input.y = 0;

        m.ReceiveInput(input);

    }

    public void PerformJump() {

        j.Execute();

    }

}
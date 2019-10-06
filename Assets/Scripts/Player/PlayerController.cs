using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
[RequireComponent(typeof(Motor))]
public class PlayerController : MonoBehaviour {

    private Motor m { get {return GetComponent<Motor>(); } }

    public void Move(InputAction.CallbackContext cb) {

        m.ReceiveInput(cb.ReadValue<Vector2>());

    }

}
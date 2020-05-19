using UnityEngine;
using Undefined.Force;

[RequireComponent(typeof(Motor))]
[DisallowMultipleComponent]

public class Dash : MonoBehaviour
{
    
        #region Variables

    [Header("Dash Settings")]
    [SerializeField] private Vector2 dashForce = Vector2.zero;

    [Space]

    [SerializeField] private float dashDuration = 3f;
    [SerializeField] private float dashCooldown = 3f;
    [SerializeField] private bool applyCooldown = true;

    // Script side variables
    Motor m {get {return GetComponent<Motor>();}}
    private float dashTimer = 0f;

    // Public acess variables
    public float CurrentCooldown { get { return dashTimer; } }

        #endregion

    // Update is called once per frame
    void Update() {

        if(dashTimer > 0)
            dashTimer = Mathf.Clamp(dashTimer - Time.deltaTime, 0, dashCooldown);

    }

    public void Execute() {
        
        if(dashTimer > 0 && applyCooldown) return;

        Force f = new Force("dash", dashForce * m.LastFacingDir, dashDuration, CollisionStopBehaviour.AnyCollision, true);
        m.ClearAllForces();
        m.AddForce(f, false, true, true);
        m.RemoveForce("jump");
        dashTimer = dashCooldown;

    }

}

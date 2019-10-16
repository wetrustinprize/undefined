using UnityEngine;
using Untitled.Motor;

[RequireComponent(typeof(Motor))]
[DisallowMultipleComponent]

public class Dash : MonoBehaviour
{
    
        #region Variables

    [Header("Dash Settings")]
    public Vector2 dashForce = Vector2.zero;

    [Space]

    public float dashDuration = 3f;
    public float dashCooldown = 3f;
    public bool applyCooldown = true;

    [Header("Other")]
    public float dashTimer = 0f;

    // Script side variables
    Motor m {get {return GetComponent<Motor>();}}

        #endregion

    // Update is called once per frame

    void Update() {

        if(dashTimer > 0)
            dashTimer = Mathf.Clamp(dashTimer - Time.deltaTime, 0, dashCooldown);

    }

    public void Execute() {
        
        if(dashTimer > 0 && applyCooldown) return;

        Force f = new Force("dash", dashForce * m.lastFaceDir, dashDuration);
        m.AddForce(f, false, true);
        dashTimer = dashCooldown;

    }

    public void Execute(Vector2 dash, float time) {

        if(dashTimer > 0 && applyCooldown) return;

        Force f = new Force("dash", dash * m.lastFaceDir, time);

        m.AddForce(f, false, true);

        dashTimer = dashCooldown;

    }
}

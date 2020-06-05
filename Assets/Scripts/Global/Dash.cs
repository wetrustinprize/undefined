using UnityEngine;
using Undefined.Force;
using System;

[RequireComponent(typeof(Motor))]
[DisallowMultipleComponent]

public class Dash : MonoBehaviour
{
    
        #region Variables

    [Header("Dash Settings")]
    [SerializeField] private ForceTemplate dashForce = null;

    [Space]

    [SerializeField] private float dashCooldown = 3f;
    [SerializeField] private bool applyCooldown = true;

    // Events
    public Action<float> onDash;
    public Action onDashCooldownEnd;

    // Script side variables
    Motor m {get {return GetComponent<Motor>();}}
    private float dashTimer = 0f;

    // Public acess variables
    public float CurrentCooldown { get { return dashTimer; } }
    public bool CanCooldownDash { get { return !(dashTimer > 0);}}

        #endregion

    // Update is called once per frame
    void Update() {

        if(dashTimer > 0)
        {
            dashTimer = Mathf.Clamp(dashTimer - Time.deltaTime, 0, dashCooldown);
            if(dashTimer <= 0)
                onDashCooldownEnd?.Invoke();
        }

    }

    public void Execute() {
        
        if(dashTimer > 0 && applyCooldown) return;

        Force f = new Force(dashForce);

        f.ForceApplied = f.ActualForce = dashForce.ForceToApply * m.LastFacingDir;
        f.Name = "dash";

        m.ClearAllForces();
        m.AddForce(f, false, true, true);
        m.RemoveForce("jump");
        dashTimer = dashCooldown;

        onDash?.Invoke(dashCooldown);

    }

}

using UnityEngine;
using Undefined.Force;
public class PlayerTeleport : MonoBehaviour
{
    
        #region Variables

    [Header("Configuration")]
    [SerializeField] private int minBounces = 2;

    [Header("Knockback")]
    [SerializeField] private ForceTemplate knockForce = null;

    [Header("Cooldown")]
    [SerializeField] private float telCooldown = 0;

    [Header("Graphics")]
    [SerializeField] private GameObject teleportGraphs = null;

    // Script side
    private bool performing;     // Is the teleport being perfomed?

    private float currentCooldown;

    private CreatureController creature;
    private Motor playerMotor;

    // Public acess variables
    public float CurrentCooldown { get { return currentCooldown; } }
    public bool Perfoming { get { return performing; } }


        #endregion

    void Start() {

        // Initial creature bounce config
        creature = GameManager.Creature;

        creature.SetMaxBounces(minBounces + 2);
        creature.SetMinBounces(minBounces);
        
        creature.OnBounceEnd += StartCountdown;

        // Gets the player motor
        playerMotor = GetComponent<Motor>();

        // Disable graphics
        teleportGraphs.SetActive(false);


    }

    void Update() {

        if(!(currentCooldown < 0)) { currentCooldown -= Time.deltaTime; }

    }

    void LateUpdate() {

        if(performing)
        {
            Vector3 dir = GameManager.Camera.GetWorldPositionOnPlane(GameManager.Player.lastMousePosition, 0) - transform.position;
            dir.Normalize();

            float rot_z = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            teleportGraphs.transform.rotation = Quaternion.Euler(0f, 0f, rot_z);
        }

    }

    public void StopPerforming() {

        // Check if is perfoming
        if(!performing) return;
        else performing = false;

        Vector3 dir = GameManager.Camera.GetWorldPositionOnPlane(GameManager.Player.lastMousePosition, 0) - transform.position;
        dir.Normalize();

        RemoveSlows();

        creature.Bounce(dir);

        teleportGraphs.SetActive(false);

        knockForce.Name = "teleportknockback";
        Force f = new Force(knockForce);
        f.ForceApplied = dir * -1 * knockForce.ForceToApply;
        f.ActualForce = dir * -1 * knockForce.ForceToApply;

        playerMotor.AddForce(f);

    }

    public void CancelPerfoming() {

        if(!performing) return;
        else performing = false;

        teleportGraphs.SetActive(false);
        RemoveSlows();
    }

    public void StartPerforming() {

        // Check if isnt bouncing
        if(creature.IsBouncing) { Teleport(); return; }

        // Check if isnt already perfoming
        if(performing || currentCooldown > 0 || creature.IsBusy) return;
        else performing = true;

        ApplySlows();

        teleportGraphs.SetActive(true);

    }

    void ApplySlows() {
        playerMotor.ResetGravity();
        playerMotor.RemoveForce("jump");

        playerMotor.AddSlow(new Slow("teleportslow", Vector2.one * -1, SlowType.Input));
        playerMotor.AddSlow(new Slow("teleportslow_grav", Vector2.one * -0.70f, SlowType.Gravity));
    }

    void RemoveSlows() {
        playerMotor.RemoveSlow("teleportslow");
        playerMotor.RemoveSlow("teleportslow_grav");
    }

    void StartCountdown() {
        currentCooldown = telCooldown;
    }

    void Teleport() {

        if(creature.TotalBounces < minBounces) { return; }

        transform.position = creature.transform.position;
        playerMotor.ResetGravity();
        creature.ChangeState(CreatureState.Following);

    }

}

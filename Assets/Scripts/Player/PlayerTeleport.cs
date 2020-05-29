using UnityEngine;
using Undefined.Force;
public class PlayerTeleport : MonoBehaviour
{
    
        #region Variables

    [Header("Configuration")]
    [SerializeField] private int min_bounces;

    [Header("Knockback")]
    [SerializeField] private float knock_force;

    [Header("Cooldown")]
    [SerializeField] private float tel_cooldown;

    [Header("Graphics")]
    [SerializeField] private GameObject teleportGraphs;

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

        creature.SetMaxBounces(min_bounces + 2);
        creature.SetMinBounces(min_bounces);
        
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
            Debug.Log(dir);
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
        playerMotor.AddForce(new Force("teleportknockback", dir * -1 * knock_force, 0.1f));

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
        currentCooldown = tel_cooldown;
    }

    void Teleport() {

        if(creature.TotalBounces < min_bounces) { return; }

        transform.position = creature.transform.position;
        playerMotor.ResetGravity();
        creature.ChangeState(CreatureState.Following);

    }

}

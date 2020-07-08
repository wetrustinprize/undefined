using UnityEngine;
using Undefined.Force;
using System;

[DisallowMultipleComponent]
public class Attack : MonoBehaviour
{

        #region Variables

    [Header("Raycast Settings")]
    [SerializeField] private Vector2 offset = Vector2.zero;
    [SerializeField] private Vector2 size = Vector2.one;
    [SerializeField] private LayerMask aliveLayes = 0;

    [Header("Attack Settings")]
    public int Damage;
    public float coolDown;

    [Header("Force Settings")]
    public ForceTemplate pushForce;

    [Header("Slow Settings")]
    public Slow attackerSlow;
    public float attackerSlowTimer;

    // Default values
    private int defaultDamage;
    private float defaultCooldown;

    private ForceTemplate defaultForce;

    private Slow defaultAttackerSlow;
    private float defaultAttackerSlowTimer;

    // Events
    public event Action onAttack;
    public event Action onPerform;

    // Other
    public bool canAttack { get { return actCoolDown == 0; } }

    // Script side
    private Motor motor { get {return GetComponent<Motor>(); } }
    private float actCoolDown = 0f;
    private float actAttackSlowTimer = 0f;

        #endregion


    void Start() {
        // Default values
        defaultDamage = Damage;
        defaultCooldown = coolDown;

        defaultAttackerSlow = attackerSlow;
        defaultAttackerSlowTimer = attackerSlowTimer;

        defaultForce = pushForce;

    }

    void Update() {
        attackerSlow.Name = "attacker_slow";

        if(actCoolDown != 0)
            actCoolDown = Mathf.Clamp(actCoolDown - Time.deltaTime, 0, actCoolDown);

        if(actAttackSlowTimer != 0)
        {
            actAttackSlowTimer = Mathf.Clamp(actAttackSlowTimer - Time.deltaTime, 0, actAttackSlowTimer);

            if(actAttackSlowTimer <= 0 && motor.HasSlow("attacker_slow"))
                motor.RemoveSlow("attacker_slow");
        }

    }

    public void SetDefaultValues() {

        Damage = defaultDamage;
        coolDown = defaultCooldown;

        attackerSlow = defaultAttackerSlow;
        attackerSlowTimer = defaultAttackerSlowTimer;

        pushForce = defaultForce;

    }

    ///<summary>Execute a attack</summary>
    public void Execute() {

        if(actCoolDown != 0) return;

        if(onPerform != null)
            onPerform();

        Vector2 dir = offset;
        dir.x *= motor.LastFacingDir;
        bool calledAttack = false;
        Collider2D[] hits = Physics2D.OverlapBoxAll((Vector2)transform.position + dir, size, 0f, aliveLayes);

        if(hits.Length > 0) {

            foreach(Collider2D c in hits) {
                if(c.gameObject == this.gameObject) continue;

                if(!calledAttack)
                {
                    onAttack?.Invoke();
                }

                if(c.TryGetComponent<Alive>(out Alive a)) ExecuteAttack(a, true);

            }

        }

        ApplySlow();
        ApplyCooldown();

    }

    void ApplyCooldown() {
        actCoolDown = coolDown;
    }

    void ApplySlow() {
        attackerSlow.Name = "attacker_slow";
        motor.AddSlow(attackerSlow, true);
        actAttackSlowTimer = attackerSlowTimer;
    }

    void ExecuteAttack(Alive a, bool ignoreCooldown, bool ignoreSlow = false,int customDamage = 0) {
        if(!ignoreCooldown && actCoolDown != 0) return;

        if(!ignoreCooldown) ApplyCooldown();
        if(!ignoreSlow) ApplySlow();

        bool received = a.TakeDamage(customDamage == 0 ? Damage : customDamage, this.gameObject);

        if(a.TryGetComponent<Motor>(out Motor m) && received) {
            
            float height = m.WallColliderSize.y / 2;
            Vector2 playerPos = (Vector2)a.transform.position + new Vector2(0, height);

            Vector2 direction = ((Vector2)transform.position - playerPos).normalized * -1;
            direction += new Vector2(0, 0.5f);

            Force f = new Force(pushForce);
            f.ForceApplied = direction * pushForce.ForceToApply;
            f.ActualForce = direction * pushForce.ForceToApply;

            m.AddForce(f, false, true, true);
        }
    }

    ///<summary>Executes a attack on a specific Alive</summary>
    ///<param name="objectToAttack">The alive component to attack</param>
    public void DirectAttack(GameObject objectToAttack, int customDamage = 0, bool ignoreCooldown = true, bool ignoreSlow = true) {
        if(objectToAttack.TryGetComponent<Alive>(out Alive a))
        {
            ExecuteAttack(a, ignoreCooldown, ignoreSlow, customDamage);
        }
    }

    void OnDrawGizmos() {

        if(!GetComponent<Motor>()) return;

        Vector2 dir = offset;
        dir.x *= motor.LastFacingDir == 0 ? 1 : motor.LastFacingDir;
        Vector3 pos = transform.position + (Vector3)dir;

        Gizmos.color = new Color(1, 1, 1, 0.8f);
        Gizmos.DrawCube(pos, size);

    }

}

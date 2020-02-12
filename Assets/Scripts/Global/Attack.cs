using UnityEngine;
using Undefined.Force;
using System;

[DisallowMultipleComponent]
public class Attack : MonoBehaviour
{

        #region Variables

    [Header("Raycast Settings")]
    public Vector2 offset;
    public Vector2 size;
    public LayerMask aliveLayes;

    [Header("Attack Settings")]
    public int Damage;
    public float coolDown;

    [Header("Force Settings")]
    public float pushForce;
    public float pushTime;

    [Header("Slow Settings")]
    public Slow attackerSlow;
    public float attackerSlowTimer;

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
        attackerSlow.Name = "attacker_slow";
    }

    void Update() {

        if(actCoolDown != 0)
            actCoolDown = Mathf.Clamp(actCoolDown - Time.deltaTime, 0, actCoolDown);

        if(actAttackSlowTimer != 0)
        {
            actAttackSlowTimer = Mathf.Clamp(actAttackSlowTimer - Time.deltaTime, 0, actAttackSlowTimer);

            if(actAttackSlowTimer <= 0 && motor.HasSlow("attacker_slow"))
                motor.RemoveSlow("attacker_slow");
        }

    }

    ///<summary>Execute a attack</summary>
    public void Execute() {

        if(actCoolDown != 0) return;

        if(onPerform != null)
            onPerform();

        actCoolDown = coolDown;

        Vector2 dir = offset;
        dir.x *= motor.lastFaceDir;
        bool calledAttack = false;
        Collider2D[] hits = Physics2D.OverlapBoxAll((Vector2)transform.position + dir, size, 0f, aliveLayes);

        motor.AddSlow(attackerSlow, true);
        actAttackSlowTimer = attackerSlowTimer;

        if(hits.Length > 0) {

            foreach(Collider2D c in hits) {
                if(c.gameObject == this.gameObject) continue;

                if(!calledAttack)
                {
                    onAttack?.Invoke();
                }

                if(c.TryGetComponent<Alive>(out Alive a)) {
                    DirectAttack(a, c.transform);
                }

            }

        }

    }

    ///<summary>Executes a attack on a specific Alive</summary>
    ///<param name="alive">The alive component to attack</param>
    ///<param name="collider">Collider to calculate push</param>
    public void DirectAttack(Alive alive, Transform aliveTrans = null) {
        alive.TakeDamage(Damage);
        if(alive.TryGetComponent<Motor>(out Motor m) && aliveTrans != null) {
            
            Vector2 direction = ((Vector2)transform.position - (Vector2)aliveTrans.position).normalized * -1;
            direction += new Vector2(0, 0.5f);
            Force f = new Force("attack", direction * pushForce, pushTime, true);

            m.AddForce(f, false, true, true);
        }
    }

    void OnDrawGizmos() {

        if(!GetComponent<Motor>()) return;

        Vector2 dir = offset;
        dir.x *= motor.lastFaceDir;
        Vector3 pos = transform.position + (Vector3)dir;

        Gizmos.color = new Color(1, 1, 1, 0.8f);
        Gizmos.DrawCube(pos, size);

    }

}

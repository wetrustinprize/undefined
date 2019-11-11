using UnityEngine;
using Undefined.Motor;
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

    // Events
    public event Action onAttack;
    public event Action onPerform;

    // Script side
    private Motor m { get {return GetComponent<Motor>(); } }
    private float actCoolDown = 0f;

        #endregion


    void Update() {

        if(actCoolDown != 0)
            actCoolDown = Mathf.Clamp(actCoolDown - Time.deltaTime, 0, actCoolDown);

    }

    ///<summary>Execute a attack</summary>
    public void Execute() {

        if(actCoolDown != 0) return;

        if(onPerform != null)
            onPerform();

        actCoolDown = coolDown;

        Vector2 dir = offset * m.lastFaceDir;
        bool calledAttack = false;
        Collider2D[] hits = Physics2D.OverlapBoxAll((Vector2)transform.position + dir, size, 0f, aliveLayes);

        if(hits.Length > 0) {

            foreach(Collider2D c in hits) {
                if(c.gameObject == this.gameObject) continue;

                if(!calledAttack)
                {
                    if(onAttack != null) {
                        onAttack();
                    }
                }

                Alive a;
                Motor m;

                if(c.TryGetComponent<Alive>(out a)) {
                    a.TakeDamage(Damage);
                    if(c.TryGetComponent<Motor>(out m)) {
                        
                        Vector2 direction = ((Vector2)transform.position - (Vector2)c.transform.position).normalized * -1;
                        direction += new Vector2(0, 0.5f);
                        Force f = new Force("attack", direction * pushForce, pushTime, true);

                        m.AddForce(f);
                    }
                }

            }

        }

    }

    void OnDrawGizmos() {

        Vector2 dir = offset * m.lastFaceDir;
        Vector3 pos = transform.position + (Vector3)dir;

        Gizmos.color = new Color(1, 1, 1, 0.8f);
        Gizmos.DrawCube(pos, size);

    }

}

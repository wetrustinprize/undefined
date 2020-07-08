using System;
using UnityEngine;

[DisallowMultipleComponent]
public class Alive : MonoBehaviour
{

        #region Variables

    [Header("Bypass")]
    public bool CanReceiveDamage = true;
    public bool CanReceiveHeal = true;

    [Header("Information")]
    [SerializeField] private int maxHealth;                         // The maximun HP this Alive can have
    [SerializeField] private int health;                            // The current HP this Alive have

    [Range(0.0f, 1.0f)]
    [SerializeField] private float defense;                         // The defense (from 0.0 to 1.0)

    // Public variables
    public int Health { get { return health; } }                    // Public acess to the health variable
    public int MaxHealth { get { return maxHealth; } }              // Public acess to the maxHealth variable
    public float Defense { get { return defense; } }                // Public acess to the defense variable

    // Events
    public event Action<int, GameObject> onHeal;                         // Called when the Alive heals
    public event Action<int, GameObject> onDamage;                       // Called when the Alive takes damage
    public event Action onDie;                                      // Called when the Alive dies (health < 0)

        #endregion

    void Start() {

        health = maxHealth;

    }

    void Update() {

        if(Input.GetKeyDown(KeyCode.H))
        {
            TakeDamage(1, this.gameObject);
        }

    }

    public void SetNewMaxHealth(int newMaxHealth, bool resetHealth = false) {

        maxHealth = newMaxHealth;

        if(resetHealth)
            health = newMaxHealth;

    }

    ///<summary>Sets a new value to the denfese</summary>
    ///<param name="newDefense">Value from 0.0 to 1.0</param>
    public void SetNewDefense(float newDefense) {

        defense = Mathf.Clamp(newDefense, 0.0f, 1.0f);

    }

    ///<summary>Takes damage or heals. Returns true if has received damage/heal</summary>
    ///<param name="damage">Damage to receive</param>
    public bool TakeDamage(int damage, GameObject dealer) {
        if(damage == 0) return false;
        if(damage > 0 && !CanReceiveDamage) return false;
        if(damage < 0 && !CanReceiveHeal) return false;

        if(damage > 0)
        {
            damage = (int)(damage * (1-defense));
        }

        health -= damage;

        if(health <= 0) {
            onDamage?.Invoke(damage, dealer);
            onDie?.Invoke();
        } else {
            if(damage > 0)
            {
                onDamage?.Invoke(damage, dealer);
            }
            else if(damage < 0)
            {
                onHeal?.Invoke(damage, dealer);
            }
        }

        return true;

    }

    ///<summary>Heals.</summary>
    ///<param name="value">Health to heal</param>
    public void Heal(int value, GameObject dealer = null) {
        if(value > 0)
            value *= -1;

        TakeDamage(value, dealer);
    }

}

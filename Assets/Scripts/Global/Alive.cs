﻿using System;
using UnityEngine;

[DisallowMultipleComponent]
public class Alive : MonoBehaviour
{

        #region Variables

    [SerializeField] private int maxHealth;                         // The maximun HP this Alive can have
    [SerializeField] private int health;                            // The current HP this Alive have
    [SerializeField] private float defense;                         // His defense (from 0% to 100%)

    // Public variables
    public int Health { get { return health; } }                    // Public acess to the health variable
    public int MaxHealth { get { return maxHealth; } }              // Public acess to the maxHealth variable
    public float Defense { get { return defense; } }                // Public acess to the defense variable

    // Events
    public float realDefense { get {return 1-(defense/100); } }     // Returns the real defense
    public event Action<int> onHeal;                                     // Called when the Alive heals
    public event Action<int> onDamage;                                   // Called when the Alive takes damage
    public event Action onDie;                                      // Called when the Alive dies (health < 0)

        #endregion

    void Start() {

        health = maxHealth;

    }

    void Update() {

        if(Input.GetKeyDown(KeyCode.H))
        {
            TakeDamage(1);
        }

    }

    public void SetNewMaxHealth(int newMaxHealth, bool resetHealth = false) {

        maxHealth = newMaxHealth;

        if(resetHealth)
            health = newMaxHealth;

    }

    public void SetNewArmor(int newArmor) {

        defense = newArmor;

    }

    ///<summary>Takes damage.</summary>
    ///<param name="damage">Damage to receive</param>
    public void TakeDamage(int damage) {

        if(damage == 0) return;

        if(damage > 0)
        {
            damage = (int)(damage * realDefense);
        }

        health -= damage;

        if(health <= 0) {
            onDamage?.Invoke(damage);
            onDie?.Invoke();
        } else {
            if(damage > 0)
            {
                onDamage?.Invoke(damage);
            }
            else if(damage < 0)
            {
                onHeal?.Invoke(damage);
            }
        }

    }

    ///<summary>Heals.</summary>
    ///<param name="value">Health to heal</param>
    public void Heal(int value) {
        TakeDamage(-value);
    }

}

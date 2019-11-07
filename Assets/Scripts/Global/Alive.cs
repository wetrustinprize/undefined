using System;
using UnityEngine;

[DisallowMultipleComponent]
public class Alive : MonoBehaviour
{

        #region Variables

    [SerializeField]
    private int maxHealth;

    public int health;
    public float defese;

    // Events

    public float realDefense { get {return 1-(defese/100); } }
    public event Action onHeal;
    public event Action onDamage;
    public event Action onDie;

        #endregion

    void Start() {

        health = maxHealth;

    }

    ///<summary>Takes damage.</summary>
    ///<param name="d">Damage to receive</param>
    public void TakeDamage(int d) {

        if(d == 0) return;

        if(d > 0)
        {
            d = (int)(d * realDefense);
        }

        health -= d;

        if(health <= 0) {
            if(onDie != null)
                onDie();
        } else {
            if(d > 0)
            {
                if(onDamage != null)
                    onDamage();
            }
            else if(d < 0)
            {
                if(onHeal != null)
                    onHeal();
            }
        }

    }

    ///<summary>Heals.</summary>
    ///<param name="h">Health to heal</param>
    public void Heal(int h) {
        TakeDamage(-h);
    }

}

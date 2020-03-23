using UnityEngine;

[RequireComponent(typeof(Motor))]
[RequireComponent(typeof(Jump))]
[RequireComponent(typeof(Alive))]
[DisallowMultipleComponent]
public class PlayerStats : MonoBehaviour {

                #region Variables

        [Header("Initial Values")]
        [SerializeField] private Vector2 initialSpeed;
        [SerializeField] private Vector2 finalSpeed;

        [Space(10)]
        [SerializeField] private int initialHealth;
        [SerializeField] private int finalHealth;

        [Space(10)]
        [SerializeField] private int initialArmor;
        [SerializeField] private int finalArmor;

        // Stat values
        private int agility;
        private int strength;
        private int toughness;

        public int Agility { get { return agility; } set { agility = value; UpdateAgility(); } }
        public int Strength { get { return strength; } set { strength = value; UpdateStrength(); } }
        public int Toughness { get { return toughness; } set { toughness = value; UpdateToughness(); } }

        // Local variables
        private Motor motor;
        private Alive alive;
        private Jump jump;

                #endregion

        void Start() {

                // Gets all components
                motor = GetComponent<Motor>();
                alive = GetComponent<Alive>();
                jump = GetComponent<Jump>();

                // Initial stat
                Agility = 1;
                Strength = 1;
                Toughness = 1;

        }

        void UpdateAgility() {

                Vector2 vel = Vector2.Lerp(initialSpeed, finalSpeed, agility / 100);
                motor.SetNewMaxSpeed(vel);
                
        }

        void UpdateStrength() {

                int health = (int)Mathf.Lerp(initialHealth, finalHealth, strength / 100);
                alive.SetNewMaxHealth(health);

        }

        void UpdateToughness() {

                int defense = initialArmor + finalArmor * toughness / 100;

                alive.SetNewArmor(defense);

        }


}
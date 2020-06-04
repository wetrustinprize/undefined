using UnityEngine;

[RequireComponent(typeof(Motor))]
[RequireComponent(typeof(Jump))]
[RequireComponent(typeof(Alive))]
[DisallowMultipleComponent]
public class PlayerStats : MonoBehaviour {

                #region Variables

        [Header("Initial Values")]
        [SerializeField] private Vector2 initialSpeed = Vector2.zero;
        public Vector2 AditionalSpeed { get { return aditionalSpeed; } set { aditionalSpeed = value; configureSpeed(); } }

        [Space(10)]
        [SerializeField] private Vector2 initialJumpSpeed = Vector2.zero;
        public Vector2 AditionalJumpSpeed { get { return aditionalJumpSpeed; } set { aditionalJumpSpeed = value; configureJump(); } }

        [Space(10)]
        [SerializeField] private Vector2 initialWallJumpSpeed = Vector2.zero;
        public Vector2 AditionalWallJumpSpeed { get { return aditionalWallJumpSpeed; } set { aditionalWallJumpSpeed = value; configureWallJump(); } }

        [Space(10)]
        [SerializeField] private int initialHealth = 10;
        public int AditionalHealth { get { return aditionalHealth; } set { aditionalHealth = value; configureHealth(); } }

        [Space(10)]
        [SerializeField] private float initialDefense = 0;
        public float AditionalDefense { get { return aditionalDefense; } set { aditionalDefense = value; configureDefense(); } }

        // Aditionals
        private Vector2 aditionalSpeed;
        private Vector2 aditionalJumpSpeed;
        private Vector2 aditionalWallJumpSpeed;
        private int aditionalHealth;
        private float aditionalDefense;

        
        // Local variables
        private Motor motor;
        private Alive alive;
        private Jump jump;

                #endregion

        void Awake() {

                // Gets all components
                motor = GetComponent<Motor>();
                alive = GetComponent<Alive>();
                jump = GetComponent<Jump>();

                // Configures aditional values
                AditionalSpeed = Vector2.zero;
                AditionalJumpSpeed = Vector2.zero;
                AditionalWallJumpSpeed = Vector2.zero;

                AditionalHealth = 0;
                AditionalDefense = 0;

        }

        void configureHealth() {
                alive.SetNewMaxHealth(initialHealth + AditionalHealth);
        }

        void configureDefense() {
                alive.SetNewDefense(initialDefense + AditionalDefense);
        }

        void configureJump() {
                jump.jumpForce = initialJumpSpeed + AditionalJumpSpeed;
        }

        void configureWallJump() {
                jump.wallJumpForce = initialWallJumpSpeed + AditionalWallJumpSpeed;
        }

        void configureSpeed() {
                motor.SetNewMaxSpeed(initialSpeed + AditionalSpeed);
        }


}
using UnityEngine;

public class EnemySimpleAnimation : MonoBehaviour
{

        #region Variables

    [Header("Animation")]
    [SerializeField] private bool ignoreInputVel = false;
    [SerializeField] private bool ignoreInputDir = false;

    [Space(10)]
    [SerializeField] private bool ignoreVel = false;
    [SerializeField] private bool ignoreCollisions = false;

    [Header("Animation Settings")]
    [SerializeField] private bool ignoreFlipX = false;
    [SerializeField] private bool ignoreFlipY = false;

    [Header("Animator Reference")]
    [SerializeField] private Animator animator = null;
    [SerializeField] private SpriteRenderer flipSprite = null;

    // Script side
    private Motor motor;

        #endregion

    void Start() {

        motor = GetComponent<Motor>();

    }


    void Update()
    {
        float inputHDir = motor.InputAxis.x;
        float inputVDir = motor.InputAxis.y;

        if(!ignoreInputVel)
        {
            // Get values
            Vector2 inputVel = motor.GetForce("input").ActualForce;

            float inputHVel = inputVel.x / motor.MaxSpeed.x;
            float inputVVel = inputVel.y / motor.MaxSpeed.y;

            // Apply animator values
            animator.SetFloat("inputHVel", inputHVel);
            animator.SetFloat("inputVVel", inputVVel);
        }

        if(!ignoreInputDir)
        {
            // Apply animator values
            animator.SetFloat("inputHDir", inputHDir);
            animator.SetFloat("inputVDir", inputVDir);
        }

        if(!ignoreVel)
        {
            // Get values
            float hVel = motor.finalSpeed.x;
            float vVel = motor.finalSpeed.y;

            // Apply animator values
            animator.SetFloat("hVel", hVel);
            animator.SetFloat("vVel", vVel);
        }

        if(!ignoreCollisions)
        {
            // Get values
            bool onAir = !motor.OnGround;
            bool onWall = motor.OnWall;

            // Apply animator values
            animator.SetBool("onAir", onAir);
            animator.SetBool("onWall", onWall);
        }

        // Check flip
        if(!ignoreFlipX)
        {
            if(inputHDir > 0) flipSprite.flipX = false;
            else if(inputHDir < 0) flipSprite.flipX = true;
        }

        if(!ignoreFlipY)
        {
            if(inputVDir > 0) flipSprite.flipY = true;
            else if(inputVDir < 0) flipSprite.flipY = false;
        }

    }
}

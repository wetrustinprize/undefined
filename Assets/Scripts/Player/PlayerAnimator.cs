﻿using UnityEngine;

[DisallowMultipleComponent]
public class PlayerAnimator : MonoBehaviour
{
    
        #region Variables

    [Header("Animator Settings")]
    [SerializeField] private Motor motor;               // Reference to the Player Motor component
    [SerializeField] private Attack attack;             // Reference to the Attack component
    [SerializeField] private Jump jump;                 // Reference to the Jump component
    [SerializeField] private float driftThreshold;      // The threshold to play the drift animation

    [Header("Particles")]
    [SerializeField] private GameObject dustParticles;          // The dust particles when walking/running
    [SerializeField] private ParticleSystem wallParticles;      // The wall dust particles when walking/running
    [SerializeField] private GameObject dashParticles;          // The dash particles when dashing
    [SerializeField] private Transform dustSpawnTransform;      // The dust particles transform

    [Header("Animator References")]
    [SerializeField] private Animator animator;             // Reference to the animator
    [SerializeField] private SpriteRenderer sprite;         // Reference to the sprite renderer

    [Header("Camera Refereces")]
    [SerializeField] private CameraController cam;          // Reference to the camera controller

    //Script side var

    private bool attacked;
    private bool dashParticleSpawned;
    private float wallParticleDefaultX;
    private float squishPercentage;

        #endregion

    void Start() {

        // Setup the attack events
        attack.onPerform += AttackAnim;
        attack.onAttack += AttackEffect;

        // Sets the initial transform of the wall particle
        wallParticleDefaultX = wallParticles.gameObject.transform.localPosition.x;

    }

        #region Attack Events

    // Called when attacks
    void AttackAnim() {

        attacked = true;

    }

    // Called when hits something
    void AttackEffect() {

        Vector2 dir = new Vector2(0.3f * motor.lastFaceDir, 0);
        cam.Push(dir, 0, 0.1f);

    }

        #endregion

    // Called to create the dust particles
    public void DustParticles() {

        if(!motor.OnGround) return;

        Vector3 pos = dustSpawnTransform.position;

        Quaternion rot = Quaternion.identity;

        float offset = dustSpawnTransform.localPosition.x;

        if(motor.lastFaceDir == -1)
        {
            pos.x -= offset*2;
            rot.y = 180;
        }

        GameObject.Instantiate(dustParticles, pos, rot);

    }

    // Called to create the dash particles
    public void DashParticles() {

        Quaternion rot = Quaternion.identity;

        if(motor.lastFaceDir == 1)
        {
            rot.y = 180;
        }

        GameObject.Instantiate(dashParticles, transform.parent.position, rot, transform);

    }

    // Updates the animator
    void Update() {
        // Gets all velocity info
        float vInput = motor.inputAxis.x;
        float hVel = motor.GetForce("input").ActualForce.x / motor.MaxSpeed.x;
        float vDir = hVel > 0 ? 1 : -1;
        float vFace = motor.lastFaceDir;

        float vVel = motor.finalSpeed.y;

        // Get all states
        bool onAir = !motor.OnGround;
        bool drifting = vInput != 0 ? (vDir != vInput) : false;
        bool onWall = motor.OnWall;
        bool canWallJump = jump.canWallJump;
        bool dashing = motor.HasForce("dash", false);

        // Check if should flip
            #region Flip
        if(vInput != 0) {
            bool flip = false;

            if(!onAir)
            {

                if(!onWall)
                {
                    if(vInput < 0)
                        flip = hVel < driftThreshold;
                    else
                        flip = !(hVel > driftThreshold);
                }
                else
                {
                    flip = vInput < 0;
                }
            }
            else if(onWall && onAir && canWallJump)
            {
                flip = motor.onRightWall ? false : true;
            }
            else
            {
                flip = vInput < 0;
            }

            sprite.flipX = flip;
        }
        else if(sprite.flipX && motor.lastFaceDir == 1 && !attacked)
        {
            sprite.flipX = false;
        }
            #endregion


        // Send values to the animator
        animator.SetFloat("SpeedH", Mathf.Abs(hVel));
        animator.SetFloat("SpeedV", vVel);
        animator.SetFloat("Input", Mathf.Abs(vInput));
        animator.SetBool("OnAir", onAir);
        animator.SetBool("Drift", drifting);
        animator.SetBool("OnWall", onWall && canWallJump && onAir);
        animator.SetBool("Dashing", dashing);
        animator.SetBool("Attack", attacked);

        // Squish effect
        squishPercentage = Mathf.Lerp(squishPercentage, Mathf.Clamp(vVel / -70, 0, 1), Time.fixedDeltaTime);
        sprite.transform.localScale = new Vector3(1, 1 + (0.05f * squishPercentage), 1);

        // Enables/disables wall particles
        Vector2 pos = wallParticles.gameObject.transform.localPosition;
        wallParticles.gameObject.transform.localPosition = new Vector2(wallParticleDefaultX * (sprite.flipX ? -1 : 1), pos.y);

        if(!(onWall && onAir && canWallJump))
            wallParticles.Play(true);

        // Spawns and checks the dash particles
        if(!dashParticleSpawned && dashing)
        {
            dashParticleSpawned = true;
            DashParticles();
        } else if(dashParticles && !dashing)
        {
            dashParticleSpawned = false;
        }

        if(attacked) attacked = false;
    }

}

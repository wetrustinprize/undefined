using UnityEngine;
using System;

public enum CreatureState {

    Following,
    Bouncing,
    Explosion

}

public class CreatureController : MonoBehaviour
{

        #region Variables

    [Header("State")]
    [SerializeField] private CreatureState state;   // The current state of the creature

    [Header("Following")]
    [SerializeField] private float follow_Time;             // The time to the creature get close to the player
    [SerializeField] private Vector2 follow_Offset;         // The off of the creature

    [Header("Bouncing")]
    [SerializeField] private float bounce_Slowing;          // Slow percentage of each bounce
    [SerializeField] private int bounce_Max;                // Maximun bounces
    [SerializeField] private int bounce_Min;                // Min bounces to the player teleport (used for sfx)
    [SerializeField] private Vector2 bounce_Dir;            // The direction to fly towards
    [SerializeField] private float bounce_Velocity;         // The velocity of the fly
    [SerializeField] private float bounce_MaxDist;          // Maximung distance to fly
    [SerializeField] private float bounce_MaxPlayerDist;    // Maximung distance from player
    [SerializeField] private LayerMask bounce_Layers;       // The layers to perform the raycast

    [Header("SFX")]
    [SerializeField] private AudioClip bounce_NoTeleportSFX;    // Bounce SFX (can't teleport)
    [SerializeField] private AudioClip bounce_TeleportSFX;      // Bounce SFX (can teleport)

    // Events
    public Action OnBounceEnd;                      // Called when bounce ends
    public Action OnBounceStart;                    // Called when bounce starts

    // Script side
    private GameObject player;                      // The player gameobject
    private Motor playerMotor;                      // The player motor

    private int lastFaceDir;                        // Last facing direction
    private bool isNearPlayer;                      // Is near to the player

    private float bounceActualDist;                 // The actual distance
    private int bounceActualBounces;                // The total bounces
    private float bounceActualVelocity;             // The actual fly velocity

    private AudioSource audioSource;                // This creature audio source

    // Public acces variables
    public int LastFacingDir { get { return lastFaceDir; } }        // Public acess to the lastFaceDir varaible
    public bool IsNearToThePlayer { get { return isNearPlayer; } }  // Public acess to the isNearPlayer variable
    public CreatureState State { get { return state; } }            // Public acess to the state variable

    public int TotalBounces { get { return bounceActualBounces; } }             // Public acess to the bounceActualBounces variable
    public bool IsBouncing { get { return state == CreatureState.Bouncing; } }  // Faster way to check if is bouncing

        #endregion

        #region Monobehaviour stuff

    void Start() {
        
        // gets the player gameobject and motor
        player = GameObject.FindWithTag("Player");
        playerMotor = player.GetComponent<Motor>();

        // gets audio source
        audioSource = GetComponent<AudioSource>();

        // sets initial face dir
        lastFaceDir = 1;

    }

    void FixedUpdate() {

        switch(state)
        {
            case CreatureState.Following:
                FollowingBehaviour();
                break;
            
            case CreatureState.Bouncing:
                BouncingBehaviour();
                break;
        }

    }

        #endregion

        #region Public functions

    public void SetMaxBounces(int max) {

        bounce_Max = max;

    }

    public void SetMinBounces(int min) {
        
        bounce_Min = min;

    }

    public void Bounce(Vector2 direction) {

        if(direction == Vector2.zero) return;

        transform.position = player.transform.position;

        bounce_Dir = direction.normalized;
        bounceActualDist = 0;
        bounceActualBounces = 0;
        bounceActualVelocity = bounce_Velocity;

        ChangeState(CreatureState.Bouncing);

    }

    public void ChangeState(CreatureState newState) {

        switch(state)
        {
            case CreatureState.Bouncing:
                OnBounceEnd?.Invoke();
                break;
        }

        switch(newState)
        {
            case CreatureState.Bouncing:
                OnBounceStart?.Invoke();
                break;
        }

        state = newState;

    }

        #endregion

        #region Private functions

    void Move(Vector2 position) {

        Vector2 difference = position - (Vector2)transform.position;

        if(difference.x > 0)
            lastFaceDir = 1;
        else if(difference.x < 0)
            lastFaceDir = -1;
    
        transform.position = position;

    }

        #endregion

        #region Behaviours

    void BouncingBehaviour() {

        // Checks distances
        if(bounceActualDist > bounce_MaxDist) { ChangeState(CreatureState.Following); return; }
        if(Vector2.Distance(transform.position, player.transform.position) > bounce_MaxPlayerDist) { ChangeState(CreatureState.Following); return; }

        // Raycast
        RaycastHit2D hit = Physics2D.Raycast(transform.position, bounce_Dir, bounce_Velocity * Time.fixedDeltaTime, bounce_Layers);

        // Bounce
        if(hit.collider) {

            // Check total bounce
            bounceActualBounces++; 
            if(bounceActualBounces >= bounce_Max) { ChangeState(CreatureState.Following); return; }

            // Reflect direction
            bounce_Dir = bounce_Dir - 2 * (bounce_Dir * hit.normal) * hit.normal; 
            bounceActualVelocity *= bounce_Slowing;

            // Play sfx
            if(bounceActualBounces < bounce_Min)
                PlaySFX(bounce_NoTeleportSFX);
            else if(bounceActualBounces == bounce_Min)
                PlaySFX(bounce_TeleportSFX);
        }

        // Calculate direction
        Vector3 dir = (Vector3)(bounce_Dir * bounceActualVelocity) * Time.fixedDeltaTime;

        // Adds up distance
        bounceActualDist += Mathf.Abs(dir.x) + Mathf.Abs(dir.y);

        // Moves the creature
        Move(transform.position + dir);

    }

    void FollowingBehaviour() {

        isNearPlayer = Vector2.Distance(transform.position - (Vector3)follow_Offset, player.transform.position) < 1;

        Move(Vector3.Lerp(  
                transform.position, 
                player.transform.position + (Vector3)follow_Offset, 
                Time.fixedDeltaTime / follow_Time
            )
        );

    }

    void PlaySFX(AudioClip sfx) {

        audioSource.PlayOneShot(sfx);

    }

        #endregion

}

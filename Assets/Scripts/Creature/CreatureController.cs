using UnityEngine;
using System;

public enum CreatureState {

    Following,

    Bouncing,

    GoingToExplode,
    Exploding,

}

public class CreatureController : MonoBehaviour
{

        #region Variables

    [Header("State")]
    [SerializeField] private CreatureState state = CreatureState.Following;   // The current state of the creature

    [Header("Following")]
    [SerializeField] private float follow_Time = 0.1f;             // The time to the creature get close to the player
    [SerializeField] private Vector2 follow_Offset = Vector2.zero;         // The off of the creature

    [Header("Bouncing")]
    [SerializeField] private float bounce_Slowing = 0.8f;          // Slow percentage of each bounce
    [SerializeField] private int bounce_Max = 3;                // Maximun bounces
    [SerializeField] private int bounce_Min = 0;                // Min bounces to the player teleport (used for sfx)
    [SerializeField] private Vector2 bounce_Dir = Vector2.zero;            // The direction to fly towards
    [SerializeField] private float bounce_Velocity = 50;         // The velocity of the fly
    [SerializeField] private float bounce_MaxDist = 50;          // Maximung distance to fly
    [SerializeField] private float bounce_MaxPlayerDist = 10000;    // Maximung distance from player
    [SerializeField] private LayerMask bounce_Layers = 0;       // The layers to perform the raycast
    [SerializeField] private Vector2 bounce_PlayerOffset = Vector2.zero;

    [Header("Explosion")]
    [SerializeField] private float explosion_Velocity = 60;      // The velocity which the creature travels to the explosion destionation
    [SerializeField] private float explosion_Time = 0.3f;          // The time the creature stays in the "explosion" animation

    [Header("SFX")]
    [SerializeField] private AudioClip bounce_NoTeleportSFX = null;    // Bounce SFX (can't teleport)
    [SerializeField] private AudioClip bounce_TeleportSFX = null;      // Bounce SFX (can teleport)

    // Events
    public Action OnBounceEnd;                      // Called when bounce ends
    public Action OnBounceStart;                    // Called when bounce starts

    public Action OnExplodeStart;                   // Called when starting the explosion behaviour
    public Action OnExplode;                        // Called when explodes
    public Action OnExplodeEnd;                     // Called when ended the explosion behaviour

    // Script side
    private GameObject player;                      // The player gameobject
    private Motor playerMotor;                      // The player motor

    private int lastFaceDir;                        // Last facing direction
    private bool isNearPlayer;                      // Is near to the player

        #region Explosion Variables

    private Vector2 explosionPosition;
    private float explosionTimer;

        #endregion

        #region Bounce Variables

    private float bounceActualDist;                 // The actual distance
    private int bounceActualBounces;                // The total bounces
    private float bounceActualVelocity;             // The actual fly velocity

        #endregion

    private AudioSource audioSource;                // This creature audio source

    // Public acces variables
    public int LastFacingDir { get { return lastFaceDir; } }        // Public acess to the lastFaceDir varaible
    public bool IsNearToThePlayer { get { return isNearPlayer; } }  // Public acess to the isNearPlayer variable
    public CreatureState State { get { return state; } }            // Public acess to the state variable

    public int TotalBounces { get { return bounceActualBounces; } }             // Public acess to the bounceActualBounces variable
    public bool IsBouncing { get { return state == CreatureState.Bouncing; } }  // Faster way to check if is bouncing
    public bool IsExploding { get { return state == CreatureState.Exploding; } }
    public bool IsBusy { get { return state != CreatureState.Following; } }

        #endregion

        #region Monobehaviour stuff

    void Start() {
        
        // gets the player gameobject and motor
        player = GameManager.Player.gameObject;
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

            // Bounce
            
            case CreatureState.Bouncing:
                BouncingBehaviour();
                break;
            
            // Explosion

            case CreatureState.GoingToExplode:
                GoingToExplodeBehaviour();
                break;
            
            case CreatureState.Exploding:
                ExplodingBehaviour();
                break;

        }

    }

        #endregion

        #region Public functions

        #region Bounce

    public void SetMaxBounces(int max) {

        bounce_Max = max;

    }

    public void SetMinBounces(int min) {
        
        bounce_Min = min;

    }

    public void Bounce(Vector2 direction) {

        if(IsBusy) return;
        if(direction == Vector2.zero) return;

        transform.position = player.transform.position + (Vector3)bounce_PlayerOffset;

        bounce_Dir = direction.normalized;
        bounceActualDist = 0;
        bounceActualBounces = 0;
        bounceActualVelocity = bounce_Velocity;

        ChangeState(CreatureState.Bouncing);

    }


        #endregion

        #region Explosion

    public void Explode(Vector2 newExplodePos) {

        if(IsBusy) return;

        explosionPosition = newExplodePos;
        ChangeState(CreatureState.GoingToExplode);

    }

        #endregion

    public void ChangeState(CreatureState newState, bool force = false) {

        if(state != CreatureState.Following && !force) return;

        switch(state)
        {
            case CreatureState.Bouncing:
                OnBounceEnd?.Invoke();
                break;
            
            case CreatureState.Exploding:
                OnExplodeEnd?.Invoke();
                break;
        }

        switch(newState)
        {
            case CreatureState.Bouncing:
                OnBounceStart?.Invoke();
                break;
            
            case CreatureState.GoingToExplode:
                OnExplodeStart?.Invoke();
                break;

            case CreatureState.Exploding:
                explosionTimer = 0f;
                OnExplode?.Invoke();
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
        if(bounceActualDist > bounce_MaxDist) { ChangeState(CreatureState.Following, true); return; }
        if(Vector2.Distance(transform.position, player.transform.position) > bounce_MaxPlayerDist) { ChangeState(CreatureState.Following, true); return; }

        // Raycast
        RaycastHit2D hit = Physics2D.Raycast(transform.position, bounce_Dir, bounce_Velocity * Time.fixedDeltaTime, bounce_Layers);

        // Bounce
        if(hit.collider) {

            // Check total bounce
            bounceActualBounces++; 
            if(bounceActualBounces >= bounce_Max) { ChangeState(CreatureState.Following, true); return; }

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

        #region Explosion Behaviours

    void GoingToExplodeBehaviour() {

        float distance = Vector2.Distance(transform.position, explosionPosition);

        if(distance > 1)
        {
            Vector2 dir = ((Vector2)transform.position - explosionPosition) * -1;
            dir.Normalize();
            Move((Vector2)transform.position + dir * explosion_Velocity * Time.fixedDeltaTime);
        }
        else
        {
            ChangeState(CreatureState.Exploding, true);
        }

    }

    public void  CreateExplosion() {

        Instantiate(GameManager.Player.GetComponent<PlayerExplosion>().explosion_Prefab, transform.position, transform.rotation);

    }

    void ExplodingBehaviour() {

        if(explosionTimer < explosion_Time)
        {
            explosionTimer += Time.fixedDeltaTime;
        }
        else
        {
            ChangeState(CreatureState.Following, true);
        }

    }

        #endregion

        #endregion

    void PlaySFX(AudioClip sfx) {

        audioSource.PlayOneShot(sfx);

    }

}

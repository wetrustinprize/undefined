using UnityEngine;

public class PortalBoss_Master : MonoBehaviour
{

        #region Variables

    [Header("Animation")]
    [SerializeField] private PortalBoss_Animation animationManager;
    [SerializeField] private Animator animationAnimator;

    [Header("Height Settings")]
    [SerializeField] private float maxHeight = 0f;

    [Header("Big Rock")]
    [SerializeField] private PortalBoss_BigRock[] bigRocks;
    [SerializeField] private AnimationCurve bigRockDelayCurve;
    [SerializeField] private float bigRockDelayStart;
    [SerializeField] private float bigRockDelayEnd;

    [Space]
    [SerializeField] private AnimationCurve bigRockFloatVelocityCurve;
    [SerializeField] private float bigRockFloatVelocityStart;
    [SerializeField] private float bigRockFloatVelocityEnd;

    [Header("Laser Rock")]
    [SerializeField] private PortaBoss_LaserRock laserRock;
    [SerializeField] private AnimationCurve laserRockDelayCurve;
    [SerializeField] private float laserRockDelayStart;
    [SerializeField] private float laserRockDelayEnd;
    
    [Space]
    [SerializeField] private AnimationCurve laserRockPrepSpeedCurve;
    [SerializeField] private float laserRockPrepSpeedStart;
    [SerializeField] private float laserRockPrepSpeedEnd;

    // Script side variables
    private bool activated = false;

    private float startHeight = 0f;
    private Transform player;
    private float heightPercentage = 0f;

    private float bigRockActualDelay = 0f;
    private float bigRockActualFloatVelocity = 0f;

    private float laserRockActualDelay = 0f;
    private float laserRockActualPrepSpeed = 0f;

    private Vector2[] bigRockPos;
    private Vector2 laserRockPos;

    // Public acess variables
    public float BigRockActualDelay { get { return bigRockActualDelay; } }
    public float BigRockActualVelocity { get { return bigRockActualFloatVelocity; } }
    public float BigRockActualPercantage { get { return this.bigRockDelayCurve.Evaluate(this.heightPercentage); } }

    public float LaserActualDelay { get { return laserRockActualDelay;} }
    public float LaserActualPrepSpeed { get { return laserRockActualPrepSpeed; } }
    public float LaserActualPercentage { get { return this.laserRockDelayCurve.Evaluate(this.heightPercentage); } }

        #endregion


    void Start() {

        this.startHeight = this.transform.position.y;
        this.player = GameManager.Player.gameObject.transform;

        this.activated = false;

        ToggleComponents(false);

        // Stop attacking when player dies
        GameManager.Player.alive.onDie += () => { AwayAll(); };

        // On load reset positions
        GameManager.Checkpoint.onLoad += () => { Reset(); };

        // Save positions
        bigRockPos = new Vector2[bigRocks.Length];

        for(int i = 0; i < bigRocks.Length; i++)
        {
            bigRockPos[i] = bigRocks[i].gameObject.transform.position;
        }

        laserRockPos = laserRock.transform.position;

    }

    public void Activate() {
        this.activated = true;

        CalculateBigRockDelay();
        CalculateBigRockVelocity();

        CalculateLaserRockDelay();
        CalculateLaserRockPrepSpeed();

        ToggleComponents(true);
    }

    public void Reset() {

        this.animationManager.animateFast = this.activated;

        Deactivate();

        for(int i = 0; i < bigRocks.Length; i++)
        {
            this.bigRocks[i].gameObject.transform.position = bigRockPos[i];
            this.bigRocks[i].Reset();
        }

        this.laserRock.gameObject.transform.position = laserRockPos;
        this.laserRock.Reset();

        this.player = GameManager.Player.gameObject.transform;

        this.animationManager.ResetAnimation();
        this.animationManager.hasAnimated = false;
        this.animationAnimator.enabled = true;

    }

    public void Deactivate() {

        this.activated = false;

        ToggleComponents(false);

    }

    public void AwayAll() {

        foreach(PortalBoss_BigRock br in bigRocks)
        {
            br.SetAway();
        }

        laserRock.SetAway();

    }

    void ToggleComponents(bool enable) {

        foreach(PortalBoss_BigRock br in bigRocks)
        {
            br.activated = enable;
        }

        laserRock.activated = enable;

    }

    void Update() {

        if(!activated) return;

        this.heightPercentage = Mathf.Clamp((this.player.position.y - this.startHeight) / this.maxHeight, 0, 1);

        CalculateBigRockDelay();
        CalculateBigRockVelocity();

        CalculateLaserRockDelay();
        CalculateLaserRockPrepSpeed();
    }

    void CalculateLaserRockDelay() {

        laserRockActualDelay = Mathf.Lerp(
            this.laserRockDelayStart,
            this.laserRockDelayEnd,
            this.laserRockDelayCurve.Evaluate(this.heightPercentage)
        );

    }

    void CalculateLaserRockPrepSpeed() {

        laserRockActualPrepSpeed = Mathf.Lerp(
            this.laserRockPrepSpeedStart,
            this.laserRockPrepSpeedEnd,
            this.laserRockPrepSpeedCurve.Evaluate(this.heightPercentage)
        );

    }

    void CalculateBigRockVelocity() {

        bigRockActualFloatVelocity = Mathf.Lerp(
            this.bigRockFloatVelocityStart,
            this.bigRockFloatVelocityEnd,
            this.bigRockDelayCurve.Evaluate(this.heightPercentage)
        );

    }

    void CalculateBigRockDelay() {

        bigRockActualDelay = Mathf.Lerp(
            this.bigRockDelayStart, 
            this.bigRockDelayEnd,
            this.bigRockDelayCurve.Evaluate(this.heightPercentage)
            );

    }

}

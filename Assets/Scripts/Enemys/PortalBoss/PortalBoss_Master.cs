using UnityEngine;

public class PortalBoss_Master : MonoBehaviour
{

        #region Variables

    [Header("Height Settings")]
    [SerializeField] private float maxHeight = 0f;

    [Header("Big Rock")]
    [SerializeField] private AnimationCurve bigRockDelayCurve;
    [SerializeField] private float bigRockDelayStart;
    [SerializeField] private float bigRockDelayEnd;

    [Space]
    [SerializeField] private AnimationCurve bigRockVelocityCurve;
    [SerializeField] private float bigRockVelocityStart;
    [SerializeField] private float bigRockVelocityEnd;

    [Header("Laser Rock")]
    [SerializeField] private AnimationCurve laserRockDelayCurve;
    [SerializeField] private float laserRockDelayStart;
    [SerializeField] private float laserRockDelayEnd;

    // Script side variables
    private float startHeight = 0f;
    private Transform player;
    private float heightPercentage = 0f;

    private float bigRockActualDelay = 0f;
    private float bigRockActualVelocity = 0f;

    // Public acess variables
    public float BigRockActualDelay { get { return bigRockActualDelay; } }
    public float BigRockActualVelocity { get { return bigRockActualVelocity; } }
    public float BigRockActualPercantage { get { return this.bigRockDelayCurve.Evaluate(this.heightPercentage); } }
    public float LaserActualPercentage { get { return this.laserRockDelayCurve.Evaluate(this.heightPercentage); } }

        #endregion


    void Start() {

        this.startHeight = this.transform.position.y;
        this.player = GameManager.Player.gameObject.transform;
    }

    void Update() {

        this.heightPercentage = Mathf.Clamp((this.player.position.y - this.startHeight) / this.maxHeight, 0, 1);

        CalculateBigRockDelay();
        CalculateBigRockVelocity();
    }

    void CalculateBigRockVelocity() {

        bigRockActualVelocity = Mathf.Lerp(
            this.bigRockVelocityStart,
            this.bigRockVelocityEnd,
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

using UnityEngine;

public class PortalBoss_BigRock : MonoBehaviour
{

    enum BigRockState {

        Following,
        Floating,
        Falling,
        Away,

    }

        #region Variables

    [SerializeField] private PortalBoss_Master master;
    [SerializeField] private BigRockState state;

    [Header("Move")]
    [SerializeField] private float moveMinVelocity = 0.1f;
    [SerializeField] private float moveMaxVelocity = 0.2f;

    [Header("Float")]
    [SerializeField] private float floatMinVelocity = 1f;
    [SerializeField] private float floatMaxVelocity = 3f;
    [SerializeField] private float floatHeight = 30f;

    [Header("Fall")]
    [SerializeField] private float fallTime = 0.2f;
    [SerializeField] private Attack rockAttack = null;

    [Header("Distance Check")]
    [SerializeField] private float distanceThreshold = 0.01f;

    [Space]
    public bool activated = false;

    // Script side variables
    private GameObject player;
    private Vector3 gotoVector;

    private float fallDelayTimer = 0f;
    private float fallTimer = 0f;
    private float fallYCoord = 0f;
    private bool attacked = false;

    private float moveVelocity = 0f;
    private float floatVelocity = 0f;
    private float fallDelay = 0f;

        #endregion    

    void Start() {

        ChangeState(BigRockState.Floating);

        this.player = GameManager.Player.gameObject;

    }

    public void SetAway() {
        ChangeState(BigRockState.Away);
    }

    void ChangeState(BigRockState newState)
    {

        switch(newState)
        {
            case BigRockState.Floating:
                this.floatVelocity = Random.Range(floatMinVelocity, floatMaxVelocity);
                break;

            case BigRockState.Following:
                this.moveVelocity = Random.Range(moveMinVelocity, moveMaxVelocity);
                break;

            case BigRockState.Falling:
                this.fallDelay = master.BigRockActualDelay;
                this.fallDelayTimer = this.fallDelay;
                this.fallYCoord = this.transform.position.y - this.floatHeight * 3;
                this.fallTimer = 0f;
                this.attacked = false;
                break;
        }

        state = newState;

    }

    void FixedUpdate() {

        if(!activated) return;
        if(this.state == BigRockState.Away) { AwayBehaviour(); return; }

        this.gotoVector = this.transform.position;

        switch(state)
        {
            case BigRockState.Floating:
                FloatCalculation();
                break;

            case BigRockState.Following:
                FloatCalculation();
                FollowCalculation();
                break;

            case BigRockState.Falling:
                FallingCalculation();
                break;
        }

        this.transform.position = gotoVector;

    }

    void AwayBehaviour() {

        Vector3 awayPos = this.player.transform.position + new Vector3(0, 60, 0);
        this.transform.position = Vector3.Lerp(this.transform.position, awayPos, Time.fixedDeltaTime / 5);

    }

    void FloatCalculation() {

        float multiplyer = state == BigRockState.Falling ? 5 : 1;

        this.gotoVector.y = Mathf.Lerp(
            gotoVector.y, 
            this.player.transform.position.y + this.floatHeight,
            Time.fixedDeltaTime * this.floatVelocity * this.master.BigRockActualVelocity * multiplyer
            );

        float distance = Mathf.Round(Mathf.Abs(this.transform.position.y - this.player.transform.position.y + this.floatHeight));

        if(this.state == BigRockState.Floating)
        {
            if(distance - this.floatHeight >= this.floatHeight)
                ChangeState(BigRockState.Following);
        }


    }

    void FollowCalculation() {

        this.gotoVector.x -= (Vector3.Normalize(this.transform.position - this.player.transform.position) * this.moveVelocity * (1 + this.master.BigRockActualVelocity)).x;
        
        float distance = Mathf.Abs(this.transform.position.x - this.player.transform.position.x);

        if(distance < this.distanceThreshold)
            ChangeState(BigRockState.Falling);
    }

    void FallingCalculation() {

        if(fallDelayTimer > 0) { fallDelayTimer -= Time.fixedDeltaTime * this.master.BigRockActualVelocity; FloatCalculation(); return; }

        this.gotoVector.y = Mathf.Lerp(this.gotoVector.y, this.fallYCoord, this.fallTimer / this.fallTime);
        this.fallTimer += Time.fixedDeltaTime;

        if(this.fallTimer / this.fallTime >= 1f)
            ChangeState(BigRockState.Floating);

    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == "Player" && !this.attacked && this.state == BigRockState.Falling && this.fallDelayTimer <= 0)
        {
            this.attacked = true;
            this.rockAttack.DirectAttack(player);
        }
    }

}

using UnityEngine;

public enum GroundEnemyBehaviour {

    Idle,
    Chasing

}

[RequireComponent(typeof(Alive))]
[RequireComponent(typeof(Motor))]
[RequireComponent(typeof(CollisionCheckModule))]
[RequireComponent(typeof(VisionModule))]
public class GroundEnemy : BaseAgent, IChunkEntity
{

        #region Variables
    
    [Header("Ground Enemy Settings")]
    [SerializeField] private GroundEnemyBehaviour myBehaviour = GroundEnemyBehaviour.Idle;
    [SerializeField] private float distanceToStopChasing = 100;
    [SerializeField] private float distanceToAttack = 1.5f;

        #region References

    CollisionCheckModule collisionModule;
    VisionModule visionModule;
    Jump jumpModule;
    Attack attackModule;
    Alive aliveModule;
    GoldDropModule goldModule;
    GameObject player;

    bool dieNextFrame = false;

        #endregion

    // Script side
    bool outOfChunk = true;

        #endregion

    protected override void Start() {
        base.Start();

        // Getting all modules
        this.collisionModule = this.GetComponent<CollisionCheckModule>();
        this.visionModule = this.GetComponent<VisionModule>();
        this.aliveModule = this.GetComponent<Alive>();
        this.goldModule = this.GetComponent<GoldDropModule>();

        this.jumpModule = this.TryGetComponent(out Jump jump) ? jump : null;
        this.attackModule = this.TryGetComponent(out Attack attack) ? attack : null;

        // Getting player
        this.player = GameManager.Player.gameObject;

        // Settings events
        this.myMotor.onTouchWall += OnTouchWall;
        this.aliveModule.onDamage += OnDamage;
        this.aliveModule.onDie += () => { dieNextFrame = true; goldModule.Spawn(); };

    }

    void Update() {
        if(dieNextFrame) Destroy(this.gameObject);
    }

    public void OnChunk() {
        outOfChunk = false;
        myMotor.SetFreeze(false);
    }

    public void OutChunk() {
        outOfChunk = true;
        myMotor.SetFreeze(true);
        myMotor.ReceiveInput(Vector2.zero);
    }

    void OnDamage(int damage, GameObject dealer) {

        if(dealer == player)
        {
            ChangeBehaviour(GroundEnemyBehaviour.Chasing);
        }

    }

    void FixedUpdate() {

        if(outOfChunk) return;

        switch(myBehaviour) {
            case GroundEnemyBehaviour.Idle:
                this.IdleBehaviour();
                break;
            
            case GroundEnemyBehaviour.Chasing:
                this.ChasingBehaviour();
                break;
        }


    }

    bool CheckInDistance() {

        float dist = Vector2.Distance(this.transform.position, player.transform.position);

        if(dist > distanceToStopChasing )
        {
            return false;
        }
        else
        {
            if(dist < distanceToAttack && attackModule != null)
            {  
                attackModule.DirectAttack(player, 0, false, false);
            }

            return true;
        }

    }

    void OnTouchWall(int wallSide) {

        if((this.visionModule.FacingDir.x > 0 && wallSide == 1) || (this.visionModule.FacingDir.x < 0 && wallSide == -1))
            this.visionModule.SetFacingDir(this.visionModule.FacingDir * -1);

    }

        #region Behaviours

    void IdleBehaviour() {

        if(myMotor.OnGround)
        {
            if(collisionModule.WillFall(this.visionModule.FacingDir.x))
                this.visionModule.SetFacingDir(this.visionModule.FacingDir * -1);

            myMotor.ReceiveInput(this.visionModule.FacingDir);
        }

        if(visionModule.RaycastVision(true, player, 15f)) {

            this.ChangeBehaviour(GroundEnemyBehaviour.Chasing);

        }

    }

    void ChasingBehaviour() {

        if(!CheckInDistance()) { ChangeBehaviour(GroundEnemyBehaviour.Idle); return; }

        this.CalculatePath(player.transform.position);
        
        this.CheckWaypointDistance();
        
        this.myMotor.ReceiveInput(this.DirToPath);
        this.visionModule.SetFacingDir(new Vector2(1, 0) * (this.DirToPath.x < 0 ? -1 : 1));
        
        if(this.jumpModule != null) {
            bool tooHigh = this.PathPos.y > this.transform.position.y + this.waypointMinDistance;
            bool edgeWay = this.collisionModule.WillFall(this.visionModule.FacingDir.x);

            if(tooHigh || edgeWay)
                jumpModule.Execute();
        }

    }

        #endregion

    void ChangeBehaviour(GroundEnemyBehaviour newBehaviour) {

        switch(myBehaviour) {
            case GroundEnemyBehaviour.Chasing:
                break;
            
            case GroundEnemyBehaviour.Idle:
                myMotor.ReceiveInput(Vector2.zero);
                break;
        }

        switch(newBehaviour) {
            case GroundEnemyBehaviour.Chasing:
                this.CalculatePath(player.transform.position);
                break;

            case GroundEnemyBehaviour.Idle:
                break;

        }

        myBehaviour = newBehaviour;

    }

}

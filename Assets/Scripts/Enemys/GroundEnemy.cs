using UnityEngine;

public enum GroundEnemyBehaviour {

    Idle,
    Chasing

}

[RequireComponent(typeof(Motor))]
[RequireComponent(typeof(CollisionCheckModule))]
[RequireComponent(typeof(VisionModule))]
public class GroundEnemy : BaseAgent
{

        #region Variables
    
    [SerializeField] private GroundEnemyBehaviour myBehaviour;

        #region References

    CollisionCheckModule collisionModule;
    VisionModule visionModule;
    Jump jumpModule;
    GameObject player;

        #endregion

        #endregion

    protected override void Start() {
        base.Start();

        this.collisionModule = this.GetComponent<CollisionCheckModule>();
        this.visionModule = this.GetComponent<VisionModule>();
        this.jumpModule = this.TryGetComponent(out Jump jump) ? jump : null;

        this.player = PlayerController.mainPlayer.gameObject;

    }

    void Update() {

        switch(myBehaviour) {
            case GroundEnemyBehaviour.Idle:
                this.IdleBehaviour();
                break;
            
            case GroundEnemyBehaviour.Chasing:
                this.ChasingBehaviour();
                break;
        }


    }

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

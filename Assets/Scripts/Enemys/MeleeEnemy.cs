using UnityEngine;

public enum MeleeEnemyBehaviour {

    Idle,
    Chasing

}

[RequireComponent(typeof(Motor))]
[RequireComponent(typeof(CollisionCheckModule))]
[RequireComponent(typeof(VisionModule))]
public class MeleeEnemy : BaseAgent
{

        #region Variables
    
    [SerializeField] private MeleeEnemyBehaviour myBehaviour;

        #region References

    CollisionCheckModule collisionModule;
    VisionModule visionModule;
    GameObject player;

        #endregion

        #endregion

    protected override void Start() {
        base.Start();

        this.collisionModule = this.GetComponent<CollisionCheckModule>();
        this.visionModule = this.GetComponent<VisionModule>();

        this.player = PlayerController.mainPlayer.gameObject;

    }

    void Update() {

        switch(myBehaviour) {
            case MeleeEnemyBehaviour.Idle:
                this.IdleBehaviour();
                break;
            
            case MeleeEnemyBehaviour.Chasing:
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

            this.ChangeBehaviour(MeleeEnemyBehaviour.Chasing);

        }

    }

    void ChasingBehaviour() {

        this.CheckWaypointDistance();
        myMotor.ReceiveInput(this.DirToPath);

        if(this.ReachedPathEnd)
            this.CalculatePath(player.transform.position);

    }

    void ChangeBehaviour(MeleeEnemyBehaviour newBehaviour) {

        switch(myBehaviour) {
            case MeleeEnemyBehaviour.Chasing:
                break;
            
            case MeleeEnemyBehaviour.Idle:
                myMotor.ReceiveInput(Vector2.zero);
                break;
        }

        switch(newBehaviour) {
            case MeleeEnemyBehaviour.Chasing:
                this.CalculatePath(player.transform.position);
                break;

            case MeleeEnemyBehaviour.Idle:
                break;

        }

        myBehaviour = newBehaviour;

    }

}

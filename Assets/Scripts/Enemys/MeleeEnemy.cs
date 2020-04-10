using UnityEngine;

[RequireComponent(typeof(Motor))]
[RequireComponent(typeof(CollisionCheckModule))]
[RequireComponent(typeof(VisionModule))]
public class MeleeEnemy : BaseEnemy
{

    Motor myMotor;
    CollisionCheckModule collisionModule;
    VisionModule visionModule;
    GameObject player;

    void Start() {

        myMotor = GetComponent<Motor>();
        collisionModule = GetComponent<CollisionCheckModule>();
        visionModule = GetComponent<VisionModule>();

        player = PlayerController.mainPlayer.gameObject;

    }

    void Update() {

        if(myMotor.OnGround)
        {
            if(collisionModule.WillFall())
                _facingDir *= -1;

            myMotor.ReceiveInput(_facingDir);
        }

        if(visionModule.CheckIsInVision(player)) {

            Debug.Log("Seen!");

        }


    }

}

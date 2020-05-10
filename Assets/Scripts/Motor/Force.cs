using UnityEngine;
using System.Collections.Generic;

namespace Undefined.Force
{

    [System.Flags]
    public enum CollisionStopBehaviour {

        AnyCollision = 15,
        AnyWall = 3,

        NotGround = 11,
        NotCelling = 7,

        DontStop = 0,
        RightWall = 1,
        LeftWall = 2,
        Ground = 4,
        Celling = 8,

        OpositeX = 16,
        OpositeY = 32
    }

    [System.Serializable]
    public class Force {

        [Header("Force Details")]
        public string Name;

        [Header("Physics")]
        public Vector2 ForceApplied;
        public Vector2 ActualForce;

        [Header("Behaviour")]
        public CollisionStopBehaviour stopBehaviour;
        public bool disableAllGravity;

        [Header("Time")]
        public float TimeToStop;
        public float Timer;
        public bool applied;

        ///<param name="force">Force to be applied</param>
        ///<param name="time">Time to the force stop</param>
        ///<param name="gravity">Should calculate gravity?</param>
        public Force(Vector2 force, float time = 0, CollisionStopBehaviour stopBehaviour = CollisionStopBehaviour.DontStop, bool disableAllGravity = false) {
            this.Name = "noname";
            this.ForceApplied = force;
            this.ActualForce = force;
            this.TimeToStop = time;
            this.stopBehaviour = stopBehaviour;
            this.disableAllGravity = disableAllGravity;
        }

        ///<param name="name">Name of the force</param>
        ///<param name="force">Force to be applied</param>
        ///<param name="time">Time to the force stop</param>
        ///<param name="gravity">Should calculate gravity?</param>
        public Force(string name, Vector2 force, float time = 0, CollisionStopBehaviour stopBehaviour = CollisionStopBehaviour.DontStop, bool disableAllGravity = false) {
            this.Name = name;
            this.ForceApplied = force;
            this.ActualForce = force;
            this.TimeToStop = time;
            this.stopBehaviour = stopBehaviour;
            this.disableAllGravity = disableAllGravity;
        }

        ///<summary>Returns the force direction</summary>
        public Vector2 Direction() {

            Vector2 dir = Vector2.zero;

            if(ActualForce.x > 0)
                dir.x = 1;
            else if(ActualForce.x < 0)
                dir.x = -1;
            else
                dir.x = 0;

            if(ActualForce.y > 0)
                dir.y = 1;
            else if(ActualForce.y < 0)
                dir.y = -1;
            else
                dir.y = 0;

            return dir;

        }


    }


}
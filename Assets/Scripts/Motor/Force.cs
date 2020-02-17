using UnityEngine;
using System.Collections.Generic;

namespace Undefined.Force
{

    [System.Serializable]
    public class Force {

        public string Name;
        public Vector2 ForceApplied;
        public Vector2 ActualForce;
        public Vector2 Gravity;
        public Vector2 FinalForce { get { return Gravity + ActualForce; } }
        public float TimeToStop;

        ///<summary>The time the force should be (0,0,0)</summary>
        public float Timer;
        public bool ApplyGravity;
        ///<summary>Tells if the force has been aplied at least once</summary>
        public bool applied;

        ///<param name="force">Force to be applied</param>
        ///<param name="time">Time to the force stop</param>
        ///<param name="gravity">Should calculate gravity?</param>
        public Force(Vector2 force, float time = 0, bool gravity = true) {
            Name = "noname";
            ForceApplied = force;
            ActualForce = (time != 0 ? Vector2.zero : force);
            TimeToStop = time;
            ApplyGravity = gravity;
            Gravity = Vector2.zero;
        }

        ///<param name="force">Force to be applied</param>
        ///<param name="name">Name of the force</param>
        ///<param name="time">Time to the force stop</param>
        ///<param name="gravity">Should calculate gravity?</param>
        public Force(string name, Vector2 force, float time = 0, bool gravity = true) {
            Name = name;
            ForceApplied = force;
            ActualForce = (time != 0 ? Vector2.zero : force);
            TimeToStop = time;
            ApplyGravity = gravity;
            Gravity = Vector2.zero;
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
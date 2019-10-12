using UnityEngine;
using System.Collections.Generic;

namespace Untitled
{

    namespace Motor
    {
        public enum SlowType {
            Gravity,
            Input,
            External,
            Constant
        }

        [System.Serializable]
        public class Force {

            public string Name;
            public Vector2 ForceApplied;
            public Vector2 ActualForce;
            public float TimeToStop;
            public bool ApplyGravity;
            public bool applied;

            ///<param name="force">Force to be applied</param>
            ///<param name="time">Time to the force stop</param>
            ///<param name="gravity">Should calculate gravity?</param>
            public Force(Vector2 force, float time = 0, bool gravity = true) {
                Name = "noname";
                ForceApplied = force;
                ActualForce = force;
                TimeToStop = time;
                ApplyGravity = gravity;
            }

            ///<param name="force">Force to be applied</param>
            ///<param name="name">Name of the force</param>
            ///<param name="time">Time to the force stop</param>
            ///<param name="gravity">Should calculate gravity?</param>
            public Force(string name, Vector2 force, float time = 0, bool gravity = true) {
                Name = name;
                ForceApplied = force;
                ActualForce = force;
                TimeToStop = time;
                ApplyGravity = gravity;
            }

        }

        [System.Serializable]
        public class Slow {

            public string Name;
            public Vector2 Value;
            public List<SlowType> Types;

            ///<param name="slow">Total of slowness</param>
            public Slow(Vector2 slow, List<SlowType> type = null) {
                Name = "noname";
                Value = slow;

                if(type == null)
                    Types = new List<SlowType>{ SlowType.Input };
            }

            ///<param name="name">Name of the slow</param>
            ///<param name="slow">Total of slowness</param>
            ///<param name="type">Type of slowness</param>
            public Slow(string name, Vector2 slow, List<SlowType> type = null) {

                Name = name;
                Value = slow;

                if(type == null)
                    Types = new List<SlowType> { SlowType.Input };

            }

            ///<param name="name">Name of the slow</param>
            ///<param name="slow">Total of slowness</param>
            ///<param name="type">Type of slowness</param>
                public Slow(string name, Vector2 slow, SlowType type) {

                Name = name;
                Value = slow;
                Types = new List<SlowType> { type };

            }

        }
    }

}
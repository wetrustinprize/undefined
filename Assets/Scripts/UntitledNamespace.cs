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

            ///<param name="f">Force to be applied</param>
            ///<param name="t">Time to the force stop</param>
            ///<param name="g">Should calculate gravity?</param>
            public Force(Vector2 f, float t = 0, bool g = true) {
                Name = "noname";
                ForceApplied = f;
                ActualForce = f;
                TimeToStop = t;
                ApplyGravity = g;
            }

            ///<param name="f">Force to be applied</param>
            ///<param name="n">Name of the force</param>
            ///<param name="t">Time to the force stop</param>
            ///<param name="g">Should calculate gravity?</param>
            public Force(string n, Vector2 f, float t = 0, bool g = true) {
                Name = n;
                ForceApplied = f;
                ActualForce = f;
                TimeToStop = t;
                ApplyGravity = g;
            }

        }

        [System.Serializable]
        public class Slow {

            public string Name;
            public Vector2 Value;
            public List<SlowType> Types;

            ///<param name="s">Total of slowness</param>
            public Slow(Vector2 s) {
                Name = "noname";
                Value = s;
            }

            ///<param name="n">Name of the slow</param>
            ///<param name="s">Total of slowness</param>
            ///<param name="t">Type of slowness</param>
            public Slow(string n, Vector2 s, List<SlowType> t = null) {

                Name = n;
                Value = s;

                if(t == null)
                    Types = new List<SlowType> { SlowType.Input };

            }

            ///<param name="n">Name of the slow</param>
            ///<param name="s">Total of slowness</param>
            ///<param name="t">Type of slowness</param>
                public Slow(string n, Vector2 s, SlowType t) {

                Name = n;
                Value = s;
                Types = new List<SlowType> { t };

            }

        }
    }

}
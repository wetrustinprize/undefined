using UnityEngine;
using System.Collections.Generic;

namespace Undefined.Force 
{
    public enum SlowType {
        Gravity,
        Input,
        External,
        Constant
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
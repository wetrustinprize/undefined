using UnityEngine;

namespace Undefined.Force 
{
    [System.Flags]
    public enum SlowType {
        Gravity = 1,
        Input = 2,
        External = 4,
        Constant = 8,

        AllTypes = 15
    }
    
    [System.Serializable]
    public class Slow {


        public string Name;
        public Vector2 Value;
        public SlowType Types;

        ///<param name="slow">Total of slowness</param>
        ///<param name="type">Type of slowness</param>
        public Slow(Vector2 slow, SlowType type = SlowType.AllTypes) {
            Name = "noname";
            Value = new Vector2(Mathf.Clamp(slow.x, 0.0f, 1.0f), Mathf.Clamp(slow.y, 0.0f, 1.0f));
            Types = type;
        }

        ///<param name="name">Name of the slow</param>
        ///<param name="slow">Total of slowness</param>
        ///<param name="type">Type of slowness</param>
        public Slow(string name, Vector2 slow, SlowType type = SlowType.AllTypes) {
            Name = name;
            Value = new Vector2(Mathf.Clamp(slow.x, 0.0f, 1.0f), Mathf.Clamp(slow.y, 0.0f, 1.0f));
            Types = type;
        }

    }
}
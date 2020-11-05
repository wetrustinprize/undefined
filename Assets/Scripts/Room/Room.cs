using UnityEngine;

namespace Undefined.Rooms {

    [System.Serializable]
    public class Room {

            #region Variables

        [Header("Room Information")]
        public Vector2Int size;

        public Vector2 origin;  
        //public Vector3 origin { get; private set; }
        public Vector2Int tilesetSize { get { return size * 2; } }

            #endregion

        public Room(Vector2Int size, Vector3 origin) {
            this.size = size;
            this.origin = origin;
        }

    }

}
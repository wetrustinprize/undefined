using UnityEngine;
using Undefined.Rooms;

public class RoomTrigger : MonoBehaviour {

        #region Variables

    [SerializeField]
    private Room room;

    // Public acess variables
    public Room Room { get { return room; } }

    // Script side variables
    private BoxCollider2D trigger;

        #endregion

    void Start() {

        // Gets the player collider size
        Vector2 psize = GameManager.Player.GetComponent<BoxCollider2D>().size;

        // Set Room origin
        this.room.origin = this.transform.position;

        // Create room trigger
        this.gameObject.layer = 10;

        Vector2 triggerSize = Vector2.zero;
        Vector2 triggerOffset = Vector2.zero;

        triggerSize = room.tilesetSize - psize * 2;
        triggerOffset = room.tilesetSize / 2;

        trigger = this.gameObject.AddComponent<BoxCollider2D>();
        trigger.isTrigger = true;
        trigger.size = triggerSize;
        trigger.offset = triggerOffset;

    }

    void OnTriggerEnter2D(Collider2D col) {
        if(col.gameObject.tag == "Player")
            GameManager.Camera.SetRoom(this.room);
    }

    void OnDrawGizmos() {

        if(this.room.size.x + this.room.size.y <= 0) return;

        Gizmos.DrawWireCube(
            this.transform.position + (Vector3)(Vector2)room.tilesetSize / 2,
            (Vector2)room.tilesetSize
        );

    }

}
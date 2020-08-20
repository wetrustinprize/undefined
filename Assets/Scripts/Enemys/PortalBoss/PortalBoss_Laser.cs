using UnityEngine;

public class PortalBoss_Laser : MonoBehaviour
{

        #region Variable

    [SerializeField] private float laserVelocity;

    // Private variables
    private GameObject player;

    private Vector2 direction;

        #endregion

    void Start() {

        this.player = GameManager.Player.gameObject;

        this.direction = this.player.transform.position - this.transform.position;
        this.direction.Normalize();

    }

    // Update is called once per frame
    void FixedUpdate()
    {

        this.transform.position = this.transform.position + ((Vector3)this.direction * laserVelocity * Time.fixedDeltaTime);

    }
}

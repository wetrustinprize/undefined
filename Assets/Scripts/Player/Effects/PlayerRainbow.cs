using UnityEngine;

public class PlayerRainbow : MonoBehaviour
{

        #region Variables

    [Header("Trails")]
    [SerializeField] private TrailRenderer[] trails = null;

    // Script side
    private GameObject myPlayer = null;

        #endregion

    public void Setup(GameObject player)
    {
        
        myPlayer = player;

        Jump jump = player.GetComponent<Jump>();
        Motor motor = player.GetComponent<Motor>();

        jump.OnWallJump += WallJump;
        motor.onTouchGround += OnTouchGround;

        ChangeTrail(false);

    }

    void OnDestroy() {

        Jump jump = myPlayer.GetComponent<Jump>();
        Motor motor = myPlayer.GetComponent<Motor>();

        jump.OnWallJump -= WallJump;
        motor.onTouchGround -= OnTouchGround;

    }

    void ChangeTrail(bool emit) {

        foreach(TrailRenderer tr in trails)
        {
            tr.emitting = emit;
        }

    }

    void WallJump() {

        ChangeTrail(true);

    }

    void OnTouchGround() {

        ChangeTrail(false);

    }

}

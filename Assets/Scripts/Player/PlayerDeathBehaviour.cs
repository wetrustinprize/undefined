using UnityEngine;

[RequireComponent(typeof(Alive))]
public class PlayerDeathBehaviour : MonoBehaviour
{
    
        #region Variables

    private Alive myAlive;

        #endregion


    void Start() {

        GameManager.Player.alive.onDie += Death;

    }

    void Death() {

        GameManager.Player.animator.DeathAnimation();

        GameManager.HUD.HideAllHUD();
        GameManager.HUD.canOpenInventory = false;

        GameManager.Player.receiveInput = false;

        GameManager.Player.alive.CanReceiveDamage = false;
        GameManager.Player.alive.CanReceiveHeal = false;

        GameManager.Player.motor.SetFreeze(true);

    }

}

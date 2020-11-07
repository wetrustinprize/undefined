using UnityEngine;
using System.Collections;

public class HUDMainMenu : MonoBehaviour
{

        #region Variables

    [SerializeField]
    private GameObject look_at;

    // Script side
    private Animator look_at_anim;
    private Animator hud_animator;

    private bool started;

        #endregion

    // Start is called before the first frame update
    void Awake()
    {
        look_at_anim = look_at.GetComponent<Animator>();
        hud_animator = this.gameObject.GetComponent<Animator>();
    }

    void Start()
    {
        StartCoroutine("freezePlayer");
    }

    IEnumerator freezePlayer() {
        yield return new WaitForSeconds(0.5f);
        GameManager.Player.receiveInput = false;
        GameManager.Player.motor.SetFreeze(true);

        GameManager.Camera.LookAt(look_at.transform);

        GameManager.HUD.HideAllHUD();
    }

    public void PlayGame() {
        if(started) return;
        started = true;

        look_at_anim.SetTrigger("Start");
        hud_animator.SetTrigger("Start");
    }

    public void EndAnimation() {
        GameManager.Player.receiveInput = true;
        GameManager.Player.motor.SetFreeze(false);
        GameManager.Camera.LookAtPlayer();

        GameManager.HUD.UpdateHUD(HUDType.Health, false);
        GameManager.HUD.UpdateHUD(HUDType.Items, false);

        Destroy(this.gameObject);
    }

}

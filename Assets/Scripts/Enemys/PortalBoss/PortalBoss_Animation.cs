using UnityEngine;

public class PortalBoss_Animation : MonoBehaviour {

        #region Variables

    [Header("Portal Boss")]
    [SerializeField] private Animator myAnimator;

    [Header("End Portal")]
    [SerializeField] private Animator endAnimator;

    [Header("Laser Rock")]
    [SerializeField] private Animator laserAnimator;
    [SerializeField] private LineRenderer laserLineRender;

        #endregion

    public void StartAnimation() {

        this.myAnimator.SetTrigger("Start");

    }

    public void FocusOnPortal() {

        GameManager.Camera.LookAt(this.transform, 0.1f);
        GameManager.Player.receiveInput = false;
        GameManager.Player.ResetInput();
        

    }

    public void FocusOnEnd() {

        GameManager.Camera.LookAt(endAnimator.transform, 0.1f);
        endAnimator.SetTrigger("Open");

    }

    public void LaserPortalSend() {

        Vector3[] pos = new Vector3[] { this.laserAnimator.transform.position, this.endAnimator.transform.position };

        this.laserLineRender.SetPositions(pos);
        this.laserAnimator.SetFloat("PrepSpeed", 1);
        this.laserAnimator.SetTrigger("Shot");

    }

    public void SmoothLookAtPlayer() {

        GameManager.Camera.LookAt(GameManager.Player.transform, 0.1f);

    }

    public void FocusOnPlayer() {

        GameManager.Camera.LookAtPlayer();
        GameManager.Player.receiveInput = true;
        Destroy(this.myAnimator);
        Destroy(this);

    }

}
using UnityEngine;

public class PortalBoss_Animation : MonoBehaviour {

    public void FocusOnPortal() {

        GameManager.Camera.LookAt(this.transform, 0.3f);

    }

    public void FocusOnPlayer() {

        GameManager.Camera.LookAtPlayer();

    }

}
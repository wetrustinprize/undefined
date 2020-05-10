using UnityEngine;

public class Tutorial_Boxfade : MonoBehaviour
{
        #region Variables

    private Animator myAnimator;
    private bool shouldShow = false;

        #endregion


    void Start() {
        this.myAnimator = this.GetComponent<Animator>();
    }

    void OnTriggerEnter2D(Collider2D col) {
        this.shouldShow = col.tag == "Player" ? true : this.shouldShow;
        this.myAnimator.SetBool("fadein", shouldShow);
    }

    void OnTriggerExit2D(Collider2D col) {
        this.shouldShow = col.tag == "Player" ? false : this.shouldShow;
        this.myAnimator.SetBool("fadein", shouldShow);
    }
}

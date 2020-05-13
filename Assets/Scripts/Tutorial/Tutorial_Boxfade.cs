using UnityEngine;

public class Tutorial_Boxfade : MonoBehaviour
{
        #region Variables

    [Header("Animator")]
    [SerializeField] private Animator animator;

    // script side
    private bool shouldShow = false;

        #endregion


    void Start() {
        this.animator = animator == null ? this.GetComponent<Animator>() : animator;
    }

    void OnTriggerEnter2D(Collider2D col) {
        this.shouldShow = col.tag == "Player" ? true : this.shouldShow;
        this.animator.SetBool("fadein", shouldShow);
    }

    void OnTriggerExit2D(Collider2D col) {
        this.shouldShow = col.tag == "Player" ? false : this.shouldShow;
        this.animator.SetBool("fadein", shouldShow);
    }
}

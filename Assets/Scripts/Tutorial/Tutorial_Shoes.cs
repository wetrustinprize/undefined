using UnityEngine;

public class Tutorial_Shoes : MonoBehaviour
{
    
        #region Variables

    private Animator myAnimator;

        #endregion

    void Start() {

        this.myAnimator = GetComponent<Animator>();

    }

    void OnTriggerEnter2D(Collider2D col) {

        if(col.tag != "Player") return;

        myAnimator.SetTrigger("Pickup");

    }

}

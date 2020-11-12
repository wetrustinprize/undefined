using UnityEngine;

[DisallowMultipleComponent]
public class PlayerInteraction : MonoBehaviour {

        #region Variables

    [SerializeField] private Interactible curInteractible = null;
    [SerializeField] private Animator iteractibleAnimator = null;

        #endregion

    public void Interact() {

        if(curInteractible == null) return;

        curInteractible.Interact();

    }

    public void SetInteractible(Interactible newInteractible, bool force = false) {
        
        if(curInteractible == null)
        {
            curInteractible = newInteractible;
            curInteractible?.SetSelect(true);
            iteractibleAnimator.SetBool("Show", true);
            return;
        }

        float curDist = Vector2.Distance(this.transform.position, curInteractible.transform.position);
        float newDist = Vector2.Distance(this.transform.position, newInteractible.transform.position);

        if(newDist < curDist || force) {

            curInteractible?.SetSelect(false);

            curInteractible = newInteractible;
            curInteractible?.SetSelect(true);
            iteractibleAnimator.SetBool("Show", true);

        }

    }

    public void RemoveInteractible(Interactible interactible) {

        interactible.SetSelect(false);
        
        if(curInteractible == interactible) 
        {
            curInteractible = null;
            iteractibleAnimator.SetBool("Show", false);
        }

    }

}
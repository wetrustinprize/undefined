using UnityEngine;

[DisallowMultipleComponent]
public class PlayerInteraction : MonoBehaviour {

        #region Variables

    [SerializeField] private Interactible curInteractible = null;

        #endregion

    public void Interact() {

        if(curInteractible == null) return;

        curInteractible.Interact();

    }

    public void SetInteractible(Interactible newInteractible, bool force = false) {

        if(force || curInteractible == null) curInteractible = newInteractible;
        
        float curDist = Vector2.Distance(this.transform.position, curInteractible.transform.position);
        float newDist = Vector2.Distance(this.transform.position, newInteractible.transform.position);

        if(newDist < curDist) {

            curInteractible.SetSelect(false);
            newInteractible.SetSelect(true);

            curInteractible = newInteractible;

        }

    }

    public void RemoveInteractible(Interactible interactible) {

        interactible.SetSelect(false);
        
        if(curInteractible == interactible) curInteractible = null;

    }

}
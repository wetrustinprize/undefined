using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
[DisallowMultipleComponent]
public class Interactible : MonoBehaviour {

        #region Variables

    [SerializeField] private Animator indicator;
    [SerializeField] private UnityEvent onInteract;

        #endregion

    void OnTriggerEnter2D(Collider2D col) {

        if(col.gameObject.tag == "Player") {

            col.GetComponent<PlayerInteraction>().SetInteractible(this);

        }

    }

    void OnTriggerExit2D(Collider2D col) {

        if(col.gameObject.tag == "Player") {
            
            col.GetComponent<PlayerInteraction>().RemoveInteractible(this);

        }

    }

    public void Interact() {
        
        onInteract?.Invoke();

    }

    public void SetSelect(bool selected) {

        if(indicator == null) return;

        indicator.SetBool("Selected", selected);

    }

}
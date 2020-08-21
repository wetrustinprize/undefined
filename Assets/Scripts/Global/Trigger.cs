using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
[DisallowMultipleComponent]
public class Trigger : MonoBehaviour 
{

    [System.Flags]
    enum TriggerType {

        Enter = 1,
        Exit = 2,

    }

        #region Variables

    [Header("Events")]
    [SerializeField] private UnityEvent onEnterTrigger = null;
    [SerializeField] private UnityEvent onExitTrigger = null;

    [Header("Filter")]
    [SerializeField] private List<string> filterTags = new List<string>();

    [Header("Repeat")]
    [SerializeField] private TriggerType repeatType = 0;

    // Script side
    private bool hasEntered = false;
    private bool hasExited = false;

        #endregion

    void OnTriggerEnter2D(Collider2D col) {

        if(!filterTags.Exists(c => c == col.gameObject.tag)) return;
        if(!repeatType.HasFlag(TriggerType.Enter) && hasEntered) return;
        hasEntered = true;

        onEnterTrigger?.Invoke();

    }

    void OnTriggerExit2D(Collider2D col) {          

        if(!filterTags.Exists(c => c == col.gameObject.tag)) return;
        if(!repeatType.HasFlag(TriggerType.Exit) && hasExited) return;
        hasExited = true;

        onExitTrigger?.Invoke();

    }

}
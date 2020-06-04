using UnityEngine;

public class Lever : MonoBehaviour
{
    
        #region Variables

    [Header("Lever Settings")]
    [SerializeField] private Color leverColor = Color.white;
    [SerializeField] private SpriteRenderer leverSprite = null;

    [Header("Lever Debug Sprites")]    
    [SerializeField] private Sprite leverOn = null;
    [SerializeField] private Sprite leverOff = null;

    [Header("Doors")]
    [SerializeField] private Door[] doorsToActivate = null;

    // script side
    private bool spriteOn = false;

        #endregion

    void Start() {

        // Updates the lever sprite color
        leverSprite.color = leverColor;

        foreach(Door d in doorsToActivate)
        {
            d.AddDoorColor(leverColor);
        }

    }

    public void Toggle() {

        leverSprite.sprite = spriteOn ? leverOff : leverOn;
        spriteOn = !spriteOn;

        foreach(Door d in doorsToActivate)
        {
            d.Toggle();
        }

    }

    [ExecuteInEditMode]
    void OnValidate() {
        leverSprite.color = leverColor;
    }

    void OnDrawGizmos() {

        if(doorsToActivate == null || doorsToActivate.Length <= 0) return;

        foreach(Door door in doorsToActivate) {
            if(door == null) continue;

            Vector3 doorPos = door.transform.position;

            Gizmos.color = leverColor;
            Gizmos.DrawLine(this.transform.position, doorPos);
        }

    }


}

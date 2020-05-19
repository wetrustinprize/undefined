using UnityEngine;

public class Lever : MonoBehaviour
{
    
        #region Variables

    [Header("Lever Settings")]
    [SerializeField] private Color leverColor;

    [Header("Doors")]
    [SerializeField] private Door[] doorsToActivate;

        #endregion

    void OnDrawGizmos() {

        if(doorsToActivate == null || doorsToActivate.Length <= 0) return;

        foreach(Door door in doorsToActivate) {
            Vector3 doorPos = door.transform.position;

            Gizmos.color = leverColor;
            Gizmos.DrawLine(this.transform.position, doorPos);
        }

    }


}
